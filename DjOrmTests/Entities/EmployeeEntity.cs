[TableAttribute]
public class EmployeeEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public int Age { get; set; }

    public EmployeeEntity() { }
}