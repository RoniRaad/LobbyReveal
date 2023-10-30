using LobbyReveal.Core.Models.RiotGames;
using Microsoft.AspNetCore.Components;

namespace LobbyReveal.Blazor.Pages
{
    public partial class LobbyRevealPage
    {
        public class PorofessorTileContract 
        {
            public Role Role { get; set; }
            public MarkupString Html { get; set; }
            public string Username { get; set; }
            public List<string> StylesheetUri { get; set; }
        }

    }
}

