public interface IDeleteUtility<T>
{
    Task DeleteData(T input);
}