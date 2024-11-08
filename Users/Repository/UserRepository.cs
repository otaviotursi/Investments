using Users.Command;
using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDB> _eventCollection;
        public UserRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {

            var database = mongoClient.GetDatabase(databaseName);
            _eventCollection = database.GetCollection<UserDB>(collectionName);
        }

        public async Task DeleteAsync(ulong id, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDB>.Filter.
                                     Where(x => x.Id.Equals(id));

            await _eventCollection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<List<UserDB>> GetAll(CancellationToken cancellationToken)
        {
            return _eventCollection.Find(Builders<UserDB>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<UserDB> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDB>.Filter.Empty;
            if (user != null) { 
                filter = Builders<UserDB>.Filter.Eq(x => x.User, user);
            }
            else if (fullName != null)
            {
                filter = Builders<UserDB>.Filter.Eq(x => x.FullName, fullName);
            }
            else if (id != null)
            {
                filter = Builders<UserDB>.Filter.Eq(x => x.Id, id);
            }

            var result = await _eventCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }



        public async Task InsertAsync(UserDB User, CancellationToken cancellationToken)
        {
            ulong id = GetDocumentWithMaxId().Result + 1 ;
            User.Id = id;
            await _eventCollection.InsertOneAsync(User);
        }

        public async Task UpdateAsync(UserDB User, CancellationToken cancellationToken)
        {
            var filter = Builders<UserDB>.Filter.
                                     Where(x => x.Id.Equals(User.Id));
            var update = Builders<UserDB>.Update
                .Set(x => x.FullName, User.FullName)
                .Set(x => x.User, User.User);

            await _eventCollection.UpdateOneAsync(filter, update, null, cancellationToken);

        }

        public async Task<ulong> GetDocumentWithMaxId()
        {

            var documentWithMaxId = await _eventCollection
                .Find(new BsonDocument())
                .Sort(Builders<UserDB>.Sort.Descending("id"))
                .ToListAsync();

            var lastId = documentWithMaxId.Last().Id;

            return lastId ?? 0;
        }

    }
}
