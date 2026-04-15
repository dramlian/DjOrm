using System.Linq.Expressions;

public interface ISelectByUtility<T>
{
    public Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>>? expression = null);
}