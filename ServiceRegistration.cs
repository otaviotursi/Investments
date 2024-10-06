using System.Reflection;
using FluentValidation;
using Infrastructure.Cache;
using Infrastructure.Repository;
using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using Investments.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Products.Command;
using Products.Command.Handler;
using Products.Repository;
using Products.Repository.Interface;
using Products.Service.Kafka;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;

namespace Investments
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services, configuration);
            AddDependencies(services, configuration);
            AddRedisCache(services, configuration);
            AddMongoDB(services, configuration);
            AddMediatR(services, configuration);
            AddServices(services, configuration);
        }

        private static void AddDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRequestHandler<CreateProductCommand, string>, CreateProductCommandHandler>();

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
            // Carregar as configurações do MongoSettings
            services.Configure<MongoSettings>(configuration.GetSection("ConnectionStrings"));

            // Registrar o cliente MongoDB como singleton
            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
                return new MongoClient(mongoSettings.ConnectionString);
            });

            // Alterar os repositórios para serem Scoped
            services.AddScoped<IReadProductRepository>(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

                return new ReadProductRepository(
                    mongoClient,
                    mongoSettings.DatabaseName,
                    mongoSettings.ProductsReadCollectionName
                );
            });

            services.AddScoped<IWriteProductRepository>(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

                return new WriteProductRepository(
                    mongoClient,
                    mongoSettings.DatabaseName,
                    mongoSettings.ProductsWriteCollectionName
                );
            });


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
