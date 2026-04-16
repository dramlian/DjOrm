using System.Linq.Expressions;

public interface IDbContext<T>
{
    public Task InsertData(T input);
    public Task DeleteData(T input);
    public Task UpdateData(T input);
    public Task<IEnumerable<T>> GetData(bool isRecursive = false);
    public Task<IEnumerable<T>> GetDataBy(Expression<Func<T, bool>> expression, bool isRecursive = false);
}