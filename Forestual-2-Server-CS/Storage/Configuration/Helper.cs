using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Storage.Configuration
{
    public class Helper
    {
        public static Values GetConfig() {
            try {
                return JsonConvert.DeserializeObject<Values>(File.ReadAllText(Application.StartupPath + "\\config.json"));
            } catch {
                throw new IOException();
            }
        }

        public static bool Exists() {
            return File.Exists(Application.StartupPath + "\\config.json");
        }

        public static void CreateDefault() {
            Values Config = new Values {
                ConsoleRequiresAuthentification = true,
                ConsoleAuthentificationTimeout = true,
                CATimeoutTime = 60,
                ServerPort = "42000",
                ServerLanguage = "de-de",
                ServerBroadcastColor = "#1E90FF",
                ServerShutdownMessage = "The server went offline!",
                MetaServerName = "Forestual 2 Server",
                MetaOwnerId = "server",
                MetaWebsiteUrl = "https://festival.ml/",
                MetaRequiresAuthentification = true,
                MetaAcceptsGuests = false,
                MetaGuestsCanChooseName = false,
                MetaAcceptsRegistration = false,
                MetaRequiresInvitation = false,
                MetaAccountsInstantlyActivated = false,
                MetaRequiredClientVersion = "0"
            };
            File.WriteAllText(Application.StartupPath + "\\config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
