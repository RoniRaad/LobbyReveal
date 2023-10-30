using LobbyReveal.Core.Interfaces;
using LobbyReveal.Core.Services;
using LobbyReveal.Infrastructure.Clients;
using LobbyReveal.UI.Services;
using Blazorise;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using LobbyReveal.Infrastructure.Services.Token;
using System.Net.Http;
using LobbyReveal.Infrastructure.Services.WebSocket;

namespace LobbyReveal.UI
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddWpfBlazorWebView();
            services.AddOptions();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddFile(o => o.RootPath = AppContext.BaseDirectory);
            });
            services.AddMemoryCache();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddLogging();
            services.AddSingleton<IRiotTokenService, RiotTokenService>();
            services.AddSingleton<IGetPorofessorTileForUser, GetPorofessorTileForUser>();
            services.AddSingleton<ILCUClient, LCUClient>();
            services.AddLazyCache();
            services.AddHttpClient();
            services.AddHttpClient("SSLBypass").ConfigureHttpMessageHandlerBuilder(x =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                x.PrimaryHandler = httpClientHandler;
            });
            services.AddSingleton<ILeagueWebSocketService, LeagueWebSocketService>();

            services.AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();
            services.AddBlazorWebViewDeveloperTools();
        }
    }
}
