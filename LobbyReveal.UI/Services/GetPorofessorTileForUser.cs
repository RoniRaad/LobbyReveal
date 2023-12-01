using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LobbyReveal.Core.Interfaces;
using LobbyReveal.Core.Models.RiotGames;
using LobbyReveal.UI.Windows;
using Microsoft.Web.WebView2.Core;

namespace LobbyReveal.UI.Services
{
    public partial class GetPorofessorTileForUser : IGetPorofessorTileForUser
    {
        private WebView2Browser? WebView { get; set; }
        public event Action<PorofessorTilesRecievedEventArgs> OnPorofessorTilesRecieved = delegate { };
        SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        public async Task<List<PorofessorTile>?> GetPorofessorTiles(List<string> usernames)
        {
            await _semaphoreSlim.WaitAsync();
            List<PorofessorTile>? tiles = null;

            try
            {
                var startTime = DateTimeOffset.Now;
                if (WebView is null)
                {
                    WebView = new WebView2Browser
                    {
                        Title = "Porofessor Browser"
                    };
                    WebView.Show();
                }

				WebView.webv2.Source = new Uri($"https://porofessor.gg/pregame/na/{string.Join(',', usernames)}");

				WebView.webv2.CoreWebView2InitializationCompleted += (_, e) =>
                {
					var cookie = WebView.webv2.CoreWebView2.CookieManager.CreateCookie("darkMode", "1", "porofessor.gg", "/");
					WebView.webv2.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);


					WebView.webv2.CoreWebView2.WebResourceResponseReceived += async (e, v) =>
                    {
                        if (v?.Request?.Uri?.Contains("leagueofgraphs") is true)
                        {
                            tiles = await GetTiles(usernames);
                        }
                    };
                };

                while (tiles is null && startTime.AddMinutes(2) > DateTimeOffset.Now)
                    await Task.Delay(300);

                CloseBrowser();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return tiles;
        }

        public void CloseBrowser()
        {
            WebView?.webv2?.Dispose();
            WebView?.Close();
            WebView = null;
        }

        private async Task<List<PorofessorTile>> GetTiles(List<string> usernames)
        {
            if (WebView is null)
                return new();


            var usernameAndTileHtml = new List<PorofessorTile>();
            string scriptToGetUrls = @"Array.from(document.styleSheets).map(ss => ss.href);";
            string jsonUrls = await WebView.webv2.CoreWebView2.ExecuteScriptAsync(scriptToGetUrls);
            List<string> styleSheetUrls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jsonUrls);

            foreach (var username in usernames)
            {
                var tile = await (WebView?.webv2?.CoreWebView2?.ExecuteScriptAsync($"document.querySelector(`div[data-summonername='{username.Replace('-', '#')}']`).outerHTML") ?? Task.FromResult(""));
                usernameAndTileHtml.Add(new()
                {
                    Html = Regex.Unescape(tile),
                    StylesheetUri = styleSheetUrls,
                    Username = username,
                });
            }

            return usernameAndTileHtml ?? new();
        }
    }
}