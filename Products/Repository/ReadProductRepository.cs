using Infrastructure.Repository.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository
{
    public class ReadProductRepository : IReadProductRepository
    {
        private readonly IMongoCollection<ProductDB> _eventCollection;
        public ReadProductRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<ProductDB>(collectionName);
        }

        public Task<List<ProductDB>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<ProductDB>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDB>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }


        public async Task InsertAsync(ProductDB productDB, CancellationToken cancellationToken)
        {
            var productById = await GetById(productDB.Id, cancellationToken);
            if (productById == null) { 
                await _eventCollection.InsertOneAsync(productDB);
            } else
            {
                await UpdateAsync(productDB, cancellationToken);
            }

            
        }

        public async Task UpdateAsync(ProductDB productDB, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDB>.Filter.
                                     Where(x => x.Id.Equals(productDB.Id));
            var update = Builders<ProductDB>.Update
                .Set(x => x.Name, productDB.Name)
                .Set(x => x.UnitPrice, productDB.UnitPrice)
                .Set(x => x.ProductType, productDB.ProductType)
                .Set(x => x.AvailableQuantity, productDB.AvailableQuantity);

            await _eventCollection.UpdateOneAsync(filter, update, null, cancellationToken);
        }

        public async Task<ProductDB> GetByName(string name, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDB>.Filter.Eq(x => x.Name, name);

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return result;
        }
        public Task<ProductDB?> GetById(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductDB>.Filter.Eq(x => x.Id, id);

            var result = _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public Task<List<ProductDB>> GetExpiritionByDateAll(int expirationDay, CancellationToken cancellationToken)
        {
            DateTime dataAtual = DateTime.Now;
            DateTime dataLimite = dataAtual.AddDays(expirationDay);

            var filter = Builders<ProductDB>.Filter.Gte(x => x.ExpirationDate, dataAtual) &
                         Builders<ProductDB>.Filter.Lte(x => x.ExpirationDate, dataLimite);

            var result = _eventCollection.Find(filter).ToListAsync(cancellationToken);

            return result;
        }
    }
}
