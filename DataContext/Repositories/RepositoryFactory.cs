using System;
using System.Collections.Generic;
using System.Text;
using WebTestMotors.DataAccess.MongoDb;

namespace WebTestMotors.DataAccess.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IMongoDatabaseFactory _dbFactory;
        public RepositoryFactory(IMongoDatabaseFactory dbFactory)
        {
            this._dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        }
        public IRepository<TEntity> Create<TEntity>(RepositoryItemOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            var db = this._dbFactory.Connect(options.ConnectionString, options.DatabaseName);

            return new Repository<TEntity>(db.GetCollection<TEntity>(options.CollectionName));
        }
    }
}
