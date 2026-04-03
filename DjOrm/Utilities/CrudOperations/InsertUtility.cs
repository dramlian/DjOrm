public class InsertUtility<T>
{
    private IDatabaseConnector dbConnect;

    public InsertUtility(IDatabaseConnector dbConnect)
    {
        this.dbConnect = dbConnect;
    }

    internal void InsertInputs(T inputs)
    {
        throw new NotImplementedException();
    }
}