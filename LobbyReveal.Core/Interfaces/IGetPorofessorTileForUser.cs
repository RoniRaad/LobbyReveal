using LobbyReveal.Core.Models.RiotGames;

namespace LobbyReveal.Core.Interfaces
{
    public interface IGetPorofessorTileForUser
    {
        event Action<PorofessorTilesRecievedEventArgs> OnPorofessorTilesRecieved;

        void CloseBrowser();
        Task<List<PorofessorTile>?> GetPorofessorTiles(List<string> usernames);
    }
}