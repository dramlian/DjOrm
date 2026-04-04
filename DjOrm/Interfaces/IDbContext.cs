public interface IDbContext<T>
{
    public Task InsertData(IEnumerable<T> inputs);
    public Task InsertData(T input);

    void DeleteData(T input);
    void DeleteData(IEnumerable<T> inputs);

    void UpdateData(T input);
    void UpdateData(IEnumerable<T> inputs);
}