using LobbyReveal.Core.Interfaces;

namespace LobbyReveal.Core.Services
{
    public sealed class AlertService : IAlertService
    {
        private readonly List<Alert> errorMessages = new();
        private readonly List<Alert> infoMessages = new();
        public event Action Notify = delegate { };
       
        public IEnumerable<Alert> GetErrorAlerts()
        {
            return errorMessages;
        }

        public IEnumerable<Alert> GetInfoAlerts()
        {
            return infoMessages;
        }

        public void AddErrorAlert(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
                return;

            errorMessages.Add(new Alert { DisplayMessage = errorMessage, Type = AlertType.Error } );
            Notify.Invoke();
        }

        public void AddInfoAlert(string infoMessage)
        {
            if (string.IsNullOrEmpty(infoMessage))
                return;


            infoMessages.Add(new Alert { DisplayMessage = infoMessage, Type = AlertType.Info });
            Notify.Invoke();
        }

        public void RemoveErrorMessage(Alert errorMessage)
        {
            errorMessages.Remove(errorMessage);
            Notify.Invoke();
        }

        public void RemoveInfoMessage(Alert infoMessage)
        {
            infoMessages.Remove(infoMessage);
            Notify.Invoke();
        }
    }

    public sealed class Alert
    {
        public AlertType Type { get; set; }
        public string DisplayMessage { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

    public enum AlertType
    {
        Error,
        Info
    }
}
