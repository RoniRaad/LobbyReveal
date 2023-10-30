using System;
using System.Collections.Generic;

namespace LobbyReveal.Core.Models.RiotGames
{
    public sealed class PorofessorTilesRecievedEventArgs : EventArgs
    {
        public List<PorofessorTile>? UsernameAndPorofessorTile { get; set; }
    }
}
