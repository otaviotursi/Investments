using System.Reflection;
using Customers.Repository;
using Customers.Repository.Interface;
using FluentValidation;
using Infrastructure.Cache;
using Infrastructure.Repository;
using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using Investments.Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Products.Command;
using Products.Command.Handler;
using Products.Repository;
using Products.Repository.Interface;
using Products.Service.Kafka;
using Users.Repository;
using Users.Repository.Interface;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;

namespace Investments
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDependencies(services, configuration);
            AddRedisCache(services, configuration);
            AddMongoDB(services, configuration);
            AddMediatR(services, configuration);
            AddRepositories(services, configuration);
            AddServices(services, configuration);
        }

        private static void AddDependencies(IServiceCollection services, IConfiguration configuration)
        {

            //services.AddTransient<IRequestHandler<InsertProductCommand, string>, InsertProductCommandHandler>();

            //services.AddTransient<INotificationHandler<CreateProductEvent>, CreateProductEventHandler>();

            // Registrar o MongoClient

        }

        private static void AddServices(IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<KafkaConfig>(configuration.GetSection("Kafka"));
            services.AddHostedService<ProductKafkaConsumerService>();
            services.AddScoped<IKafkaProducerService, KafkaPublisherService>();
        }

        private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
        {
            // Registrar o MongoClient
            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("DefaultConnection")));



            // Registrar o ProductReadRepository com os valores diretamente
            services.AddSingleton<IReadProductRepository>(sp =>
                new ReadProductRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("ProductsReadCollectionName")  // Nome correto da coleção
                ));

            // Registrar o ProductWriteRepository com os valores diretamente
            services.AddScoped<IWriteProductRepository>(sp =>
                new WriteProductRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("ProductsWriteCollectionName")  // Nome correto da coleção
                ));


            // Registrar o CustomerRepository com os valores diretamente
            services.AddScoped<ICustomerRepository>(sp =>
                new CustomerRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("CustomerCollectionName")  // Nome correto da coleção
                ));

            // Registrar o UserRepository com os valores diretamente
            services.AddScoped<IUserRepository>(sp =>
                new UserRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("UserCollectionName")  // Nome correto da coleção
                ));
        }

        private static void AddMediatR(IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));


        }
        private static void AddMongoDB(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDb");
            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
            services.AddSingleton<MongoDbContext>();
        }

        private static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.InstanceName = configuration.GetSection("Regis:InstanceName")?.Value;
                opt.Configuration = configuration.GetSection("Regis:Ip")?.Value;
            });
            services.AddScoped<IRedisCacheHelper, RedisCacheHelper>();
        }
    }
}
