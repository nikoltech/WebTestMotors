namespace WebTestMotors.DataAccess
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDb;
    using MongoDB.Driver;

    public class DataContext : IDataContext
    {
        private readonly IMongoDatabase db;

        public DataContext(IMongoDatabase database)
        {
            this.db = database;
        }

        public IMongoCollection<Car> Cars => db.GetCollection<Car>("Cars");
    }
}
