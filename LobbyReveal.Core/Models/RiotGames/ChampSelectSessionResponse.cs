using System.Text.Json.Serialization;

namespace LobbyReveal.Core.Models.RiotGames
{
    public class Bans
    {
        [JsonPropertyName("myTeamBans")]
        public List<object> MyTeamBans { get; set; }

        [JsonPropertyName("numBans")]
        public int NumBans { get; set; }

        [JsonPropertyName("theirTeamBans")]
        public List<object> TheirTeamBans { get; set; }
    }

    public class ChatDetails
    {
        [JsonPropertyName("mucJwtDto")]
        public MucJwtDto MucJwtDto { get; set; }

        [JsonPropertyName("multiUserChatId")]
        public string MultiUserChatId { get; set; }

        [JsonPropertyName("multiUserChatPassword")]
        public string MultiUserChatPassword { get; set; }
    }

    public class EntitledFeatureState
    {
        [JsonPropertyName("additionalRerolls")]
        public int AdditionalRerolls { get; set; }

        [JsonPropertyName("unlockedSkinIds")]
        public List<object> UnlockedSkinIds { get; set; }
    }

    public class MucJwtDto
    {
        [JsonPropertyName("channelClaim")]
        public string ChannelClaim { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("jwt")]
        public string Jwt { get; set; }

        [JsonPropertyName("targetRegion")]
        public string TargetRegion { get; set; }
    }

    public class MyTeam
    {
        [JsonPropertyName("assignedPosition")]
        public string AssignedPosition { get; set; }

        [JsonPropertyName("cellId")]
        public int CellId { get; set; }

        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }

        [JsonPropertyName("championPickIntent")]
        public int ChampionPickIntent { get; set; }

        [JsonPropertyName("entitledFeatureType")]
        public string EntitledFeatureType { get; set; }

        [JsonPropertyName("nameVisibilityType")]
        public string NameVisibilityType { get; set; }

        [JsonPropertyName("obfuscatedPuuid")]
        public string ObfuscatedPuuid { get; set; }

        [JsonPropertyName("obfuscatedSummonerId")]
        public long ObfuscatedSummonerId { get; set; }

        [JsonPropertyName("puuid")]
        public string Puuid { get; set; }

        [JsonPropertyName("selectedSkinId")]
        public int SelectedSkinId { get; set; }

        [JsonPropertyName("spell1Id")]
        public int Spell1Id { get; set; }

        [JsonPropertyName("spell2Id")]
        public int Spell2Id { get; set; }

        [JsonPropertyName("summonerId")]
        public long SummonerId { get; set; }

        [JsonPropertyName("team")]
        public int Team { get; set; }

        [JsonPropertyName("wardSkinId")]
        public int WardSkinId { get; set; }
    }

    public class PickOrderSwap
    {
        [JsonPropertyName("cellId")]
        public int CellId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
    }

    public class ChampSelectSessionResponse
    {
        [JsonPropertyName("allowBattleBoost")]
        public bool AllowBattleBoost { get; set; }

        [JsonPropertyName("allowDuplicatePicks")]
        public bool AllowDuplicatePicks { get; set; }

        [JsonPropertyName("allowLockedEvents")]
        public bool AllowLockedEvents { get; set; }

        [JsonPropertyName("allowRerolling")]
        public bool AllowRerolling { get; set; }

        [JsonPropertyName("allowSkinSelection")]
        public bool AllowSkinSelection { get; set; }

        [JsonPropertyName("bans")]
        public Bans Bans { get; set; }

        [JsonPropertyName("benchChampions")]
        public List<object> BenchChampions { get; set; }

        [JsonPropertyName("benchEnabled")]
        public bool BenchEnabled { get; set; }

        [JsonPropertyName("boostableSkinCount")]
        public int BoostableSkinCount { get; set; }

        [JsonPropertyName("chatDetails")]
        public ChatDetails ChatDetails { get; set; }

        [JsonPropertyName("counter")]
        public int Counter { get; set; }

        [JsonPropertyName("entitledFeatureState")]
        public EntitledFeatureState EntitledFeatureState { get; set; }

        [JsonPropertyName("gameId")]
        public long GameId { get; set; }

        [JsonPropertyName("hasSimultaneousBans")]
        public bool HasSimultaneousBans { get; set; }

        [JsonPropertyName("hasSimultaneousPicks")]
        public bool HasSimultaneousPicks { get; set; }

        [JsonPropertyName("isCustomGame")]
        public bool IsCustomGame { get; set; }

        [JsonPropertyName("isSpectating")]
        public bool IsSpectating { get; set; }

        [JsonPropertyName("localPlayerCellId")]
        public int LocalPlayerCellId { get; set; }

        [JsonPropertyName("lockedEventIndex")]
        public int LockedEventIndex { get; set; }

        [JsonPropertyName("myTeam")]
        public List<MyTeam> MyTeam { get; set; }

        [JsonPropertyName("pickOrderSwaps")]
        public List<PickOrderSwap> PickOrderSwaps { get; set; }

        [JsonPropertyName("recoveryCounter")]
        public int RecoveryCounter { get; set; }

        [JsonPropertyName("rerollsRemaining")]
        public int RerollsRemaining { get; set; }

        [JsonPropertyName("skipChampionSelect")]
        public bool SkipChampionSelect { get; set; }

        [JsonPropertyName("theirTeam")]
        public List<TheirTeam> TheirTeam { get; set; }

        [JsonPropertyName("timer")]
        public Timer Timer { get; set; }

        [JsonPropertyName("trades")]
        public List<object> Trades { get; set; }
    }

    public class TheirTeam
    {
        [JsonPropertyName("assignedPosition")]
        public string AssignedPosition { get; set; }

        [JsonPropertyName("cellId")]
        public int CellId { get; set; }

        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }

        [JsonPropertyName("championPickIntent")]
        public int ChampionPickIntent { get; set; }

        [JsonPropertyName("entitledFeatureType")]
        public string EntitledFeatureType { get; set; }

        [JsonPropertyName("nameVisibilityType")]
        public string NameVisibilityType { get; set; }

        [JsonPropertyName("obfuscatedPuuid")]
        public string ObfuscatedPuuid { get; set; }

        [JsonPropertyName("obfuscatedSummonerId")]
        public int ObfuscatedSummonerId { get; set; }

        [JsonPropertyName("puuid")]
        public string Puuid { get; set; }

        [JsonPropertyName("selectedSkinId")]
        public int SelectedSkinId { get; set; }

        [JsonPropertyName("spell1Id")]
        public int Spell1Id { get; set; }

        [JsonPropertyName("spell2Id")]
        public int Spell2Id { get; set; }

        [JsonPropertyName("summonerId")]
        public int SummonerId { get; set; }

        [JsonPropertyName("team")]
        public int Team { get; set; }

        [JsonPropertyName("wardSkinId")]
        public int WardSkinId { get; set; }
    }

    public class Timer
    {
        [JsonPropertyName("adjustedTimeLeftInPhase")]
        public int AdjustedTimeLeftInPhase { get; set; }

        [JsonPropertyName("internalNowInEpochMs")]
        public long InternalNowInEpochMs { get; set; }

        [JsonPropertyName("isInfinite")]
        public bool IsInfinite { get; set; }

        [JsonPropertyName("phase")]
        public string Phase { get; set; }

        [JsonPropertyName("totalTimeInPhase")]
        public int TotalTimeInPhase { get; set; }
    }
}
