using System.Linq.Expressions;

public interface IDbContext<T>
{
    public Task InsertData(T input);
    public Task DeleteData(T input);
    public Task UpdateData(T input);
    public Task<IEnumerable<T>> GetData();
    public Task<IEnumerable<T>> GetDataBy(Expression<Func<T, bool>> expression);
}