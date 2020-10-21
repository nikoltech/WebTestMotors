namespace WebTestMotors.DataAccess
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDb;
    using MongoDB.Driver;

    public class DataContext : IDataContext
    {
        private readonly IMongoDatabase db;

        public DataContext(IMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            this.db = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Car> Cars => db.GetCollection<Car>("Cars");
    }
}
