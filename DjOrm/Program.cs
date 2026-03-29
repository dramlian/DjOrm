public class DjOrm
{
    public static void Main()
    {
        TableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        var entities = entitiesMaker.CreateObjectEntities();
    }
}