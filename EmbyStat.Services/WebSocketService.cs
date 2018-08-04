using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using EmbyStat.Api.WebSocketClient;
using EmbyStat.Common.Enums;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Timer = System.Threading.Timer;

namespace EmbyStat.Services
{
    public class WebSocketService : IWebSocketService, IHostedService
    {
        private System.Timers.Timer checkForEmbyCredentialsTimer;
        private readonly IWebSocketClient _webSocketClient;
        private readonly IConfigurationService _configurationService;

        public WebSocketService(IWebSocketClient webSocketClient, IConfigurationService configurationService)
        {
            _webSocketClient = webSocketClient;
            _configurationService = configurationService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {       
            if (IsEmbyConnected())
            {
                await OpenWebSocketConnection(cancellationToken);
            }
            else
            {
                checkForEmbyCredentialsTimer = new System.Timers.Timer(5000);
                checkForEmbyCredentialsTimer.Elapsed += CheckForEmbyCredentialsTimer_Elapsed;
                checkForEmbyCredentialsTimer.AutoReset = true;
                checkForEmbyCredentialsTimer.Start();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _webSocketClient.Close(cancellationToken);
        }

        private bool IsEmbyConnected()
        {
            var settings = _configurationService.GetServerSettings();
            return settings.AccessToken != "";
        }

        private async void CheckForEmbyCredentialsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsEmbyConnected())
            {
                checkForEmbyCredentialsTimer.Stop();
                await OpenWebSocketConnection(new CancellationToken(false));
            }
        }

        private async Task OpenWebSocketConnection(CancellationToken cancellationToken)
        {
            var settings = _configurationService.GetServerSettings();
            var protocol = settings.EmbyServerProtocol == ConnectionProtocol.Http ? "ws" : "wss";
            var socketUrl = $"{protocol}://{settings.EmbyServerAddress}:{settings.EmbyServerPort}?api_key={settings.AccessToken}&deviceId={Guid.NewGuid().ToString()}";

            await _webSocketClient.Connect(socketUrl, cancellationToken);
        }

        public bool IsWebSocketConnectionOpen()
        {
            return _webSocketClient.IsWebSocketConnectionOpen();
        }
    }
}
