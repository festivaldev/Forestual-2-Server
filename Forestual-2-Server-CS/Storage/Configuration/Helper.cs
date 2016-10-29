using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Storage.Configuration
{
    public class Helper
    {
        public static Database.Values GetDatabase() {
            try {
                return JsonConvert.DeserializeObject<Database.Values>(File.ReadAllText(Application.StartupPath + "\\config.json"));
            } catch {
                throw new IOException();
            }
        }

        public static bool Exists() {
            return File.Exists(Application.StartupPath + "\\config.json");
        }

        public static void CreateDefault() {
            Values Config = new Values();
            Config.ConsoleRequiresAuthentification = true;
            Config.ConsoleAuthentificationTimeout = true;
            Config.CATimeoutTime = 60;
            Config.ServerPort = "42000";
            Config.ServerLanguage = "de-de";
            Config.ServerBroadcastColor = "#1E90FF";
            Config.ServerShutdownMessage = "Shutdown";
            Config.MetaServerName = "Forestual 2 Server";
            Config.MetaOwnerId = "server";
            Config.MetaWebsiteUrl = "https://festival.ml/";
            Config.MetaRequiresAuthentification = true;
            Config.MetaAcceptsGuests = false;
            Config.MetaGuestsCanChooseName = false;
            Config.MetaAcceptsRegistration = false;
            Config.MetaRequiresInvitation = false;
            Config.MetaAccountsInstantlyActivated = false;
            Config.MetaRequiredClientVersion = "0";
            File.WriteAllText(Application.StartupPath + "\\config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
