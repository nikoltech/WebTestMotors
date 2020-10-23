namespace WebTestMotors.DataAccess
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDb;
    using WebTestMotors.DataAccess.Repositories;
    using System;

    public class DataContext : IDataContext
    {
        public DataContext(IRepositoryFactory repoFactory, IMongoDbSettings dbSettings)
        {
            repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
            dbSettings = dbSettings ?? throw new ArgumentNullException(nameof(dbSettings));

            this.Cars = repoFactory.Create<Car>(new RepositoryItemOptions { CollectionName = "Cars", DatabaseName = dbSettings.DatabaseName, ConnectionString = dbSettings.ConnectionString });
        }

        public IRepository<Car> Cars { get; private set; }
    }
}
