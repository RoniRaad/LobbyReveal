using LobbyReveal.Core.Interfaces;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using LobbyReveal.Core.Models.RiotGames;

namespace LobbyReveal.Infrastructure.Clients
{
    public sealed class LCUClient : ILCUClient
    {
        private readonly ILogger<LCUClient> _logger;
        private readonly IRiotTokenService _riotTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<string, Role> roleMap = new Dictionary<string, Role>()
        {
            { "top", Role.Top },
            { "jungle", Role.Jungle },
            { "utility", Role.Support },
            { "bottom", Role.Bottom },
            { "middle", Role.Mid }
        };
        public LCUClient(IHttpClientFactory httpClientFactory,
            ILogger<LCUClient> logger,
            IRiotTokenService riotTokenService)
        {
            _httpClientFactory = httpClientFactory;
            _riotTokenService = riotTokenService;
            _logger = logger;
        }

        public async Task<List<UserInLobby>?> GetLocalLobbyUsers()
        {
            if (!_riotTokenService.TryGetRiotPortAndToken(out string riotToken, out string riotPort))
                return null;

            if (!_riotTokenService.TryGetLeaguePortAndToken(out string leagueToken, out string leaguePort))
                return null;

            var usersInLobby = new List<UserInLobby>();

            var riotClient = _httpClientFactory.CreateClient("SSLBypass");
            riotClient.BaseAddress = new Uri($"https://127.0.0.1:{riotPort}");
            riotClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{riotToken}")));
            riotClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");

            var leagueClient = _httpClientFactory.CreateClient("SSLBypass");
            leagueClient.BaseAddress = new Uri($"https://127.0.0.1:{leaguePort}");
            leagueClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{leagueToken}")));
            leagueClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");
            HttpResponseMessage champSelectChatRequest;
            HttpResponseMessage champSelectSessionRequest;

            try
            {
                champSelectChatRequest = await riotClient.GetAsync("/chat/v5/participants/champ-select");
                champSelectChatRequest.EnsureSuccessStatusCode();

                champSelectSessionRequest = await leagueClient.GetAsync("/lol-champ-select/v1/session");
                champSelectSessionRequest.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Unable to get local league of legends session token! Status Code: {StatusCode}, Message: {Message}", ex.StatusCode, ex.Message);
                return null;
            }

            var participants = await champSelectChatRequest.Content.ReadFromJsonAsync<Participants>();
            int attempts = 0;

            while (participants?.Users?.Count == 1 && attempts < 8)
            {
                await Task.Delay(5000);
                champSelectChatRequest = await riotClient.GetAsync("/chat/v5/participants/champ-select");
                participants = await champSelectChatRequest.Content.ReadFromJsonAsync<Participants>();
                attempts++;
            }

            var champSelectSession = await champSelectSessionRequest.Content.ReadFromJsonAsync<ChampSelectSessionResponse>();

            if (participants is null || champSelectSession is null)
            {
                _logger.LogError("Unable to get local league of legends session token! Token was null!");
                return null;
            }
            var ourCellId = champSelectSession.LocalPlayerCellId;
            var localSummoner = await GetLocalSummoner();
            foreach (var teamMember in champSelectSession.MyTeam)
            {
                if (teamMember.CellId == ourCellId)
                {
                    if (roleMap.TryGetValue(teamMember.AssignedPosition, out Role role))
                        usersInLobby.Add(new()
                        {
                            Username = localSummoner.DisplayName,
                            Role = role,
                            Puuid = localSummoner.Puuid
                        });
                }
                else
                {
                    if (teamMember.SummonerId == 0L)
                        await MutePlayer(0L, "", teamMember.ObfuscatedSummonerId, teamMember.ObfuscatedPuuid);
                    else
                        await MutePlayer(teamMember.SummonerId, teamMember.Puuid, 0L, "");

                    var count = await CountMutedPlayers();
                    for (int i = 0; i < participants.Users.Count; i++)
                    {
                        Participant? participant = participants.Users[i];
                        if (localSummoner.Puuid != participant.Puuid)
                        {
                            var summonerId = await GetSummonerId(participant.Puuid);
                            await MutePlayer(summonerId, participant.Puuid, 0L, "");
                            var newCount = await CountMutedPlayers();
                            if (newCount < count)
                            {
                                if (roleMap.TryGetValue(teamMember.AssignedPosition, out Role role))
                                    usersInLobby.Add(new()
                                    {
                                        Username = participant.Name,
                                        Role = role,
                                        Puuid = participant.Puuid,
                                    });

                                i = participants.Users.Count;
                            }
                            else
                            {
                                await MutePlayer(summonerId, participant.Puuid, 0L, "");
                            }
                        }
                    }
                }
            }

            var summonerNamesRequest = new SummonerNamesRequest()
            {
                Puuids = usersInLobby.Select(x => x.Puuid).ToList()
            };

            var namesetsResponse = await riotClient.PostAsJsonAsync("/player-account/lookup/v1/namesets-for-puuids", summonerNamesRequest);
            var namesets = await namesetsResponse.Content.ReadFromJsonAsync<NamesetsResponse>();

            namesets.Namesets.ForEach(x =>
            {
				usersInLobby.First(y => y.Puuid == x.Puuid).Username = $"{x.Gnt.GameName}-{x.Gnt.TagLine}";

			});

			return usersInLobby;
        }

