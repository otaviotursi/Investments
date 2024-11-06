using Infrastructure.Repository.Entities;
using MongoDB.Driver;
using Portfolio.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<PortfolioDB> _eventCollection;
        public PortfolioRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<PortfolioDB>(collectionName);
        }

        public Task<List<PortfolioDB>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<PortfolioDB>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public Task<PortfolioDB> GetByName(ulong customerId, CancellationToken cancellationToken)
        {
            var filter = Builders<PortfolioDB>.Filter.Eq(x => x.CustomerId, customerId);

            var result = _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public async Task InsertAsync(PortfolioDB product, CancellationToken cancellationToken)
        {
            var filter = Builders<PortfolioDB>.Filter.Eq(x => x.CustomerId, product.CustomerId) &
                         Builders<PortfolioDB>.Filter.Eq(x => x.ProductId, product.ProductId);

            var update = Builders<PortfolioDB>.Update.Inc(x => x.AmountNegotiated, product.AmountNegotiated);

            var options = new FindOneAndUpdateOptions<PortfolioDB>
            {
                IsUpsert = true
            };

            await _eventCollection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);

        }

        public async Task RemoveAsync(PortfolioDB product, CancellationToken cancellationToken)
        {
            var filter = Builders<PortfolioDB>.Filter.Eq(x => x.CustomerId, product.CustomerId) &
                         Builders<PortfolioDB>.Filter.Eq(x => x.ProductId, product.ProductId);

            // Tenta decrementar o AmountNegotiated
            var update = Builders<PortfolioDB>.Update.Inc(x => x.AmountNegotiated, (-product.AmountNegotiated));

            var result = await _eventCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<PortfolioDB>
            {
                ReturnDocument = ReturnDocument.After
            }, cancellationToken);

            // Se o AmountNegotiated for menor ou igual a zero, exclui o registro
            if (result != null && result.AmountNegotiated <= 0)
            {
                await _eventCollection.DeleteOneAsync(filter, cancellationToken);
            }
        }
    }
}
