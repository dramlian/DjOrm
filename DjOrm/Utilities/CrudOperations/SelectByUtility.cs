using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

public class SelectByUtility<T> : Utility, ISelectByUtility<T>
{
    public SelectByUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {
    }

    public async Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>>? expression = null)
    {
        var properties = GetPropertyInfos();
        var command = $"SELECT * FROM {typeof(T).FullName} {(expression == null ? ";" : $"WHERE {Travel(expression.Body)};")}";
        var rows = (await _dbConnect.GetDataReaderResults(command, properties.Count())).ToArray();
        return CastRowsIntoObjects(rows, properties);
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

    private PropertyInfo[] GetPropertyInfos()
    {
        return typeof(T).GetProperties()
                       .Where(p => !p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .ToArray();
    }

    private IEnumerable<T> CastRowsIntoObjects(object[] rows, System.Reflection.PropertyInfo[] properties)
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