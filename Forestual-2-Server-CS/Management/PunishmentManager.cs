using System;
using System.Linq;
using F2Core;
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
            Enumerations.ClientState State;
            if (punishment.Type == Enumerations.PunishmentType.Bann || punishment.Type == Enumerations.PunishmentType.BannTemporarily) {
                State = Enumerations.ClientState.Banned;
            } else {
                State = Enumerations.ClientState.Muted;
            }
            Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetState.ToString(), State.ToString(), JsonConvert.SerializeObject(punishment)));

            // Extension Management
            ListenerManager.InvokeEvent(Event.PunishmentRecorded, punishment);
            //End
        }

        public static string CheckForRecords(string accountId, params Enumerations.PunishmentType[] types) {
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
            var ActiveRecords = Server.Database.Punishments.FindAll(p => p.EndDate.CompareTo(DateTime.Now) > 0 || p.Type == Enumerations.PunishmentType.Bann);
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