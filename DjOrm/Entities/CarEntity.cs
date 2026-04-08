[TableAttribute]
public class CarEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;

    [SecondaryKeyAttribute]
    public DriverEntity? Driver { get; set; }

    [SecondaryKeyAttribute]
    public DriverEntity? DriverTwo { get; set; }

    public CarEntity(string name, string make, DriverEntity driver)
    {
        Name = name;
        Make = make;
        Driver = driver;
    }

    public CarEntity()
    {

    }
}