using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ToDOFunctionApp.Services;
using Azure.Identity;
using System;

[assembly: FunctionsStartup(typeof(ToDOFunctionApp.Startup))]
namespace ToDOFunctionApp
{
    public class Startup : FunctionsStartup
    {
        //public IConfiguration Configuration { get; }
        //public Startup(IConfiguration configuration)
        //{
        //    //Configuration = configuration;
        //}
        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfiguration configurationSection)
        {
            string databaseName = configurationSection["CosmosDb.DatabaseName"];
            string containerName = configurationSection["CosmosDb.ContainerName"];
            string account = configurationSection["CosmosDb.Account"];
            string key = configurationSection["CosmosDb.Key"];

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            CosmosClient client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {

            //builder.Con
            //builder.Services.AddHttpClient();

            //throw new System.NotImplementedException();
         
          

            var config = builder.GetContext().Configuration;

            var _builder = new ConfigurationBuilder();

            //_builder.AddAzureAppConfiguration(config["AppConfig:Endpoint"]);
            //_builder.AddAzureAppConfiguration(options => {
            //    options.Connect(new Uri(config["AppConfig:Endpoint"]), new ManagedIdentityCredential());
            //});
            _builder.AddAzureAppConfiguration(options => {
                options.Connect(new Uri(config["AppConfig:Endpoint"]), new DefaultAzureCredential());
            });
            var config2 = _builder.Build();
            var cdb = InitializeCosmosClientInstanceAsync(config2).Result;
            builder.Services.AddSingleton<CosmosDbService>((s) => cdb);
            //builder.Services.AddSingleton<ICosmosDbService>((ICosmosDbService)InitializeCosmosClientInstanceAsync(config2).GetAwaiter().GetResult());
        }


    }
}
