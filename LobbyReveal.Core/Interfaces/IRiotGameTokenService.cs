namespace LobbyReveal.Core.Interfaces
{
    public interface IRiotGameTokenService
    {
        string GetLeagueCommandlineParams();
        bool TryGetPortAndToken(out string token, out string port);
    }
}