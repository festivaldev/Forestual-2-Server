using System;
using System.Linq;
using Forestual2Core;
using F2CE = Forestual2Core.Enumerations;
using Forestual2ServerCS.Internal;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Management
{
    public class PunishmentManager
    {
        public static string GetRandomIdentifier(int length) {
            const string Characters = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random Rnd = new Random();
            return new string(Enumerable.Repeat(Characters, length).Select(s => s[Rnd.Next(s.Length)]).ToArray());
        }

        public static void CreateRecord(Punishment punishment) {
            var ERecordId = CheckForRecords(punishment.AccountId, punishment.Type);
            if (ERecordId != "-1")
                DisposeRecord(ERecordId);
            Server.Database.Punishments.Add(punishment);
            Storage.Database.Helper.Save();

            var Connection = Server.Connections.Find(c => c.Owner.Id == punishment.AccountId);
            if (!Connection.Owner.Online)
                return;
            F2CE.ClientState State;
            if (punishment.Type == F2CE.PunishmentType.Bann || punishment.Type == F2CE.PunishmentType.BannTemporarily) {
                State = F2CE.ClientState.Banned;
            } else {
                State = F2CE.ClientState.Muted;
            }
            Connection.SetStreamContent(string.Join("|", F2CE.Action.SetState.ToString(), State.ToString(), JsonConvert.SerializeObject(punishment)));
        }

        public static string CheckForRecords(string accountId, params F2CE.PunishmentType[] types) {
            DisposeExceededRecords();
            foreach (var PType in types) {
                var Punishment = Server.Database.Punishments.Find(p => p.Type == PType && p.AccountId == accountId);
                if (Punishment != null)
                    return Punishment.Id;
            }
            return "-1";
        }

        public static Punishment GetRecord(string id) {
            return Server.Database.Punishments.Find(p => p.Id == id);
        }

        public static void DisposeExceededRecords() {
            var ActiveRecords = Server.Database.Punishments.FindAll(p => p.EndDate.CompareTo(DateTime.Now) > 0 || p.Type == F2CE.PunishmentType.Bann);
            Server.Database.Punishments.Clear();
            Server.Database.Punishments.AddRange(ActiveRecords);
            Storage.Database.Helper.Save();
        }

        public static void DisposeRecord(string id) {
            var ActiveRecords = Server.Database.Punishments.FindAll(p => p.Id != id);
            Server.Database.Punishments.Clear();
            Server.Database.Punishments.AddRange(ActiveRecords);
            Storage.Database.Helper.Save();
        }
    }
}