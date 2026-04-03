public class DbContext<T> : IDbContext<T>
{
    private T _data;
    private InsertUtility<T> _insertUtility;

    public DbContext(T data, IDatabaseConnector dbConnect)
    {
        _data = data;
        _insertUtility = new InsertUtility<T>(dbConnect);
    }

    public void InsertData(IEnumerable<T> inputs)
    {
        foreach (var input in inputs)
        {
            InsertData(input);
        }
    }

    public void InsertData(T input)
    {
        _insertUtility.InsertInputs(input);
    }

    public void DeleteData(IEnumerable<T> inputs)
    {

    }

    public void DeleteData(T input)
    {

    }

    public void UpdateData(IEnumerable<T> inputs)
    {

    }

    public void UpdateData(T input)
    {

    }
}