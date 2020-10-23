namespace WebTestMotors.DataAccess.MongoDb
{
    using MongoDB.Driver;
    
    public interface IMongoDatabaseFactory
    {
        IMongoDatabase Connect(string connectionString, string dbName);
    }
}
