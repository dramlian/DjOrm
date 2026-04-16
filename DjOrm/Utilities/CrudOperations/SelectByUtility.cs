using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

public class SelectByUtility : Utility, ISelectByUtility
{
    public SelectByUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {
    }

    public async Task<IEnumerable<T>> GetByExpression<T>(Expression<Func<T, bool>>? expression = null, bool recursive = false)
    {
        var properties = GetPropertyInfos(typeof(T));
        var command = $"SELECT * FROM {typeof(T).FullName} {(expression == null ? ";" : $"WHERE {Travel(expression.Body)};")}";
        var rows = (await _dbConnect.GetDataReaderResults(command, properties.Count())).ToArray();
        var castedObjects = CastRowsIntoObjects<T>(rows, properties);
        var relations = GetRelationsProperties(typeof(T)).DistinctBy(x => x.PropertyType);

        if (recursive)
        {
            foreach (var castedObject in castedObjects)
            {
                if (castedObject is null) continue;
                var pk = GetNameValueOfProperty(castedObject, [typeof(PrimaryKeyAttribute)])?.First();
                if (pk is null || pk.Value.Item2 is null) continue;

                foreach (var relation in relations)
                {
                    var getRelationsCommand = $"SELECT * FROM {typeof(T).FullName + relation.PropertyType.FullName} WHERE {typeof(T).FullName +
                    pk.Value.Item1} = {AppendQuotes(pk.Value.Item2, pk.Value.Item2.GetType())};";

                    var fetchedRelationID = ((ArrayList)(await _dbConnect.GetDataReaderResults(getRelationsCommand, 2)).First())[1];
                    if (fetchedRelationID is null) continue;
                    var lambda = BuildEqualsExpression(relation.PropertyType, fetchedRelationID);
                    var fetchedRelation = (await GetByExpressionDynamic(relation.PropertyType, lambda))?.First();
                    relation.SetValue(castedObject, fetchedRelation);
                }

            }
        }

        return castedObjects;
    }

    private LambdaExpression BuildEqualsExpression(Type type, object value)
    {
        var param = Expression.Parameter(type, "x");

        var pkProperty = type
            .GetProperties()
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(PrimaryKeyAttribute)))
            ?? throw new Exception($"No PrimaryKeyAttribute found on {type.Name}");

        var property = Expression.Property(param, pkProperty);
        var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
        var convertedValue = Convert.ChangeType(value, targetType);
        var constant = Expression.Constant(convertedValue, property.Type);
        var body = Expression.Equal(property, constant);

        return Expression.Lambda(body, param);
    }

    private async Task<IEnumerable<object>?> GetByExpressionDynamic(Type type, LambdaExpression? expression = null)
    {
        var method = GetType()
            .GetMethod(nameof(GetByExpression))!
            .MakeGenericMethod(type);

        var task = (Task)method.Invoke(
            this,
            new object?[] { expression as object, true }
        )!;

        await task.ConfigureAwait(false);
        var resultProperty = task.GetType().GetProperty("Result");
        var result = resultProperty?.GetValue(task);

        if (result is System.Collections.IEnumerable enumerable)
        {
            return enumerable.Cast<object>();
        }

        return null;
    }

    private string Travel(Expression input)
    {
        var binaryExpresison = input as BinaryExpression;
        var memberExpression = input as MemberExpression;
        var constantExpression = input as ConstantExpression;

        if (binaryExpresison != null)
        {
            if (binaryExpresison.NodeType == ExpressionType.AndAlso)
            {
                return $"({Travel(binaryExpresison.Left)} AND {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.Equal)
            {
                return $"({Travel(binaryExpresison.Left)} = {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.NotEqual)
            {
                return $"({Travel(binaryExpresison.Left)} <> {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.OrElse)
            {
                return $"({Travel(binaryExpresison.Left)} OR {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.LessThan)
            {
                return $"({Travel(binaryExpresison.Left)} < {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.LessThanOrEqual)
            {
                return $"({Travel(binaryExpresison.Left)} <= {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.GreaterThan)
            {
                return $"({Travel(binaryExpresison.Left)} > {Travel(binaryExpresison.Right)})";
            }
            if (binaryExpresison.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                return $"({Travel(binaryExpresison.Left)} >= {Travel(binaryExpresison.Right)})";
            }
        }
        else if (memberExpression != null)
        {
            if (memberExpression.Expression is ParameterExpression)
            {
                return memberExpression.Member.Name;
            }
            var memberExpressionValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
            if (memberExpressionValue is not null)
            {
                return AppendQuotes(memberExpressionValue, memberExpressionValue.GetType()).ToString() ?? string.Empty;
            }
        }
        else if (constantExpression?.Value != null)
        {
            return AppendQuotes(constantExpression.Value, constantExpression.Value.GetType()).ToString() ?? string.Empty;
        }

        return input.ToString();
    }

    private PropertyInfo[] GetPropertyInfos(Type type)
    {
        return type.GetProperties()
                       .Where(p => !p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .ToArray();
    }

    private PropertyInfo[] GetRelationsProperties(Type type)
    {
        return type.GetProperties()
                       .Where(p => p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .ToArray();
    }

    private IEnumerable<T> CastRowsIntoObjects<T>(object[] rows, System.Reflection.PropertyInfo[] properties)
    {
        var result = new List<T>();

        foreach (var row in rows.Cast<ArrayList>())
        {
            var obj = Activator.CreateInstance<T>() ??
            throw new InvalidOperationException($"Could not create instance of {typeof(T).Name}");
            for (int i = 0; i < properties.Length; i++)
            {
                var prop = obj.GetType().GetProperty(properties[i].Name);
                prop?.SetValue(obj, Convert.ChangeType(row[i], properties[i].PropertyType));
            }
            result.Add(obj);
        }

        return result;
    }
}