namespace LobbyReveal.Infrastructure.Services
{
    public interface IAppUpdateService
    {
        Task<bool> CheckForUpdate();
        Task UpdateAndRestart();
        Task Restart();
    }
}