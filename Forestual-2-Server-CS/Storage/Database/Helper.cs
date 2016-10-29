using System.IO;
using System.Linq;
using System.Windows.Forms;
using Forestual2Core;
using Forestual2Core.Enumerations;
using Forestual2ServerCS.Internal;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Storage.Database
{
    public class Helper
    {
        public static Values GetDatabase() {
            try {
                return JsonConvert.DeserializeObject<Values>(File.ReadAllText(Application.StartupPath + "\\database.json"));
            } catch {
                throw new IOException();
            }
        }

        public static bool Exists() {
            return File.Exists(Application.StartupPath + "\\database.json");
        }

        public static void Save() {
            Server.Database.Channels.Clear();
            Server.Database.Channels.AddRange(Server.Channels.FindAll(c => c.Persistent && c.Id != "lnr-forestual"));
            File.WriteAllText(Application.StartupPath + "\\database.json", JsonConvert.SerializeObject(Server.Database, Formatting.Indented));
            Server.Database = GetDatabase();
        }

        public static Account GetAccount(string id) {
            return Server.Database.Accounts.Find(a => a.Id == id);
        }

        public static bool AccountExists(string id) {
            return Server.Database.Accounts.Find(a => a.Id == id) != null;
        }

        public static string GetAccountId(string name) {
            return Server.Database.Accounts.Find(a => a.Name == name).Id;
        }

        public static bool AccountHasFlags(Account account, params Flag[] flags) {
            return flags.All(f => account.Flags.Contains(f) || Server.Database.Ranks.Find(r => r.Id == account.RankId).Flags.Contains(f) || account.Flags.Contains(Flag.Wildcard) || Server.Database.Ranks.Find(r => r.Id == account.Id).Flags.Contains(Flag.Wildcard));
        }
    }
}