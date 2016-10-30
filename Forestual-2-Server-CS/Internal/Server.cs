using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using Forestual2Core;
using F2CE = Forestual2Core.Enumerations;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Internal
{
    public class Server
    {
        public static Storage.Database.Values Database;
        public static Storage.Configuration.Values Config;

        public static List<Connection> Connections;
        public static List<Channel> Channels;
        private Channel Forestual;
        private Account Root;

        private RSACryptoServiceProvider PreServiceProvider = new RSACryptoServiceProvider(4096);
        private RSACryptoServiceProvider ServiceProvider = new RSACryptoServiceProvider(4096);
        private const string PrivateKey = "<RSAKeyValue><Modulus>n1G5qxqPSnvu3A0ympXV/qHCeMasaXOqrmlIF/2sAMgrjYmCXcAeyplvirGPDOUPHHUIBmZzqbtmU5Ol2l9VpMEesuDneEZh8nB9dpvtNe+LpoDAX4qVvrf78SXDzT9biFwJj8AAUgYI1JA2lN/+rHYCOYTlfrn1cln3q2F1sbtOKfJyYdt5PsbALI2In3b134k4XP93W5fLqNSFHbG3LcWTLkU06/cobg8etttjyyg5svUAEN+LnhtfrGilLW67oi4vHnjzhggEy7zo2RGfs2PJ8CnwlmAOGGtN/DaPTjobeHZRrIsIWy9/SPpSozaUV/mNxkrvYFEgE0BP6KCgS7HVXcJbsOcNIKIdUhRgRkXKT5XF7wakw9SjD3BCNZRIbfruBbN/dUx0jHgdU1zLJ1gVQcE0P/Fyrubq6VcKSTLrhygz2CkRSqUmE9MVmbISmDv13cI/lg/sTbEEpxWF+6lZdxmts5GVxjvTLbbv0CglRu8SyYGycWtHkSYsVEKYwBV5DRXfEWN8/uJcgrWxYNKH8+1nld/RSKVQ2lYKK2b0cJF4OHuhNGubNDDUn99LZviQmNQAzaK4hTFtRGaTVhcMOgl6KdEafQ6/oy9l9ynk+dw9HUJSq521ef1tFEvFYp3jNIjG8fcMikr5XuOoETaLFsdBNRPqMGQm7BuRzc8=</Modulus><Exponent>AQAB</Exponent><P>0izEpTNE2NV+nxhotvIqDEmKrYe9NlHqjvcr4uqbDTDem+Ci84aOnxwSeVF/zgL2svqvU+XiyT58EbxcHWpByhtbl2GleqBPhdLgkwAJhMkauVMzz4dQ9E0JPX3J2nHAuE0XGLl+wZ+o+ZFg8X9JVF0VrJ/V8e/HSWuomIexF8O8nE5SmhX6CqBPhJj6tMYTRv3gGa8cJ2VMwv0QFgk5YLAV5LCwGSLSQ9gIXFQVu9NUWNnbqhHj4N4r3O8fm+X3bsWkf/PdqB114bPtCv7hp6exD/f8p2JhP4/E88U8RvaaZsnVVNjsLVTyIDfXUeZihdXYObhZLjAnYiTSrcKeew==</P><Q>wg5fQACeKLxBwsGeq9qWuGhlFDdwWf5iui0s1m2bt87imrUbAsvZL+QHcY8jMvw0MOVGEGVxX9JA9T8ug5KVOS+ofJvibezAgmhZl5XbMJY3l/sjvHCdmoDdZetaw/lNuBqAMk567gCN/+evMt2dVJWBIyIsyt3dFQsA+r2Mk5ZJxJzPbXppoSr0fHfEtclmWMNI97VhXzLoxCvJBtAl3bmHctp0HR+uH8kjwKitXsVVQ8UHasFy65IT+Z4c1jt1crotK4rSgUo6vhpLjeiUd9sX8IsXmDk0mY5HFXaN5EqF7wSo/MCTRTNx2KbwCjl9cPyBr8tWj+QDrjHIUeVXvQ==</Q><DP>zjyZ1hWiCDgvIQS9tE+bDSWZDED3XXcyeIl4qhlWfrImrsTWgarXBrBwPFXJ2Ki11dkB9IzPZnSHIIw5w6+B0UXZMYni7Jqkjgfo0LanoIIKVDKd05XPzXpOh+WIDm+zEeartFpJVMxL7mFGxJMHrN4Op67MLLUCVDxtWwdDsrMiwCpnCcZo7sZyYQYQdRUs02vJ3MolEU9o7KmQgF8ay5LeWOM8Wd3+gA5b3eWw0fdEfE+DKraVaxH37rtCxCL8EtmkWt488nu+MfTxtOl5GqAFskrAxKtYDBwSwrYXOPdBeX2ydajK0Izbbtv80OQGZ5f4rmMEN7uO2dKSXWltQw==</DP><DQ>GiPNeNWceGhDg3SJZyTewKBvXTXKkJTPv7xuGcRSAYSAyc4zgUDsVKMmzYk2eJu4fA2mTncbuoib720/WsHYEAf3bjGhYqVNmUNtLholmHnjqzlNKwkQccuCB0SYyWU/rtkDA8PGk2DHv/z5gKSRmN86sfzcg8c3DKqayyvVT9wiu7VTy699oxQiMtH/UW17t+E2ZwerwiMdb69mOOC4+REQycvbcEDgN6/kfQM7t1Rlk+dqhFrinBDlV+6Qe9suivHBO+hLStcw6oKoQsldlneQ1fomh37NMxITSTTbEDFpsTSzfriCHgQ8Ba8XDomH+DxLS97cHi3cwQ47qax3EQ==</DQ><InverseQ>VUC9weP0nmDNwJ/tB5ezEDtik012Zr/HqkIqszaIgWvPaVGWhASSA/qpNGJwgeSLNC0i/FZX6gsxr9dQ3fWGZC+c2dxPV2T9+DW2cNkLMDbvsf4YK5ZtmAza+49rhN8zKETZkqkGGA2KRsP6qRaLRzJw5+7JgpkuPrVMcIGlt4m5JR2QyBab31Z5mmCyQeNAG9DCRdqU3VreKb/6aNNGY5wQ4Rs0ObEn39CqUuA0DuKMZKzyLcPyR8fR/Nf2cEzTIm5fTH5Zfx2rGdGbalHXSkAxMUOy+i2WynNVjPiXsSg0CGY+vO+cb+VnnXR5Wy148901IHH0NN9DdzcyaSeuBQ==</InverseQ><D>DPcZpP2vAOC/uiiuIDu8A9ImLANUWes6eKHDZtsTTvz7OQl3vSllWBd50aT21JXPela2eyQwmsolXv0lAiBqrShfgdnLpyi9z9JXuM9M/paqnAzeRZLW8i5cJ8PAVh5R1JxTgDSf0g0BAEtm0GFqLZ7COkFrwQ8Lv8KSj9/eiW2KGYp2xH/tM1lOn6bk92qMQnoS8X6DYsYiTMWpZOvm0agX0iwk97mlZfoqWwx/kojeKTIcT0M3RCagzTxhiiZOHrn4xldmz1bXt7zSi4Jjp2BM5BPbAGHQw9aiV2QZRW8fzS3TzQwuIeg7njTA2jIW4GdD97R2xop+PGgqGJmkcbj2/r6AqWuDdCcb/Xggs+dNlH7XYzxooz0MPXz6jeyl0r7ScFl00SQ51cNTV7F2Euq3a6oj9Qwu7FV3pWgrWp+FmyU0m6ikb3YAUWDlA6riQU5XU79sarVB7sussXFVP6ONYNPwU1rldkMavPgF/KUypXfm8EYVre+Hu1C5KEgmp/Y0QRdo+mTN5zzDwvYvoXZVtyyFvqAu2jTEt1TFeS0oybJtGLk3Jm9/vtMz6JjD8vpd2+yIRqN+6NCnFVP58Ss/wek5Yy0X3+B8xQUk8QYR95KUu9GnafNrCgeqpd2qOw/39NwzVp5yIqw2fckJzE2N5dDQcN62DA5GEFO1NAE=</D></RSAKeyValue>";
        private const string PublicKey = "<RSAKeyValue><Modulus>pFLs/4KQ1sQy5EQxPlGNpwD3/aP52csVtFTMLmR0vKaoAD0CjggLxq0v14R/idIc1rN7KYA+Yv3ZqTHchTCx5DIhMX5f31uPNWvrKxbw7mmOjL2/cgP5KI399pbcBse/3sfOiPMgzZPaKuFI4x/cET03WJoG2Uexwu4/pZHVVBiCkqOSI4pUGxhR4ZsCE+sFczhy6HhHNH29wRzTKL7dZpERobZlcgjWAxyX6iPS7emnbuxdVfES2r/X5uW812KMPDaSEW0NYK2mQ/cnZtdQp1TbiOv/NsFaTlDlOKlbKAwmq/bJZoyfqi8U0Z00B0iWypXL3O9fbodNNFZDftq/4flpsiRxrEqqeYp3zsAaewdgtvmha0rs90xH08Vhgi+i+xesvxLR/pxL5iw3x2dE6PMmLkCrh0T2sNqauqE2LEKANo2r29Nr83aa9X8Cs+Oqe/y30xEInWcF7iKMQCiaRfWORuxdgjaSXybGkbEe2iDhK8X8aTbkq7xKGSzmP53FxDi0U2kkIRZoCnrkk54MYPDAgPR0lWOOq9Cc9A5iipOTyMiioNP+d7OH7/97EwEC2sIC2daYA3kwA0mrTB2GV1wCSOBgZGY/j2zmSfonPXH8YitLWj++vlM3XOz0Fd73G9fRLHk84zNopxXH2xbom2j8LItaHP4SxRwFN0IQQuk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private TcpListener FServer;
        private TcpClient FClient;
        private IPEndPoint FIPEndPnt = new IPEndPoint(IPAddress.Any, int.Parse(Config.ServerPort));

        private Thread WaitingThread;
        private bool ExitThreadOnPurpose;

        public bool Start() {
            try {
                File.Copy(Application.StartupPath + "\\database.json", Application.StartupPath + "\\database.json.bak");
                File.Copy(Application.StartupPath + "\\config.json", Application.StartupPath + "\\config.json.bak");
            } catch { }
            Connections.Clear();
            ExitThreadOnPurpose = false;
            ServiceProvider.FromXmlString(PrivateKey);
            PreServiceProvider.FromXmlString(PublicKey);
            // Localization
            Forestual.Id = "lnr-forestual";
            Forestual.Name = "Forestual";
            Forestual.Owner = Root;
            Forestual.Persistent = true;
            Forestual.JoinRestrictionMode = F2CE.ChannelJoinMode.Default;
            try {
                FServer = new TcpListener(FIPEndPnt);
                FServer.Start();
                WaitingThread = new Thread(Wait);
                WaitingThread.Start();
            } catch {
                return false;
            }
            Channels.RemoveAll(c => c.Persistent);
            Channels.AddRange(Database.Channels);
            Channels.Add(Forestual);
            Management.PunishmentManager.DisposeExceededRecords();
            // RaiseEvent Connected
            return true;
        }

        public void Stop() {
            try {
                try {
                    SendToAll(string.Join("|", F2CE.Action.Disconnect.ToString(), Config.ServerShutdownMessage));
                    Database.Accounts.ForEach(a => a.Online = false);
                } catch { }
                FServer.Stop();
                ExitThreadOnPurpose = true;
                WaitingThread.Abort();
            } catch { }
        }

        private void Wait() {
            while (true) {
                FClient = FServer.AcceptTcpClient();
                var Connection = new Connection(FClient.GetStream());
                var RawStreamContent = Connection.GetRawStreamContent();
                var DRawStreamContent = Cryptography.RSADecrypt(RawStreamContent, ServiceProvider);
                if (DRawStreamContent == F2CE.Action.GetServerMetaData.ToString()) {
                    Connection.SetRawStreamContent(Cryptography.RSAEncrypt(JsonConvert.SerializeObject(GetMetaData()), PreServiceProvider));
                    Connection.Dispose();
                    Management.PunishmentManager.DisposeExceededRecords();
                } else {
                    var SessionData = DRawStreamContent.Split('|');
                    var AesData = new AesData {
                        Key = Convert.FromBase64String(SessionData[0]),
                        IV = Convert.FromBase64String(SessionData[1])
                    };
                    Connection.AesData = AesData;
                    Connection.HmacKey = Convert.FromBase64String(SessionData[2]);
                    var AuthData = Connection.GetStreamContent().Split('|');
                    var Account = Storage.Database.Helper.GetAccount(AuthData[0]);
                    if (Account != null) {
                        if (Account.Password == AuthData[1]) {
                            var PunishmentId = Management.PunishmentManager.CheckForRecords(Account.Id, F2CE.PunishmentType.Bann, F2CE.PunishmentType.BannTemporarily);
                            if (PunishmentId != "-1") {
                                var Punishment = Management.PunishmentManager.GetRecord(PunishmentId);
                                Connection.SetStreamContent(string.Join("|", F2CE.Action.SetState.ToString(), F2CE.ClientState.Banned.ToString(), JsonConvert.SerializeObject(Punishment)));
                            } else {
                                Connection.Owner = Account;
                                Connection.Channel = Forestual;
                                Connections.Add(Connection);
                                var ListeningThread = new Thread(Listen);
                                ListeningThread.Start(Connection);
                                Connection.SetStreamContent(string.Join("|", F2CE.Action.LoginResult.ToString(), "hej"));
                                var Flags = new List<F2CE.Flag>();
                                Flags.AddRange(Connection.Owner.Flags);
                                Flags.AddRange(Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).Flags);
                                Connection.SetStreamContent(string.Join("|", F2CE.Action.SetFlags, JsonConvert.SerializeObject(Flags)));
                                Forestual.MemberIds.Add(Account.Id);
                                // Send Channel List
                                var Message = new Forestual2Core.Message {
                                    Time = DateTime.Now.ToShortTimeString(),
                                    Type = F2CE.MessageType.Center,
                                    Content = $"{Connection.Owner.Name} (@{Connection.Owner.Id}) hat den Chat betreten."
                                    // Demo Purposes
                                };
                                SendMessageToAll(Message);
                                Connection.Owner.Online = true;
                                SendToAll(string.Join("|", F2CE.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                                // RaiseEvent PrintToConsole
                            }
                        } else {
                            Connection.SetStreamContent(string.Join("|", F2CE.Action.LoginResult, "authentificationFailed"));
                        }
                    } else {
                        Connection.SetStreamContent(string.Join("|", F2CE.Action.LoginResult, "accountUnknown"));
                    }
                }
            }
        }

        private void Listen(object o) {
            var Connection = (Connection) o;
            while (true) {
                if (ExitThreadOnPurpose) {
                    return;
                }
                try {
                    var Content = Connection.GetStreamContent();
                    var PunishmentId = Management.PunishmentManager.CheckForRecords(Connection.Owner.Id, F2CE.PunishmentType.Mute);
                    if (PunishmentId != "-1") {
                        var RemainingTime = Management.PunishmentManager.GetRecord(PunishmentId).EndDate.Subtract(DateTime.Now);
                        var Message = new Forestual2Core.Message {
                            Time = DateTime.Now.ToShortTimeString(),
                            Type = F2CE.MessageType.Center,
                            RankColor = "#FC3539",
                            Content = $"Your mute still lasts {RemainingTime.Days} days, {RemainingTime.Hours} hours, {RemainingTime.Minutes} minutes and {RemainingTime.Seconds} seconds."
                        };
                        SendMessageTo(Connection.Owner.Id, Message);
                        continue;
                    }
                    try {
                        var Contents = Content.Split('|');
                        if (string.IsNullOrEmpty(Contents[0]) || string.IsNullOrWhiteSpace(Contents[0]))
                            continue;
                        var Type = (F2CE.Action) Enum.Parse(typeof(F2CE.Action), Contents[0]);
                        switch (Type) {
                        case F2CE.Action.Plain:
                            if (Contents[1].StartsWith("/")) {
                                var CommandParts = Contents[1].Remove(0, 1).Split(' ');
                                switch (CommandParts[0]) {
                                case "clear":
                                    SendToChannel(Connection.Channel.Id, F2CE.Action.ClearConversation.ToString());
                                    break;
                                case "vuohen":
                                    var Account = CommandParts.Length >= 3 ? Storage.Database.Helper.GetAccount(CommandParts[2]) : null;
                                    var Value = CommandParts.Length >= 4 ? int.Parse(CommandParts[3]) : 0;
                                    switch (CommandParts[1]) {
                                    case "add":
                                        Account.Deposit += Value;
                                        break;
                                    case "remove":
                                        Account.Deposit -= Value;
                                        if (Account.Deposit < 0)
                                            Account.Deposit = 0;
                                        break;
                                    case "set":
                                        Account.Deposit = Value;
                                        break;
                                    case "send":
                                        if (Connection.Owner.Deposit < Value) {
                                            var Message1 = new Forestual2Core.Message() {
                                                Time = DateTime.Now.ToShortTimeString(),
                                                Type = F2CE.MessageType.Center,
                                                RankColor = "#FC3539",
                                                Content = "Your do not have enough indian hillside goats to perform this transaction."
                                            };
                                            SendMessageTo(Connection.Owner.Id, Message1);
                                        } else {
                                            Connection.Owner.Deposit -= Value;
                                            Account.Deposit += Value;
                                        }
                                        break;
                                    case "balance":
                                        var Message2 = new Forestual2Core.Message() {
                                            Time = DateTime.Now.ToShortTimeString(),
                                            Type = F2CE.MessageType.Center,
                                            RankColor = Config.ServerBroadcastColor,
                                            Content = $"Your balance is {Connection.Owner.Deposit} indian hillside goats."
                                        };
                                        SendMessageTo(Connection.Owner.Id, Message2);
                                        break;
                                    }
                                    break;
                                }
                            } else {
                                var Message3 = new Forestual2Core.Message {
                                    SenderId = Connection.Owner.Id,
                                    SenderPrefix = ComposePrefix(Connection.Owner.Id),
                                    RankColor = Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).Color,
                                    Time = DateTime.Now.ToShortTimeString(),
                                    Content = Contents[1],
                                    Type = F2CE.MessageType.Left
                                };
                                SendMessageToAllExceptTo(Message3.SenderId, Message3, Connection.Channel.Id);
                                Message3.Type = F2CE.MessageType.Right;
                                SendMessageTo(Message3.SenderId, Message3);
                            }
                            break;
                        case F2CE.Action.RegisterRecord:
                            var Punishment = JsonConvert.DeserializeObject<Punishment>(Contents[1]);
                            Punishment.CreatorId = Connection.Owner.Id;
                            Punishment.Id = Management.PunishmentManager.GetRandomIdentifier(6);
                            Management.PunishmentManager.CreateRecord(Punishment);
                            break;
                        }
                    } catch {
                        // Syntax Error in Command
                    }
                } catch {
                    Connections.Remove(Connection);
                    var Message = new Forestual2Core.Message {
                        Time = DateTime.Now.ToShortTimeString(),
                        Type = F2CE.MessageType.Center,
                        Content = $"{Connection.Owner.Name} disconnected."
                        // Demo Purposes
                    };
                    SendMessageToAll(Message);
                    Connection.Channel.MemberIds.Remove(Connection.Owner.Id);
                    if (Connection.Channel.Owner == Connection.Owner)
                        Connection.Channel.Owner = null;
                    if (Connection.Channel.MemberIds.Count == 0 && !Connection.Channel.Persistent) {
                        Channels.Remove(Connection.Channel);
                        // Send Channel List
                    }
                    Connection.Owner.Online = false;
                    SendToAll(string.Join("|", F2CE.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                    // RaiseEvent Print To Console
                    Connection.Dispose();
                    return;
                }
            }
        }

        private string ComposePrefix(string accountId) {
            var Account = Connections.Find(c => c.Owner.Id == accountId).Owner;
            var Rank = Database.Ranks.Find(r => r.Id == Account.RankId);
            return $"[{Rank.Name}] {Account.Name}";
        }

        private void SendToAll(string content) {
            Connections.ForEach(c => c.SetStreamContent(content));
        }

        private void SendToChannel(string id, string content) {
            Connections.FindAll(c => c.Channel.Id == id).ForEach(c => c.SetStreamContent(content));
        }

        private void SendMessageTo(string id, Forestual2Core.Message message) {
            Connections.Find(c => c.Owner.Id == id).SetStreamContent(string.Join("|", F2CE.Action.Plain, JsonConvert.SerializeObject(message)));
        }

        private void SendMessageToAll(Forestual2Core.Message message) {
            Connections.ForEach(c => c.SetStreamContent(string.Join("|", F2CE.Action.Plain, JsonConvert.SerializeObject(message))));
        }

        private void SendMessageToAllExceptTo(string accountId, Forestual2Core.Message message, string channelId) {
            Connections.FindAll(c => c.Owner.Id != accountId && c.Channel.Id == channelId).ForEach(c => c.SetStreamContent(string.Join("|", F2CE.Action.Plain, JsonConvert.SerializeObject(message))));
        }

        private List<Account> GetAccountsWithoutPassword() {
            var Accounts = new List<Account>();
            Database.Accounts.ForEach(a => Accounts.Add(new Account() { Deposit = a.Deposit, Flags = a.Flags, Id = a.Id, Name = a.Name, Online = a.Online, Password = Database.Ranks.Find(r => r.Id == a.RankId).Color, RankId = a.RankId }));
            return Accounts;
        }

        private ServerMetaData GetMetaData() {
            var Meta = new ServerMetaData {
                AcceptsGuests = Config.MetaAcceptsGuests,
                AcceptsRegistration = Config.MetaAcceptsRegistration,
                AccountsInstantlyActivated = Config.MetaAccountsInstantlyActivated,
                GuestsCanChooseName = Config.MetaGuestsCanChooseName,
                Language = Config.ServerLanguage,
                Name = Config.MetaServerName,
                OperatorWebsiteUrl = Config.MetaWebsiteUrl,
                OwnerId = Config.MetaOwnerId,
                RequiredClientVersion = Config.MetaRequiredClientVersion,
                RequiresAuthentification = Config.MetaRequiresAuthentification,
                RequiresInvitation = Config.MetaRequiresInvitation
            };
            return Meta;
        }
    }
}