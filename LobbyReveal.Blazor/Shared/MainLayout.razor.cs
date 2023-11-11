namespace LobbyReveal.Blazor.Shared
{
    public partial class MainLayout
    {
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        private bool updateAvailable = false;
        private bool settingsModalOpen = false;

        protected override async Task OnInitializedAsync()
        {
            _alertService.Notify += () => InvokeAsync(() => StateHasChanged());

            updateAvailable = await _appUpdateService.CheckForUpdate();
        }

        private void Refresh()
        {
            _navManager.NavigateTo("/", true);
        }
    }
}