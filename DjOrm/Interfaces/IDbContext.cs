public interface IDbContext<T>
{
    public Task InsertData(T input);
    public Task DeleteData(T input);
    public Task<T> UpdateData(T input);
    public Task<IEnumerable<T>> GetData();
}