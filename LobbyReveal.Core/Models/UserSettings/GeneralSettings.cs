﻿namespace LobbyReveal.Core.Models.UserSettings
{
    public sealed class GeneralSettings
    {
        public GeneralSettings()
        {
            var potentialRiotDrives = DriveInfo.GetDrives().Where((drive) => Directory.Exists(Path.Combine(drive.ToString(), "Riot Games")));
            var potentialSteamDrives = DriveInfo.GetDrives().Where((drive) => Directory.Exists(Path.Combine(drive.ToString(), "Program Files (x86)", "Steam")));
            var potentialEpicGamesDrives = DriveInfo.GetDrives().Where((drive) => Directory.Exists(Path.Combine(drive.ToString(), "Program Files (x86)", "Epic Games")));
            var riotDrive = potentialRiotDrives.Any() ? potentialRiotDrives.First() : null;
            
            if (riotDrive is not null)
                RiotInstallDirectory = Path.Combine(riotDrive.ToString(), "Riot Games");

            if (potentialSteamDrives.Any())
                SteamInstallDirectory = Path.Combine(potentialSteamDrives.First().ToString(),
                    "Program Files (x86)", "Steam");

            if (potentialEpicGamesDrives.Any())
                EpicGamesInstallDirectory = Path.Combine(potentialEpicGamesDrives.First().ToString(),
                    "Program Files (x86)", "Epic Games");
        }

        public string RiotInstallDirectory { get; set; } = "";
        public string SteamInstallDirectory { get; set; } = "";
        public string EpicGamesInstallDirectory { get; set; } = "";
    }
}
