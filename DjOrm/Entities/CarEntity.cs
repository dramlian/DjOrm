[TableAttribute]
public class CarEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Make { get; set; }

    [SecondaryKeyAttribute]
    public DriverEntity Driver { get; set; }

    [SecondaryKeyAttribute]
    public DriverEntity DriverTwo { get; set; }
}