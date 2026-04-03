public interface IDbContext<T>
{
    void InsertData(T input);
    void InsertData(IEnumerable<T> inputs);

    void DeleteData(T input);
    void DeleteData(IEnumerable<T> inputs);

    void UpdateData(T input);
    void UpdateData(IEnumerable<T> inputs);
}