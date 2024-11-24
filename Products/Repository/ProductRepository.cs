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
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<ProductDB> _eventCollection;
        public ProductRepository(IMongoClient mongoClient, string databaseName, string collectionName)
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
            var filter = Builders<ProductDB>.Filter.Eq(x => x.Id, productDB.Id);
            var updateDefinitionBuilder = Builders<ProductDB>.Update;
            var updateDefinitions = new List<UpdateDefinition<ProductDB>>();

            // Verifique cada campo e adicione ao UpdateDefinition se não for null
            if (productDB?.Name != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.Name, productDB.Name));

            if (productDB?.UnitPrice != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.UnitPrice, productDB.UnitPrice));

            if (productDB?.ProductType != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.ProductType, productDB.ProductType));

            if (productDB?.AvailableQuantity != null)
                updateDefinitions.Add(updateDefinitionBuilder.Set(x => x.AvailableQuantity, productDB.AvailableQuantity));

            // Combine todas as definições de atualização em um único UpdateDefinition
            var update = updateDefinitionBuilder.Combine(updateDefinitions);

            // Execute a atualização somente se houver algo a atualizar
            if (updateDefinitions.Any())
            {
                await _eventCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            }

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