        public async Task MutePlayer(long id, string puuid, long obfuscatedId, string obfuscatedPuuid)
        {
            if (!_riotTokenService.TryGetLeaguePortAndToken(out string leagueToken, out string leaguePort))
                return;

            var leagueClient = _httpClientFactory.CreateClient("SSLBypass");
            leagueClient.BaseAddress = new Uri($"https://127.0.0.1:{leaguePort}");
            leagueClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{leagueToken}")));
            leagueClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");

            var request = new MutePlayerRequest()
            {
                Id = id,
                Puuid = puuid,
                ObfuscatedPuuid = obfuscatedPuuid,
                ObfuscatedId = obfuscatedId
            };

            var postRequest = await leagueClient.PostAsJsonAsync("/lol-champ-select/v1/toggle-player-muted", request);
            postRequest.EnsureSuccessStatusCode();
        }
        
        public async Task<int> CountMutedPlayers()
        {
            if (!_riotTokenService.TryGetLeaguePortAndToken(out string leagueToken, out string leaguePort))
                return 0;

            var leagueClient = _httpClientFactory.CreateClient("SSLBypass");
            leagueClient.BaseAddress = new Uri($"https://127.0.0.1:{leaguePort}");
            leagueClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{leagueToken}")));
            leagueClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");

            var mutedPlayers = await leagueClient.GetFromJsonAsync<List<object>>("/lol-champ-select/v1/muted-players");

            return mutedPlayers.Count;
        }

