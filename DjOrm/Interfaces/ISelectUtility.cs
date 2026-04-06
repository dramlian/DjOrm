public interface ISelectUtility<T>
{
    public Task<IEnumerable<T>> GetAllData();
}