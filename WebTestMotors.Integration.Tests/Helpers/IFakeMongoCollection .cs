using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebTestMotors.Integration.Tests.Helpers
{
    public interface IFakeMongoCollection : IMongoCollection<BsonDocument>
    {
        IFindFluent<BsonDocument, BsonDocument> Find(FilterDefinition<BsonDocument> filter, FindOptions options);

        IFindFluent<BsonDocument, BsonDocument> Project(ProjectionDefinition<BsonDocument, BsonDocument> projection);

        IFindFluent<BsonDocument, BsonDocument> Skip(int skip);

        IFindFluent<BsonDocument, BsonDocument> Limit(int limit);

        IFindFluent<BsonDocument, BsonDocument> Sort(SortDefinition<BsonDocument> sort);
    }
}
