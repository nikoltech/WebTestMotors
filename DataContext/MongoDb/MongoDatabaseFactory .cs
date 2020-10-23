namespace WebTestMotors.DataAccess.MongoDb
{
    using MongoDB.Driver;
    using System;

    public class MongoDatabaseFactory : IMongoDatabaseFactory
    {
        public IMongoDatabase Connect(string connectionString, string dbName)
        {
            connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString :
                throw new ArgumentNullException("connectionString");

            dbName = !string.IsNullOrWhiteSpace(dbName) ? dbName :
                throw new ArgumentNullException("dbName");

            var dbClient = new MongoClient(connectionString);
            return dbClient.GetDatabase(dbName);
        }
    }
}
