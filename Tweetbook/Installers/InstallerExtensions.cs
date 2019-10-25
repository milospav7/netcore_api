using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetbook.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration Configuration)
        {
            var installers = typeof(Startup).Assembly
                .ExportedTypes
                .Where(t => typeof(IInstaller).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract) // every class implementing IInstaller
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>() // cast those implementers to interface
                .ToList();

            installers.ForEach(i => i.InstallService(services, Configuration));
        }

    }
}
