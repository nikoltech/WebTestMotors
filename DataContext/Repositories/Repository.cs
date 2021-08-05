namespace WebTestMotors.DataAccess.Repositories
{
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class Repository<TEntity> : IRepository<TEntity>
    {
        public Repository(IMongoCollection<TEntity> entityCollection)
        {
            this.Collection = entityCollection ?? throw new ArgumentNullException(nameof(entityCollection));
        }

        public IMongoCollection<TEntity> Collection { get; private set; }

        public string CollectionName => this.Collection.CollectionNamespace.CollectionName;

        public IMongoQueryable<TEntity> AsQueryable()
        {
            return this.Collection.AsQueryable<TEntity>();
        }

        public async Task<long> CountAsync(FilterDefinition<TEntity> filter)
        {
            return await this.Collection.CountDocumentsAsync(filter);
        }

        public List<TEntity> Find(FilterDefinition<TEntity> filter)
        {
            return this.Collection.Find(filter).ToList();
        }

        public List<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            return this.Collection.Find(filter).ToList();
        }

        public async Task<List<TEntity>> FindAsync(FilterDefinition<TEntity> filter)
        {
            return await this.Collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.Collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<TEntity> FindOneAndReplaceAsync(FilterDefinition<TEntity> filter, TEntity replacement)
        {
            return await this.Collection.FindOneAndReplaceAsync(filter, replacement);
        }

        public async Task<TEntity> FindOneAndReplaceAsync(Expression<Func<TEntity, bool>> filter, TEntity replacement)
        {
            return await this.Collection.FindOneAndReplaceAsync(filter, replacement);
        }

        public async Task InsertOneAsync(TEntity entity)
        {
            await this.Collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> list)
        {
            await this.Collection.InsertManyAsync(list);
        }

        public async Task<DeleteResult> DeleteOneAsync(FilterDefinition<TEntity> filter)
        {
            return await this.Collection.DeleteOneAsync(filter);
        }

        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.Collection.DeleteOneAsync(filter);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter)
        {
            return await this.Collection.DeleteManyAsync(filter);
        }

        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.Collection.DeleteManyAsync(filter);
        }
    }
}
