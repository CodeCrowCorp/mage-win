using AutoMapper;
using MageWin.Interfaces;
using MageWin.Models;
using MageWin.Models.Api.ChannelResponse;
using MageWin.Models.Api.YoutubeChatResponse;
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
            services.AddSingleton<IMageServices, MageServices>();
            services.AddSingleton<IBaseClientService, BaseClientService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChannelResponse, ChannelModel>();
                cfg.CreateMap<YoutubeMessage, YoutubeModel>();
                cfg.CreateMap<User, UserModel>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton<IMapper>(mapper);

        }
    }
}
