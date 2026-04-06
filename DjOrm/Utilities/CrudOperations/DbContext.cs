public class DbContext<T> : IDbContext<T>
{
    private IInsertUtility<T> _insertUtility;
    private ISelectUtility<T> _selectUtility;


    public DbContext(IDatabaseConnector dbConnect)
    {
        _insertUtility = new InsertUtility<T>(dbConnect);
        _selectUtility = new SelectUtility<T>(dbConnect);
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
        if (input is null) return;
        await _insertUtility.InsertInputs(input);
    }

    public async Task<IEnumerable<T>> GetData()
    {
        return await _selectUtility.GetAllData();
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