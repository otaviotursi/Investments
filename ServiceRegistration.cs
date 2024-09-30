using System.Reflection;
using FluentValidation;
using InvestmentPortfolio.Services;
using Investments.Infrastructure.Cache;
using Investments.Infrastructure.Kafka;
using Investments.Infrastructure.Repository;
using Investments.Infrastructure.Services;
using MongoDB.Driver;

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


        }

        private static void AddServices(IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<KafkaConfig>(configuration.GetSection("Kafka"));
            services.AddHostedService<KafkaConsumerService>();
            services.AddScoped<IKafkaProducerService, KafkaPublisherService>();
        }

        private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
        {
            // Registrar o MongoClient
            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("DefaultConnection")));

            //services.AddSingleton<IPrivateProductReadRepository>(sp =>
            //    new PrivateProductReadRepository(
            //        sp.GetRequiredService<IMongoClient>(),
            //        configuration.GetConnectionString("DefaultDatabase"),
            //        configuration.GetConnectionString("ProductsReadCollectionName")
            //    ));


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
