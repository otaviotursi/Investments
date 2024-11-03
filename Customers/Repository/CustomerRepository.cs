using Customers.Command;
using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Customers.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<CustomerDB> _eventCollection;
        public CustomerRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<CustomerDB>(collectionName);
        }

        public async Task DeleteAsync(ulong id, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDB>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<List<CustomerDB>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<CustomerDB>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<CustomerDB> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDB>.Filter.Empty;
            if (user != null) { 
                filter = Builders<CustomerDB>.Filter.Eq(x => x.User, user);
            }
            else if (fullName != null)
            {
                filter = Builders<CustomerDB>.Filter.Eq(x => x.FullName, fullName);
            }
            else if (id != null)
            {
                filter = Builders<CustomerDB>.Filter.Eq(x => x.Id, id);
            }

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }



        public async Task InsertAsync(CustomerDB customer, CancellationToken cancellationToken)
        {
            ulong id = GetDocumentWithMaxId().Result + 1 ;
            customer.Id = id;
            await _eventCollection.InsertOneAsync(customer);
        }

        public async Task UpdateAsync(CustomerDB customer, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDB>.Filter.
                                     Where(x => x.Id.Equals(customer.Id));
            var update = Builders<CustomerDB>.Update
                .Set(x => x.FullName, customer.FullName)
                .Set(x => x.User, customer.User);

            await _eventCollection.UpdateOneAsync(filter, update, null, cancellationToken);

        }

        public async Task<ulong> GetDocumentWithMaxId()
        {

            var documentWithMaxId = await _eventCollection
                .Find(new BsonDocument())
                .Sort(Builders<CustomerDB>.Sort.Descending("id"))
                .Limit(1)
                .FirstOrDefaultAsync();

            return documentWithMaxId.Id;
        }

    }
}
