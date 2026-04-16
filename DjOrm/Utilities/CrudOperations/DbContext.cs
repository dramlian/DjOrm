using System.Linq.Expressions;
public class DbContext<T> : IDbContext<T>
{
    private IInsertUtility<T> _insertUtility;
    private ISelectByUtility _selectByUtility;
    private IDeleteUtility<T> _deleteUtility;
    private IUpdateUtility<T> _updateUtility;



    public DbContext(IDatabaseConnector dbConnect)
    {
        _insertUtility = new InsertUtility<T>(dbConnect);
        _deleteUtility = new DeleteUtility<T>(dbConnect);
        _updateUtility = new UpdateUtility<T>(dbConnect);
        _selectByUtility = new SelectByUtility(dbConnect);
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

    public async Task UpdateData(T input)
    {
        await _updateUtility.UpdateData(input);
    }

    public async Task<IEnumerable<T>> GetData(bool isRecursive = false)
    {
        return await _selectByUtility.GetByExpression<T>(null, isRecursive);
    }

    public async Task<IEnumerable<T>> GetDataBy(Expression<Func<T, bool>> expression, bool isRecursive = false)
    {
        return await _selectByUtility.GetByExpression<T>(expression, isRecursive);
    }
}