using LobbyReveal.Core.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace LobbyReveal.Infrastructure.Services.WebSocket
{
    public class LeagueWebSocketService : ILeagueWebSocketService
    {
        private readonly IRiotTokenService _riotTokenService;
        private ClientWebSocket _webSocket = new ClientWebSocket();
        public event Action<string> OnMessageReceived = delegate { };
        public bool IsConnected { get { return _webSocket.State == WebSocketState.Open; } }

        public LeagueWebSocketService(IRiotTokenService riotTokenService)
        {
            _riotTokenService = riotTokenService;
        }

        public async Task ConnectToLeagueClientAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
                return;

            _riotTokenService.TryGetLeaguePortAndToken(out var token, out var port);
            _webSocket.Options.SetRequestHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{token}"))}");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var invoker = new HttpMessageInvoker(handler))
            {
                await _webSocket.ConnectAsync(new Uri($"wss://localhost:{port}"), invoker, CancellationToken.None);
            }

            await SendMessageAsync("[5, \"OnJsonApiEvent_lol-gameflow_v1_gameflow-phase\"]");

            _ = ListenForMessages();
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[1024 * 6];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket connection is not open.");

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(messageBuffer);

            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
