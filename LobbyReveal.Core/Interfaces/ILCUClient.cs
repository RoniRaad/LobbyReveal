using LobbyReveal.Core.Models.RiotGames;

namespace LobbyReveal.Core.Interfaces
{
    public interface ILCUClient
    {
        Task<List<UserInLobby>?> GetLocalLobbyUsers();
    }
}