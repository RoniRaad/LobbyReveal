namespace LobbyReveal.Core.Interfaces
{
    public interface IRiotTokenService
    {
        bool TryGetLeaguePortAndToken(out string token, out string port);
        bool TryGetRiotPortAndToken(out string token, out string port);
    }
}