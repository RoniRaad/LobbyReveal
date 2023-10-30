using LobbyReveal.Core.Models.RiotGames;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Timers;

namespace LobbyReveal.Blazor.Pages
{
    public partial class LobbyRevealPage
    {
        private List<PorofessorTileContract>? tiles;
        private List<UserInLobby>? users;
        private readonly System.Timers.Timer timer;
        SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public LobbyRevealPage()
        {
            timer = new System.Timers.Timer(30000);
            timer.Elapsed += TryConnectToWebsocket;
            timer.AutoReset = true;
            timer.Start();
        }

        public void Dispose()
        {
            timer?.Stop();
            timer?.Dispose();
            WebSocketService.OnMessageReceived -= WebsocketMessageReceived;
        }

        private void TryConnectToWebsocket(object? sender, ElapsedEventArgs e)
        {
            if (!WebSocketService.IsConnected)
                _ = WebSocketService.ConnectToLeagueClientAsync();
        }

        private async Task FetchData()
        {
            await _semaphoreSlim.WaitAsync();

            await InvokeAsync(async () =>
            {
                users = await _lcuClient.GetLocalLobbyUsers();
                if (users is null)
                    return;

                var poroTiles = await _porofessor.GetPorofessorTiles(users.Select(x => x.Username).ToList());

                if (poroTiles is null) return;
                tiles = poroTiles.Select(x => new PorofessorTileContract()
                {
                    StylesheetUri = x.StylesheetUri,
                    Html = new MarkupString(x.Html),
                    Username = x.Username,
                    Role = users?.FirstOrDefault(y => y.Username == x.Username)?.Role ?? Role.Top
                }).ToList();

                foreach (var tile in tiles)
                {
                    await AddContentToIFrame(tile.Username, tile.Html.ToString(), tile.StylesheetUri);
                }

                if (tiles is null) return;

                _semaphoreSlim.Release();
                StateHasChanged();
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            users = await _lcuClient.GetLocalLobbyUsers();

            _ = Task.Run(async () =>
            {
                try
                {
                    await FetchData();
                }
                catch
                {

                }
            });

            try
            {
                await WebSocketService.ConnectToLeagueClientAsync();
            }
            catch
            {

            }
        }

        private async Task AddContentToIFrame(string username, string htmlContent, List<string> stylesheetUri)
        {
            var headElements = new StringBuilder();

            foreach (var stylesheet in stylesheetUri)
            {
                headElements.Append($"<link rel=\"stylesheet\" type=\"text/css\" href=\"{stylesheet}\">");
            }

            var iframeContent = $@"
            <html>
                <head>
                    {headElements}
                </head>
                <body>
                    {htmlContent.Trim('\"')}
                </body>
            </html>
            ";

            try
            {
                await _jsRuntime.InvokeVoidAsync("addToIframe", (new List<string> { $"iframe_{username}", iframeContent.Trim('\"') }).ToArray());
            }
            catch
            {
                
            }
        }

        private void WebsocketMessageReceived(string message)
        {
            var eventData = ParseMessage<RiotGameflowWebsocketEventData>(message, 8, "OnJsonApiEvent_lol-gameflow_v1_gameflow-phase");
            if (eventData?.Data == "ChampSelect" && eventData?.EventType == "Update")
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await FetchData();
                    }
                    catch
                    {

                    }
                });
            }
            else
            {
                tiles = null;
            }
            
            StateHasChanged(); 
        }

        private T? ParseMessage<T>(string message, int type, string eventType)
        {
            if (string.IsNullOrEmpty(message))
                return default;

            var parsedArray = JsonConvert.DeserializeObject<object[]>(message);

            var messageType = (Int64)parsedArray[0];
            var messageEventType = (string)parsedArray[1];
            var messageEventData = JsonConvert.DeserializeObject<T>(parsedArray[2]?.ToString() ?? "");

            if (type != messageType || eventType != messageEventType)
            {
                return default;
            }

            return messageEventData;
        }

        protected override void OnInitialized()
        {
            WebSocketService.OnMessageReceived += WebsocketMessageReceived;
            base.OnInitialized();
        }

        public class RiotGameflowWebsocketEventData
        {
            [JsonPropertyName("data")]
            public string Data { get; set; }

            [JsonPropertyName("eventType")]
            public string EventType { get; set; }

            [JsonPropertyName("uri")]
            public string Uri { get; set; }
        }
    }
}

