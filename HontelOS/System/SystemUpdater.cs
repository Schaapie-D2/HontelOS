using HontelOS.System.Network;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HontelOS.System
{
    public class SystemUpdater
    {
        static string hontelOSURL = "http://os.hontel.rf.gd/updates.json";

        public static bool IsNewVersionDownloaded = false;

        public static SystemUpdate? SearchForUpdates()
        {
            string updates = HTTP.DownloadFile(hontelOSURL);
            SystemUpdatesFile? desUpdates = JsonConvert.DeserializeObject<SystemUpdatesFile>(updates);

            if (desUpdates == null)
                return null;

            if(desUpdates.LatestVersionNumber != VersionInfo.VersionNumber)
                return desUpdates.Updates.FirstOrDefault(verNum => verNum.VersionNumber == desUpdates.LatestVersionNumber);

            return null;
        }

        public static void DownloadUpdate(SystemUpdate update)
        {
            File.WriteAllBytes("1:\\boot\\New_HontelOS.bin", HTTP.DownloadRawFile(update.DownloadURL));
            IsNewVersionDownloaded = true;
        }

        public static bool InstallUpdate()
        {
            if(!IsNewVersionDownloaded)
                return false;

            if(!File.Exists("1:\\boot\\New_HontelOS.bin"))
                return false;

            try
            {
                File.Copy("1:\\boot\\HontelOS.bin", "1:\\boot\\Backup_HontelOS.bin", true);
                File.Move("1:\\boot\\New_HontelOS.bin", "1:\\boot\\HontelOS.bin", true);
            }
            catch
            {
                return false;
            } 

            return true;
        }

        public static bool RevertToBackup()
        {
            if(!File.Exists("1:\\boot\\Backup_HontelOS.bin"))
                return false;
            try
            {
                File.Move("1:\\boot\\Backup_HontelOS.bin", "1:\\boot\\HontelOS.bin", true);
            }
            catch
            {
                return false;
            }

            return true;
        }

        class SystemUpdatesFile
        {
            public string LatestVersion;
            public string LatestVersionNumber;
            public List<SystemUpdate> Updates;
        }
    }

    public class SystemUpdate
    {
        public string Description;
        public string Version;
        public string VersionNumber;
        public string DownloadURL;
    }
}
