public class DbContext<T> : IDbContext<T>
{
    private IInsertUtility<T> _insertUtility;
    private ISelectUtility<T> _selectUtility;
    private IDeleteUtility<T> _deleteUtility;


    public DbContext(IDatabaseConnector dbConnect)
    {
        _insertUtility = new InsertUtility<T>(dbConnect);
        _selectUtility = new SelectUtility<T>(dbConnect);
        _deleteUtility = new DeleteUtility<T>(dbConnect);
    }

    public async Task InsertData(T input)
    {
        if (input is null) return;
        await _insertUtility.InsertInputs(input);
    }

    public async Task DeleteData(T input)
    {
        await _deleteUtility.DeleteData(input);
    }

    public Task<T> UpdateData(T input)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> GetData()
    {
        return await _selectUtility.GetAllData();
    }
}