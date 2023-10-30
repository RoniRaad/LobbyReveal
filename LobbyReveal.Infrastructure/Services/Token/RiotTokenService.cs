using LobbyReveal.Core.Interfaces;

namespace LobbyReveal.Infrastructure.Services.Token
{
    public sealed class RiotTokenService : IRiotTokenService
    {

        public bool IsFileLocked(string filePath)
        {

            if (!File.Exists(filePath))
                return false;

            try
            {
                using FileStream inputStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                return inputStream.Length <= 0;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public bool TryGetLeaguePortAndToken(out string token, out string port)
        {
            port = "";
            token = "";
            var fileName = $@"{GetLeagueInstallPath()}\lockfile";
            if (!IsFileLocked(fileName))
                return false;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader fileReader = new StreamReader(fileStream))
            {
                if (!fileReader.EndOfStream)
                {
                    var leagueLockFile = fileReader.ReadLine();
                    if (string.IsNullOrEmpty(leagueLockFile))
                        return false;

                    var leagueParams = leagueLockFile.Split(":");
                    token = leagueParams[3];
                    port = leagueParams[2];
                    return true;
                }
            }

            return false;
        }
        public bool TryGetRiotPortAndToken(out string token, out string port)
        {
            port = "";
            token = "";
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fileName = $@"{appDataPath}\Riot Games\Riot Client\Config\lockfile";
            if (!IsFileLocked(fileName))
                return false;

            using (FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader fileReader = new(fileStream))
            {
                if (!fileReader.EndOfStream)
                {
                    var lockfileContents = fileReader.ReadLine();
                    if (lockfileContents == null)
                        return false;

                    var riotParams = lockfileContents.Split(":");
                    token = riotParams[3];
                    port = riotParams[2];
                    return true;
                }
            }

            return false;
        }

        private DriveInfo? GetLeagueDrive()
        {
            return DriveInfo.GetDrives().FirstOrDefault(
                (drive) => Directory.Exists(@$"{drive?.RootDirectory}\Riot Games\League of Legends\"), null);
        }

        public string GetLeagueInstallPath()
        {
            return @$"{GetLeagueDrive()}\Riot Games\League of Legends\";
        }
    }
}
