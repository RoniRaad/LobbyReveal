using LobbyReveal.Core.Models;
using LobbyReveal.Core.Services;

namespace LobbyReveal.Core.Interfaces
{
    public interface IAlertService
    {
        event Action Notify;

        void AddErrorAlert(string errorMessage);
        void AddInfoAlert(string infoMessage);
        IEnumerable<Alert> GetErrorAlerts();
        IEnumerable<Alert> GetInfoAlerts();
        void RemoveErrorMessage(Alert errorMessage);
        void RemoveInfoMessage(Alert infoMessage);
    }
}