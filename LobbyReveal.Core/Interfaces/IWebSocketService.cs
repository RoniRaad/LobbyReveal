namespace LobbyReveal.Infrastructure.Services.WebSocket
{
    public interface ILeagueWebSocketService
    {
        bool IsConnected { get; }

        event Action<string> OnMessageReceived;

        Task ConnectToLeagueClientAsync();
        Task DisconnectAsync();
    }
}