using System.Text.Json.Serialization;

namespace LobbyReveal.Core.Models.RiotGames
{
    public class UserInLobby
    {
        public string Username { get; set; }
        public Role Role { get; set; }
        public string Puuid { get; set; }
        public string RiotId { get; set; }
        public string Tag { get; set; }
    }

    public enum Role
    {
        Top,
        Jungle,
        Mid,
        Bottom,
        Support
    }

    public class Participant
    {
        [JsonPropertyName("activePlatform")]
        public object ActivePlatform { get; set; }
        [JsonPropertyName("cid")]
        public string Cid { get; set; }
        [JsonPropertyName("game_name")]
        public string GameName { get; set; }
        [JsonPropertyName("game_tag")]
        public string GameTag { get; set; }
        [JsonPropertyName("muted")]
        public bool Muted { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("pid")]
        public string Pid { get; set; }
        [JsonPropertyName("puuid")]
        public string Puuid { get; set; }
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }

    public class Participants
    {
        [JsonPropertyName("participants")]
        public List<Participant>? Users { get; set; }
    }
}