using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetbook.Domain;

namespace Tweetbook.Installers
{
    public class CosmosInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var cosmosStoreSettings = new CosmosStoreSettings(configuration["CosmosSettings:DatabaseName"], configuration["CosmosSettings:AccountUri"], configuration["CosmosSettings:AccountKey"], new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });

            services.AddCosmosStore<Domain.Post>(cosmosStoreSettings); // similar to EF DbSet

        }
    }
}
