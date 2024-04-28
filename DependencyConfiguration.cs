using MageWin.Interfaces;
using MageWin.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace MageWin
{
    public static class DependencyConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IGoogleServices, GoogleServices>();
            services.AddSingleton<IBaseClientService, BaseClientService>();

        }
    }
}
