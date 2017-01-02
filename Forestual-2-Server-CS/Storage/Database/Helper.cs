using System.IO;
using System.Linq;
using System.Windows.Forms;
using F2Core;
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

        public static void CreateDefault() {
            File.WriteAllText(Application.StartupPath + "\\database.json", JsonConvert.SerializeObject(new Values(), Formatting.Indented));
        }

        public static void Save() {
            Server.Database.Channels.Clear();
            Server.Database.Channels.AddRange(Server.Channels.FindAll(c => c.Persistent && c.Id != "lnr-forestual"));
            File.WriteAllText(Application.StartupPath + "\\database.json", JsonConvert.SerializeObject(Server.Database, Formatting.Indented));
            Server.Database = GetDatabase();
        }

        public static bool AccountExists(string id) {
            return AccountExists(Server.Database, id);
        }

        public static bool AccountExists(Values values, string id) {
            return values.Accounts.Any(Account => Account.Id == id);
        }

        public static Account GetAccount(string id) {
            return GetAccount(Server.Database, id);
        }

        public static Account GetAccount(Values values, string id) {
            return values.Accounts.Find(a => a.Id == id);
        }

        public static string GetAccountId(string name) {
            return Server.Database.Accounts.Find(a => a.Name == name).Id;
        }

        public static bool AccountHasFlags(Account account, params Enumerations.Flag[] flags) {
            return AccountHasFlags(Server.Database, account, flags);
        }

        public static bool AccountHasFlags(Values values, Account account, params Enumerations.Flag[] flags) {
            return flags.All(f => account.Flags.Contains(f) || values.Ranks.Find(r => r.Id == account.RankId).Flags.Contains(f) || account.Flags.Contains(Enumerations.Flag.Wildcard) || values.Ranks.Find(r => r.Id == account.RankId).Flags.Contains(Enumerations.Flag.Wildcard));
        }
    }
}