using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using EmbyStat.Clients.WebSocketClient;
using EmbyStat.Common.Enums;
using EmbyStat.Services.Interfaces;
using EmbyStat.Sockets.EmbyClient;
using Microsoft.Extensions.Hosting;

namespace EmbyStat.Services
{
    public class WebSocketService : IWebSocketService, IHostedService
    {
        private System.Timers.Timer _checkForEmbyCredentialsTimer;
        private readonly IEmbySocketClient _webSocketClient;
        private readonly IConfigurationService _configurationService;

        public WebSocketService(IEmbySocketClient webSocketClient, IConfigurationService configurationService)
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
                _checkForEmbyCredentialsTimer = new System.Timers.Timer(5000);
                _checkForEmbyCredentialsTimer.Elapsed += CheckForEmbyCredentialsTimer_Elapsed;
                _checkForEmbyCredentialsTimer.AutoReset = true;
                _checkForEmbyCredentialsTimer.Start();
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
                _checkForEmbyCredentialsTimer.Stop();
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
