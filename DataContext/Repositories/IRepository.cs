﻿namespace WebTestMotors.DataAccess.Repositories
{
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<TEntity>
    {
        string CollectionName { get; }

        IMongoCollection<TEntity> Collection { get; }

        IMongoQueryable<TEntity> AsQueryable();

        Task<long> CountAsync(FilterDefinition<TEntity> filter);

        List<TEntity> Find(FilterDefinition<TEntity> filter);

        List<TEntity> Find(Expression<Func<TEntity, bool>> filter);

        Task<List<TEntity>> FindAsync(FilterDefinition<TEntity> filter);

        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> FindOneAndReplaceAsync(FilterDefinition<TEntity> filter, TEntity replacement);
        
        Task<TEntity> FindOneAndReplaceAsync(Expression<Func<TEntity, bool>> filter, TEntity replacement);

        Task InsertOneAsync(TEntity entity);

        Task InsertManyAsync(IEnumerable<TEntity> list);

        Task<DeleteResult> DeleteOneAsync(FilterDefinition<TEntity> filter);

        Task<DeleteResult> DeleteOneAsync(Expression<Func<TEntity, bool>> filter);

        Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter);

        Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter);
    }
}
