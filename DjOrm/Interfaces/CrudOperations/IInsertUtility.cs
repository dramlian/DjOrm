public interface IInsertUtility<T>
{
    public Task<(int, string)> InsertInputs(object input);
}