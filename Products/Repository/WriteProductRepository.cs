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

        public async Task<List<ProductDB>> GetStatementBy(string? name, string? user, DateTime? expirationDate,CancellationToken cancellationToken)
        {
            var filters = new List<FilterDefinition<BsonDocument>>();

            // Verifica se 'name' não é null e adiciona o filtro correspondente
            if (!string.IsNullOrEmpty(name))
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.Name", name));
            }

            // Verifica se 'user' não é null e adiciona o filtro correspondente
            if (!string.IsNullOrEmpty(user))
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.User", user)); // Supondo que o campo seja 'Data.User'
            }

            // Verifica se 'expirationDate' não é null e adiciona o filtro correspondente
            if (expirationDate != null)
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("Data.ExpirationDate", expirationDate));
            }

            // Combina os filtros usando o operador AND se houver mais de um
            var filter = filters.Count > 0 ? Builders<BsonDocument>.Filter.And(filters) : FilterDefinition<BsonDocument>.Empty;

            var documents = _eventCollection.Find(filter).ToList();
            // Convertendo os resultados para ProductDb
            List<ProductDB> products = new List<ProductDB>();
            foreach (var document in documents)
            {
                var product = new ProductDB
                {
                    Id = Guid.Parse(document["Data"]["_id"].AsGuid.ToString()),
                    Name = document["Data"]["Name"].AsString,
                    UnitPrice = Convert.ToDecimal(document["Data"]["UnitPrice"].AsString),
                    ExpirationDate = document["Data"]["ExpirationDate"].ToUniversalTime(),
                    ProductType = document["Data"]["ProductType"].AsString,
                    AvailableQuantity = Convert.ToInt32(document["Data"]["AvailableQuantity"].AsString)
                };

                products.Add(product);
            }

            return products;
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