        public async Task<long> GetSummonerId(string puuid)
        {
            if (!_riotTokenService.TryGetLeaguePortAndToken(out string leagueToken, out string leaguePort))
                return 0L;

            var leagueClient = _httpClientFactory.CreateClient("SSLBypass");
            leagueClient.BaseAddress = new Uri($"https://127.0.0.1:{leaguePort}");
            leagueClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{leagueToken}")));
            leagueClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");

            var summoner = await leagueClient.GetFromJsonAsync<Summoner>($"/lol-summoner/v2/summoners/puuid/{puuid}");

            return summoner.SummonerId;
        }
        public async Task<Summoner?> GetLocalSummoner()
        {
            if (!_riotTokenService.TryGetLeaguePortAndToken(out string leagueToken, out string leaguePort))
                return null;

            var leagueClient = _httpClientFactory.CreateClient("SSLBypass");
            leagueClient.BaseAddress = new Uri($"https://127.0.0.1:{leaguePort}");
            leagueClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"riot:{leagueToken}")));
            leagueClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");
            
            var currentSummoner = await leagueClient.GetFromJsonAsync<Summoner>($"/lol-summoner/v1/current-summoner");

            return currentSummoner;
        }

        public class MutePlayerRequest
        {
            [JsonPropertyName("summonerId")]
            public long Id { get; set; }
            [JsonPropertyName("puuid")]
            public string Puuid { get; set; }
            [JsonPropertyName("obfuscatedSummonerId")]
            public long ObfuscatedId { get; set; }
            [JsonPropertyName("obfuscatedPuuid")]
            public string ObfuscatedPuuid { get; set; }

        }
        public class RerollPoints
        {
            [JsonPropertyName("currentPoints")]
            public int CurrentPoints { get; set; }

            [JsonPropertyName("maxRolls")]
            public int MaxRolls { get; set; }

            [JsonPropertyName("numberOfRolls")]
            public int NumberOfRolls { get; set; }

            [JsonPropertyName("pointsCostToRoll")]
            public int PointsCostToRoll { get; set; }

            [JsonPropertyName("pointsToReroll")]
            public int PointsToReroll { get; set; }
        }

        public class Summoner
        {
            [JsonPropertyName("accountId")]
            public long AccountId { get; set; }

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }

            [JsonPropertyName("gameName")]
            public string GameName { get; set; }

            [JsonPropertyName("internalName")]
            public string InternalName { get; set; }

            [JsonPropertyName("nameChangeFlag")]
            public bool NameChangeFlag { get; set; }

            [JsonPropertyName("percentCompleteForNextLevel")]
            public int PercentCompleteForNextLevel { get; set; }

            [JsonPropertyName("privacy")]
            public string Privacy { get; set; }

            [JsonPropertyName("profileIconId")]
            public int ProfileIconId { get; set; }

            [JsonPropertyName("puuid")]
            public string Puuid { get; set; }

            [JsonPropertyName("rerollPoints")]
            public RerollPoints RerollPoints { get; set; }

            [JsonPropertyName("summonerId")]
            public long SummonerId { get; set; }

            [JsonPropertyName("summonerLevel")]
            public int SummonerLevel { get; set; }

            [JsonPropertyName("tagLine")]
            public string TagLine { get; set; }

            [JsonPropertyName("unnamed")]
            public bool Unnamed { get; set; }

            [JsonPropertyName("xpSinceLastLevel")]
            public int XpSinceLastLevel { get; set; }

            [JsonPropertyName("xpUntilNextLevel")]
            public int XpUntilNextLevel { get; set; }
        }

        public class SummonerNamesRequest
        {
            [JsonPropertyName("puuids")]
            public List<string> Puuids { get; set; } = new List<string>();
        }

		public class Gnt
		{
			[JsonPropertyName("gameName")]
			public string GameName { get; set; }

			[JsonPropertyName("shadowGnt")]
			public bool ShadowGnt { get; set; }

			[JsonPropertyName("tagLine")]
			public string TagLine { get; set; }
		}

		public class Nameset
		{
			[JsonPropertyName("error")]
			public string Error { get; set; }

			[JsonPropertyName("gnt")]
			public Gnt Gnt { get; set; }

			[JsonPropertyName("playstationNameset")]
			public PlaystationNameset PlaystationNameset { get; set; }

			[JsonPropertyName("providerId")]
			public string ProviderId { get; set; }

			[JsonPropertyName("puuid")]
			public string Puuid { get; set; }

			[JsonPropertyName("switchNameset")]
			public SwitchNameset SwitchNameset { get; set; }

			[JsonPropertyName("xboxNameset")]
			public XboxNameset XboxNameset { get; set; }
		}

		public class PlaystationNameset
		{
			[JsonPropertyName("name")]
			public string Name { get; set; }
		}

		public class NamesetsResponse
		{
			[JsonPropertyName("namesets")]
			public List<Nameset> Namesets { get; set; }
		}

		public class SwitchNameset
		{
			[JsonPropertyName("name")]
			public string Name { get; set; }
		}

		public class XboxNameset
		{
			[JsonPropertyName("name")]
			public string Name { get; set; }
		}


	}
}
