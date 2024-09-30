﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Investments.Infrastructure.Repository
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            // Recupera a string de conexão e o nome do banco de dados do appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseName = configuration.GetConnectionString("DefaultDatabase");

            // Configura a conexão com o MongoDB
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Definição das coleções (equivalente ao DbSet do Entity Framework)
        public IMongoCollection<ProductDTO> ProductsRead => _database.GetCollection<ProductDTO>("ProductsRead");
        public IMongoCollection<ProductDTO> ProductsWrite => _database.GetCollection<ProductDTO>("ProductsWrite");

    }
}
