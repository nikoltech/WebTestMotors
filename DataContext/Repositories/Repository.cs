namespace WebTestMotors.DataAccess.Repositories
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly IMongoCollection<TEntity> collection;

        public Repository(IMongoCollection<TEntity> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public string CollectionName => this.collection.CollectionNamespace.CollectionName;

        public async Task<long> CountAsync(FilterDefinition<TEntity> filter)
        {
            return await this.collection.CountDocumentsAsync(filter);
        }

        public List<TEntity> Find(FilterDefinition<TEntity> filter)
        {
            return this.collection.Find(filter).ToList();
        }

        public List<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            return this.collection.Find(filter).ToList();
        }

        public async Task<List<TEntity>> FindAsync(FilterDefinition<TEntity> filter)
        {
            return await this.collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<TEntity> FindOneAndReplaceAsync(FilterDefinition<TEntity> filter, TEntity replacement)
        {
            return await this.collection.FindOneAndReplaceAsync(filter, replacement);
        }

        public async Task<TEntity> FindOneAndReplaceAsync(Expression<Func<TEntity, bool>> filter, TEntity replacement)
        {
            return await this.collection.FindOneAndReplaceAsync(filter, replacement);
        }

        public async Task InsertOneAsync(TEntity entity)
        {
            await this.collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> list)
        {
            await this.collection.InsertManyAsync(list);
        }

        public async Task<DeleteResult> DeleteOneAsync(FilterDefinition<TEntity> filter)
        {
            return await this.collection.DeleteOneAsync(filter);
        }

        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.collection.DeleteOneAsync(filter);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter)
        {
            return await this.collection.DeleteManyAsync(filter);
        }

        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this.collection.DeleteManyAsync(filter);
        }
    }
}
