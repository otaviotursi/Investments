using Infrastructure.Enum;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository
{
    public class WriteProductRepository
    : IWriteProductRepository
    {

        private readonly IMongoCollection<BsonDocument> _eventCollection;

        public WriteProductRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<BsonDocument>(collectionName);

        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            ProductDB productDB = new(id);

            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", productDB.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Delete.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }

        public async Task InsertAsync(ProductDB productDB, CancellationToken cancellationToken)
        {
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", productDB.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Insert.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }

        public async Task UpdateAsync(ProductDB productDB, CancellationToken cancellationToken)
        {
            var eventDocument = new BsonDocument
            {
                {"AggregateId", Guid.NewGuid().ToString()},
                {"Data", productDB.ToBsonDocument()},
                {"TypeOperation", TypeOperationEnum.Update.Name},
                {"TimeStamp", DateTime.UtcNow},
            };

            await _eventCollection.InsertOneAsync(eventDocument);
        }
    }
}
