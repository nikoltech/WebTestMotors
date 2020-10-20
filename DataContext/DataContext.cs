namespace WebTestMotors.DataAccess
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDb;
    using MongoDB.Driver;

    public class DataContext
    {
        public DataContext(IMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.Cars = database.GetCollection<Car>("Cars");
        }

        public readonly IMongoCollection<Car> Cars;
    }
}
