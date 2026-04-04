public class DbContext<T> : IDbContext<T>
{
    private InsertUtility<T> _insertUtility;

    public DbContext(IDatabaseConnector dbConnect)
    {
        _insertUtility = new InsertUtility<T>(dbConnect);
    }

    public async Task InsertData(IEnumerable<T> inputs)
    {
        foreach (var input in inputs)
        {
            await InsertData(input);
        }
    }

    public async Task InsertData(T input)
    {
        await _insertUtility.InsertInputs(input);
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