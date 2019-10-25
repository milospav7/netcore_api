using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetbook
{
    public interface IInstaller
    {
        void InstallService(IServiceCollection services, IConfiguration Configuration);
    }
}
