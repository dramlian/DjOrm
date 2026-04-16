using System.Linq.Expressions;

public interface ISelectByUtility
{
    public Task<IEnumerable<T>> GetByExpression<T>(Expression<Func<T, bool>>? expression = null, bool recursive = false);
}