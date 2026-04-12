# DjOrm

A lightweight ORM for .NET that maps C# classes to PostgreSQL tables using custom attributes and a generic `DbContext<T>`.

## Features

- Auto-generates `CREATE TABLE` SQL from annotated C# entities
- Junction table generation for entity relationships
- Full CRUD operations via a single generic context
- Translates simple LINQ expressions into SQL `WHERE` clauses
- Async throughout

## Usage

**1. Annotate your entities**

```csharp
[TableAttribute]
public class CarEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Make { get; set; }

    [SecondaryKeyAttribute]
    public DriverEntity? Driver { get; set; }
}
```

**2. Create tables and run queries**

```csharp
var db = new DatabaseConnector(connString);

// Auto-create tables from entity definitions
var entities = new TableEntitiesMaker(new TypeTranslator()).CreateObjectEntities();
await db.ExecuteCommands(new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables());

// CRUD
IDbContext<CarEntity> ctx = new DbContext<CarEntity>(db);

await ctx.InsertData(new CarEntity("Civic", "Honda", driver));
var all    = await ctx.GetData();
var hondas = await ctx.GetDataBy(x => x.Make == "Honda" && x.Name != "Accord");
await ctx.UpdateData(car);
await ctx.DeleteData(car);
```

**LINQ expression support** — `GetDataBy` walks the expression tree and translates binary expressions (`==`, `!=`, `<`, `>`, `&&`, `||`) directly into parameterized SQL conditions.

## Configuration

Connection string is loaded from a `.env` file:
