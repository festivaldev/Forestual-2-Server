using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using F2Core;
using F2Core.Compatibility;
using F2Core.Management;
using Forestual2ServerCS.Management;
using Forestual2ServerCS.Storage.Database;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Internal
{
    public class Server : IServer
    {
        public delegate void DConsoleMessageReceivedHandler(string content, bool newLine = true);

        public delegate void DConsoleColorChangedHandler(Color color);

        public delegate void DConnectedHandler(string address);

        public delegate void DDisplayFormHandler(Form form);

        public delegate void DRefreshAccounts();

        public event DConsoleMessageReceivedHandler ConsoleMessageReceived;
        public event DConsoleColorChangedHandler ConsoleColorChanged;
        public event DConnectedHandler Connected;
        public event DDisplayFormHandler DisplayFormEvent;
        public event DRefreshAccounts RefreshAccounts;

        public static Values Database = Helper.GetDatabase();
        public static Storage.Configuration.Values Config = Storage.Configuration.Helper.GetConfig();
        public static Storage.Localization.Values Lcl;

        public static List<Connection> Connections = new List<Connection>();
        public static List<Channel> Channels = new List<Channel>();
        private Channel Forestual = new Channel();
        private Account Root = new Account();

        private RSACryptoServiceProvider PreServiceProvider = new RSACryptoServiceProvider(4096);
        private RSACryptoServiceProvider ServiceProvider = new RSACryptoServiceProvider(4096);
        private const string PrivateKey = "<RSAKeyValue><Modulus>n1G5qxqPSnvu3A0ympXV/qHCeMasaXOqrmlIF/2sAMgrjYmCXcAeyplvirGPDOUPHHUIBmZzqbtmU5Ol2l9VpMEesuDneEZh8nB9dpvtNe+LpoDAX4qVvrf78SXDzT9biFwJj8AAUgYI1JA2lN/+rHYCOYTlfrn1cln3q2F1sbtOKfJyYdt5PsbALI2In3b134k4XP93W5fLqNSFHbG3LcWTLkU06/cobg8etttjyyg5svUAEN+LnhtfrGilLW67oi4vHnjzhggEy7zo2RGfs2PJ8CnwlmAOGGtN/DaPTjobeHZRrIsIWy9/SPpSozaUV/mNxkrvYFEgE0BP6KCgS7HVXcJbsOcNIKIdUhRgRkXKT5XF7wakw9SjD3BCNZRIbfruBbN/dUx0jHgdU1zLJ1gVQcE0P/Fyrubq6VcKSTLrhygz2CkRSqUmE9MVmbISmDv13cI/lg/sTbEEpxWF+6lZdxmts5GVxjvTLbbv0CglRu8SyYGycWtHkSYsVEKYwBV5DRXfEWN8/uJcgrWxYNKH8+1nld/RSKVQ2lYKK2b0cJF4OHuhNGubNDDUn99LZviQmNQAzaK4hTFtRGaTVhcMOgl6KdEafQ6/oy9l9ynk+dw9HUJSq521ef1tFEvFYp3jNIjG8fcMikr5XuOoETaLFsdBNRPqMGQm7BuRzc8=</Modulus><Exponent>AQAB</Exponent><P>0izEpTNE2NV+nxhotvIqDEmKrYe9NlHqjvcr4uqbDTDem+Ci84aOnxwSeVF/zgL2svqvU+XiyT58EbxcHWpByhtbl2GleqBPhdLgkwAJhMkauVMzz4dQ9E0JPX3J2nHAuE0XGLl+wZ+o+ZFg8X9JVF0VrJ/V8e/HSWuomIexF8O8nE5SmhX6CqBPhJj6tMYTRv3gGa8cJ2VMwv0QFgk5YLAV5LCwGSLSQ9gIXFQVu9NUWNnbqhHj4N4r3O8fm+X3bsWkf/PdqB114bPtCv7hp6exD/f8p2JhP4/E88U8RvaaZsnVVNjsLVTyIDfXUeZihdXYObhZLjAnYiTSrcKeew==</P><Q>wg5fQACeKLxBwsGeq9qWuGhlFDdwWf5iui0s1m2bt87imrUbAsvZL+QHcY8jMvw0MOVGEGVxX9JA9T8ug5KVOS+ofJvibezAgmhZl5XbMJY3l/sjvHCdmoDdZetaw/lNuBqAMk567gCN/+evMt2dVJWBIyIsyt3dFQsA+r2Mk5ZJxJzPbXppoSr0fHfEtclmWMNI97VhXzLoxCvJBtAl3bmHctp0HR+uH8kjwKitXsVVQ8UHasFy65IT+Z4c1jt1crotK4rSgUo6vhpLjeiUd9sX8IsXmDk0mY5HFXaN5EqF7wSo/MCTRTNx2KbwCjl9cPyBr8tWj+QDrjHIUeVXvQ==</Q><DP>zjyZ1hWiCDgvIQS9tE+bDSWZDED3XXcyeIl4qhlWfrImrsTWgarXBrBwPFXJ2Ki11dkB9IzPZnSHIIw5w6+B0UXZMYni7Jqkjgfo0LanoIIKVDKd05XPzXpOh+WIDm+zEeartFpJVMxL7mFGxJMHrN4Op67MLLUCVDxtWwdDsrMiwCpnCcZo7sZyYQYQdRUs02vJ3MolEU9o7KmQgF8ay5LeWOM8Wd3+gA5b3eWw0fdEfE+DKraVaxH37rtCxCL8EtmkWt488nu+MfTxtOl5GqAFskrAxKtYDBwSwrYXOPdBeX2ydajK0Izbbtv80OQGZ5f4rmMEN7uO2dKSXWltQw==</DP><DQ>GiPNeNWceGhDg3SJZyTewKBvXTXKkJTPv7xuGcRSAYSAyc4zgUDsVKMmzYk2eJu4fA2mTncbuoib720/WsHYEAf3bjGhYqVNmUNtLholmHnjqzlNKwkQccuCB0SYyWU/rtkDA8PGk2DHv/z5gKSRmN86sfzcg8c3DKqayyvVT9wiu7VTy699oxQiMtH/UW17t+E2ZwerwiMdb69mOOC4+REQycvbcEDgN6/kfQM7t1Rlk+dqhFrinBDlV+6Qe9suivHBO+hLStcw6oKoQsldlneQ1fomh37NMxITSTTbEDFpsTSzfriCHgQ8Ba8XDomH+DxLS97cHi3cwQ47qax3EQ==</DQ><InverseQ>VUC9weP0nmDNwJ/tB5ezEDtik012Zr/HqkIqszaIgWvPaVGWhASSA/qpNGJwgeSLNC0i/FZX6gsxr9dQ3fWGZC+c2dxPV2T9+DW2cNkLMDbvsf4YK5ZtmAza+49rhN8zKETZkqkGGA2KRsP6qRaLRzJw5+7JgpkuPrVMcIGlt4m5JR2QyBab31Z5mmCyQeNAG9DCRdqU3VreKb/6aNNGY5wQ4Rs0ObEn39CqUuA0DuKMZKzyLcPyR8fR/Nf2cEzTIm5fTH5Zfx2rGdGbalHXSkAxMUOy+i2WynNVjPiXsSg0CGY+vO+cb+VnnXR5Wy148901IHH0NN9DdzcyaSeuBQ==</InverseQ><D>DPcZpP2vAOC/uiiuIDu8A9ImLANUWes6eKHDZtsTTvz7OQl3vSllWBd50aT21JXPela2eyQwmsolXv0lAiBqrShfgdnLpyi9z9JXuM9M/paqnAzeRZLW8i5cJ8PAVh5R1JxTgDSf0g0BAEtm0GFqLZ7COkFrwQ8Lv8KSj9/eiW2KGYp2xH/tM1lOn6bk92qMQnoS8X6DYsYiTMWpZOvm0agX0iwk97mlZfoqWwx/kojeKTIcT0M3RCagzTxhiiZOHrn4xldmz1bXt7zSi4Jjp2BM5BPbAGHQw9aiV2QZRW8fzS3TzQwuIeg7njTA2jIW4GdD97R2xop+PGgqGJmkcbj2/r6AqWuDdCcb/Xggs+dNlH7XYzxooz0MPXz6jeyl0r7ScFl00SQ51cNTV7F2Euq3a6oj9Qwu7FV3pWgrWp+FmyU0m6ikb3YAUWDlA6riQU5XU79sarVB7sussXFVP6ONYNPwU1rldkMavPgF/KUypXfm8EYVre+Hu1C5KEgmp/Y0QRdo+mTN5zzDwvYvoXZVtyyFvqAu2jTEt1TFeS0oybJtGLk3Jm9/vtMz6JjD8vpd2+yIRqN+6NCnFVP58Ss/wek5Yy0X3+B8xQUk8QYR95KUu9GnafNrCgeqpd2qOw/39NwzVp5yIqw2fckJzE2N5dDQcN62DA5GEFO1NAE=</D></RSAKeyValue>";
        private const string PublicKey = "<RSAKeyValue><Modulus>pFLs/4KQ1sQy5EQxPlGNpwD3/aP52csVtFTMLmR0vKaoAD0CjggLxq0v14R/idIc1rN7KYA+Yv3ZqTHchTCx5DIhMX5f31uPNWvrKxbw7mmOjL2/cgP5KI399pbcBse/3sfOiPMgzZPaKuFI4x/cET03WJoG2Uexwu4/pZHVVBiCkqOSI4pUGxhR4ZsCE+sFczhy6HhHNH29wRzTKL7dZpERobZlcgjWAxyX6iPS7emnbuxdVfES2r/X5uW812KMPDaSEW0NYK2mQ/cnZtdQp1TbiOv/NsFaTlDlOKlbKAwmq/bJZoyfqi8U0Z00B0iWypXL3O9fbodNNFZDftq/4flpsiRxrEqqeYp3zsAaewdgtvmha0rs90xH08Vhgi+i+xesvxLR/pxL5iw3x2dE6PMmLkCrh0T2sNqauqE2LEKANo2r29Nr83aa9X8Cs+Oqe/y30xEInWcF7iKMQCiaRfWORuxdgjaSXybGkbEe2iDhK8X8aTbkq7xKGSzmP53FxDi0U2kkIRZoCnrkk54MYPDAgPR0lWOOq9Cc9A5iipOTyMiioNP+d7OH7/97EwEC2sIC2daYA3kwA0mrTB2GV1wCSOBgZGY/j2zmSfonPXH8YitLWj++vlM3XOz0Fd73G9fRLHk84zNopxXH2xbom2j8LItaHP4SxRwFN0IQQuk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private TcpListener FServer;
        private TcpClient FClient;
        private readonly IPEndPoint FIPEndPnt = new IPEndPoint(IPAddress.Any, int.Parse(Config.ServerPort));

        private Thread WaitingThread;
        private bool ExitThreadOnPurpose;

        private bool CancelMessageHandling;
        private Queue<F2Core.Message> MessageQueue = new Queue<F2Core.Message>();

        public bool Start() {
            // Extension Management
            ExtensionManager.Extensions.Clear();
            ListenerManager.Listeners.Clear();
            foreach (var File in Directory.GetFiles(Application.StartupPath + "\\Extensions\\")) {
                try {
                    ExtensionManager.LoadExtension(File);
                } catch {
                    PrintToConsole($"[ExtensionSystem] Loading the Extension \"{File.Split('\\').Last()}\" failed.", ColorTranslator.FromHtml("#FC3539"));
                }
            }
            foreach (var Extension in ExtensionManager.Extensions) {

                // Version Check

                var ExtensionVersion = Extension.Version;
                var CoreVersion = new F2Core.Compatibility.Version();
                if (!CoreVersion.InRange(ExtensionVersion)) {
                    var RangeStart = ExtensionVersion.SupportedVersion;
                    if (ExtensionVersion.SupportedVersion == VersioningProfiler.Lowest.ToMediumString()) {
                        RangeStart = "Lowest";
                    }
                    PrintToConsole($"[{Extension.Name}] Extension skipped due to a Version Conflict.\nRequired CoreVersion: {RangeStart}\nCurrent CoreVersion: {CoreVersion.ToMediumString()}", ColorTranslator.FromHtml("#FC3539"));
                    Extension.Disabled = true;
                    continue;
                }

                // End

                PrintToConsole($"[{Extension.Name}] Enabling...", Color.DarkSlateGray);
                try {
                    Extension.OnEnable();
                    Extension.ServerListeners.ToList().ForEach(ListenerManager.RegisterListener);
                    PrintToConsole($"[{Extension.Name}] Extension enabled. Version: {Extension.Version.ToShortString()}", Color.LimeGreen);
                } catch {
                    PrintToConsole($"[{Extension.Name}] Enabling failed.", ColorTranslator.FromHtml("#FC3539"));
                }
            }
            ExtensionManager.Extensions.RemoveAll(e => e.Disabled);
            foreach (var Extension in ExtensionManager.Extensions) {
                if (Extension.StorageNeeded) {
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace)))
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace));
                    Extension.StoragePath = Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace);
                }
                Extension.OnRun();
            }
            // End

            try {
                File.Copy(Application.StartupPath + "\\database.json", Application.StartupPath + "\\database.json.bak");
                File.Copy(Application.StartupPath + "\\config.json", Application.StartupPath + "\\config.json.bak");
            } catch { }
            Connections.Clear();
            ExitThreadOnPurpose = false;
            ServiceProvider.FromXmlString(PrivateKey);
            PreServiceProvider.FromXmlString(PublicKey);
            if (Storage.Localization.Helper.Exists(Config.ServerLanguage))
                Lcl = Storage.Localization.Helper.GetLocalization(Config.ServerLanguage);
            Forestual.Id = "lnr-forestual";
            Forestual.Name = "Forestual";
            Forestual.Owner = Root;
            Forestual.Persistent = true;
            Forestual.JoinRestrictionMode = Enumerations.ChannelJoinMode.Default;
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
            PunishmentManager.DisposeExceededRecords();
            Connected?.Invoke($"{Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Find(ip => ip.AddressFamily == AddressFamily.InterNetwork)}:{Config.ServerPort}");

            // Extension Management
            ListenerManager.InvokeEvent(Event.ServerStarted, null);
            // End

            return true;
        }

        public void Stop() {
            try {
                // Extension Management
                ListenerManager.InvokeEvent(Event.ServerStopped, null);
                foreach (var Extension in ExtensionManager.Extensions) {
                    Extension.OnDisable();
                    PrintToConsole($"[{Extension.Name}] Extension disabled.", Color.DarkSlateGray);
                }
                // End

                try {
                    SendToAll(string.Join("|", Enumerations.Action.Disconnect.ToString(), Config.ServerShutdownMessage));
                    Database.Accounts.ForEach(a => a.Online = false);
                    Helper.Save();
                } catch { }
                FServer.Stop();
                ExitThreadOnPurpose = true;
                WaitingThread.Abort();
            } catch { }
        }

        private void Wait() {
            while (true) {
                try {
                    FClient = FServer.AcceptTcpClient();
                    var Connection = new Connection(FClient.GetStream());
                    var RawStreamContent = Connection.GetRawStreamContent();
                    var DRawStreamContent = Cryptography.RSADecrypt(RawStreamContent, ServiceProvider);
                    if (DRawStreamContent == Enumerations.Action.GetServerMetaData.ToString()) {
                        Connection.SetRawStreamContent(Cryptography.RSAEncrypt(JsonConvert.SerializeObject(GetMetaData()), PreServiceProvider));
                        Connection.Dispose();
                        PunishmentManager.DisposeExceededRecords();
                    } else {
                        var SessionData = DRawStreamContent.Split('|');
                        var AesData = new AesData {
                            Key = Convert.FromBase64String(SessionData[0]),
                            IV = Convert.FromBase64String(SessionData[1])
                        };
                        Connection.AesData = AesData;
                        Connection.HmacKey = Convert.FromBase64String(SessionData[2]);
                        var AuthData = Connection.GetStreamContent().Split('|');
                        if (Helper.AccountExists(AuthData[0])) {
                            var Account = Helper.GetAccount(AuthData[0]);
                            if (Connections.Any(c => c.Owner.Id == AuthData[0])) {
                                continue;
                            }
                            if (Account.Password == AuthData[1]) {
                                var PunishmentId = PunishmentManager.CheckForRecords(Account.Id, Enumerations.PunishmentType.Bann, Enumerations.PunishmentType.BannTemporarily);
                                if (PunishmentId != "-1") {
                                    var Punishment = PunishmentManager.GetRecord(PunishmentId);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetState.ToString(), Enumerations.ClientState.Banned.ToString(), JsonConvert.SerializeObject(Punishment)));
                                } else {
                                    Connection.Owner = Account;
                                    Connection.Channel = Forestual;
                                    Connections.Add(Connection);
                                    var ListeningThread = new Thread(Listen);
                                    ListeningThread.Start(Connection);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.LoginResult.ToString(), "hej"));
                                    var Flags = new List<Enumerations.Flag>();
                                    Flags.AddRange(Connection.Owner.Flags);
                                    Flags.AddRange(Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).Flags);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetFlags, JsonConvert.SerializeObject(Flags)));
                                    Forestual.MemberIds.Add(Account.Id);
                                    ChannelManager.SendChannelList();

                                    // Extension Management
                                    var ExtensionPaths = new List<string>();
                                    ExtensionManager.Extensions.FindAll(e => e.ClientInstance).ForEach(e => ExtensionPaths.Add(e.Path));
                                    ExtensionPaths.ForEach(e => Connection.SetStreamContent(string.Join("|", Enumerations.Action.ExtensionTransport.ToString(), JsonConvert.SerializeObject(File.ReadAllBytes(e)))));
                                    // End

                                    var Message = new F2Core.Message {
                                        Time = DateTime.Now.ToShortTimeString(),
                                        Type = Enumerations.MessageType.Center,
                                        Content = $"{Connection.Owner.Name} (@{Connection.Owner.Id}) hat den Chat betreten."
                                        // Demo Purposes
                                    };
                                    SendMessageToAll(Message);
                                    Connection.Owner.Online = true;
                                    Database.Accounts.Find(a => a.Id == Connection.Owner.Id).Online = true;
                                    SendTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.SetRankList, JsonConvert.SerializeObject(Database.Ranks)));
                                    SendToAll(string.Join("|", Enumerations.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                                    PrintToConsole($"{Connection.Owner.Name} (@{Connection.Owner.Id}) joined. <{((IPEndPoint) FClient.Client.RemoteEndPoint).Address}>", ColorTranslator.FromHtml("#07D159"));

                                    RefreshAccounts?.Invoke();

                                    // Extension Management
                                    ListenerManager.InvokeEvent(Event.ClientConnected, Connection.Owner.Id);
                                    // End
                                }
                            } else {
                                Connection.SetStreamContent(string.Join("|", Enumerations.Action.LoginResult, "authentificationFailed"));
                            }
                        } else {
                            Connection.SetStreamContent(string.Join("|", Enumerations.Action.LoginResult, "accountUnknown"));
                        }
                    }
                } catch { }
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

                    var PunishmentId = PunishmentManager.CheckForRecords(Connection.Owner.Id, Enumerations.PunishmentType.Mute);
                    if (PunishmentId != "-1") {
                        var RemainingTime = PunishmentManager.GetRecord(PunishmentId).EndDate.Subtract(DateTime.Now);
                        var Message = new F2Core.Message {
                            Time = DateTime.Now.ToShortTimeString(),
                            Type = Enumerations.MessageType.Center,
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
                        var Type = (Enumerations.Action) Enum.Parse(typeof(Enumerations.Action), Contents[0]);
                        switch (Type) {
                        case Enumerations.Action.Plain:

                            // Extension Management
                            ListenerManager.InvokeEvent(Event.ClientMessageReceived, Connection.Owner.Id, Contents[1]);
                            if (CancelMessageHandling) {
                                CancelMessageHandling = false;
                                continue;
                            }
                            // End

                            if (Contents[1].StartsWith("/")) {
                                var CommandParts = Contents[1].Remove(0, 1).Split(' ');
                                switch (CommandParts[0]) {
                                case "versions":
                                    var Message0 = new F2Core.Message {
                                        Time = DateTime.Now.ToShortTimeString(),
                                        Type = Enumerations.MessageType.Left,
                                        RankColor = "#1E90FF",
                                        SenderId = "server",
                                        SenderPrefix = "[Server] Forestual 2",
                                        Content = $"Forestual 2 Server<br /><small>Version { new Version().ToLongString() }</small><br /><br />Forestual 2 Core<br /><small>Version { new F2Core.Compatibility.Version().ToLongString() }</small>"
                                    };
                                    SendMessageTo(Connection.Owner.Id, Message0);
                                    break;
                                case "clear":
                                    SendToChannel(Connection.Channel.Id, Enumerations.Action.ClearConversation.ToString());
                                    break;
                                case "vuohen":
                                    var Account = CommandParts.Length >= 3 ? Helper.GetAccount(CommandParts[2]) : null;
                                    var MyAccount = Helper.GetAccount(Connection.Owner.Id);
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
                                        if (MyAccount.Deposit < Value) {
                                            var Message1 = new F2Core.Message {
                                                Time = DateTime.Now.ToShortTimeString(),
                                                Type = Enumerations.MessageType.Center,
                                                RankColor = "#FC3539",
                                                Content = "Your balance isn't high enough to perform this transaction."
                                            };
                                            SendMessageTo(Connection.Owner.Id, Message1);
                                        } else {
                                            MyAccount.Deposit -= Value;
                                            Account.Deposit += Value;
                                        }
                                        break;
                                    case "balance":
                                        var Message2 = new F2Core.Message {
                                            Time = DateTime.Now.ToShortTimeString(),
                                            Type = Enumerations.MessageType.Center,
                                            RankColor = Config.ServerBroadcastColor,
                                            Content = $"Your balance is {MyAccount.Deposit}."
                                        };
                                        SendMessageTo(Connection.Owner.Id, Message2);
                                        break;
                                    }
                                    Helper.Save();
                                    Database = Helper.GetDatabase();
                                    break;
                                }
                            } else {
                                var Message3 = new F2Core.Message {
                                    SenderId = Connection.Owner.Id,
                                    SenderPrefix = ComposePrefix(Connection.Owner.Id),
                                    RankColor = Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).Color,
                                    Time = DateTime.Now.ToShortTimeString(),
                                    Content = Contents[1],
                                    Type = Enumerations.MessageType.Left
                                };
                                SendMessageToAllExceptTo(Message3.SenderId, Message3, Connection.Channel.Id);
                                Message3.Type = Enumerations.MessageType.Right;
                                SendMessageTo(Message3.SenderId, Message3);
                            }
                            break;
                        case Enumerations.Action.RegisterRecord:
                            var Punishment = JsonConvert.DeserializeObject<Punishment>(Contents[1]);
                            Punishment.CreatorId = Connection.Owner.Id;
                            Punishment.Id = PunishmentManager.GetRandomIdentifier(6);
                            PunishmentManager.CreateRecord(Punishment);
                            break;
                        case Enumerations.Action.Extension:
                            var EventArgs = JsonConvert.DeserializeObject<EventArguments>(Contents[1]);
                            EventArgs.EndpointId = Connection.Owner.Id;
                            ListenerManager.InvokeSpecialEvent(EventArgs);
                            break;
                        }

                        // Message Dequeueing
                        while (MessageQueue.Count > 0) {
                            SendMessageToAll(MessageQueue.Dequeue());
                        }
                        // End
                    } catch {
                        // Syntax Error in Command
                    }
                } catch {
                    // Extension Management
                    ListenerManager.InvokeEvent(Event.ClientDisconnected, null);
                    // End

                    Connections.Remove(Connection);
                    var Message = new F2Core.Message {
                        Time = DateTime.Now.ToShortTimeString(),
                        Type = Enumerations.MessageType.Center,
                        Content = $"{Connection.Owner.Name} disconnected."
                        // Demo Purposes
                    };
                    SendMessageToAll(Message);
                    Connection.Channel.MemberIds.Remove(Connection.Owner.Id);
                    if (Connection.Channel.Owner == Connection.Owner)
                        Connection.Channel.Owner = null;
                    if (Connection.Channel.MemberIds.Count == 0 && !Connection.Channel.Persistent) {
                        Channels.Remove(Connection.Channel);
                        ChannelManager.SendChannelList();
                    }
                    Connection.Owner.Online = false;
                    Database.Accounts.Find(a => a.Id == Connection.Owner.Id).Online = false;
                    SendToAll(string.Join("|", Enumerations.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                    PrintToConsole($"{Connection.Owner.Name} (@{Connection.Owner.Id}) disconnected.", ColorTranslator.FromHtml("#FC3539"));

                    RefreshAccounts?.Invoke();

                    Connection.Dispose();
                    return;
                }
            }
        }

        public void PrintToConsole(string content, Color color, bool newLine = true) {
            ConsoleColorChanged?.Invoke(color);
            ConsoleMessageReceived?.Invoke(content, newLine);
            ConsoleColorChanged?.Invoke(SystemColors.WindowFrame);
        }

        public string ComposePrefix(string accountId) {
            var Account = Connections.Find(c => c.Owner.Id == accountId).Owner;
            var Rank = Database.Ranks.Find(r => r.Id == Account.RankId);
            return $"[{Rank.Name}] {Account.Name}";
        }

        public void SendTo(string id, string content) {
            Connections.Find(c => c.Owner.Id == id).SetStreamContent(content);
        }

        public void SendToAll(string content) {
            Connections.ForEach(c => c.SetStreamContent(content));
        }

        public void SendToChannel(string id, string content) {
            Connections.FindAll(c => c.Channel.Id == id).ForEach(c => c.SetStreamContent(content));
        }

        public void SendMessageTo(string id, F2Core.Message message) {
            Connections.Find(c => c.Owner.Id == id).SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(message)));
        }

        public void SendMessageToAll(F2Core.Message message) {
            Connections.ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(message))));
        }

        public void SendMessageToAllExceptTo(string accountId, F2Core.Message message, string channelId) {
            Connections.FindAll(c => c.Owner.Id != accountId && c.Channel.Id == channelId).ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(message))));
        }

        public void CreatePunishment(Punishment punishment) {
            punishment.CreatorId = Root.Id;
            punishment.Id = PunishmentManager.GetRandomIdentifier(6);
            PunishmentManager.CreateRecord(punishment);
        }

        public Account GetAccountById(string id) {
            return GetAccountsWithoutPassword().Find(a => a.Id == id);
        }

        public Rank GetRankById(string id) {
            return Database.Ranks.Find(r => r.Id == id);
        }

        public Punishment GetPunishmentById(string id) {
            return PunishmentManager.GetRecord(id);
        }

        public void InvokeInternalEvent(Event e, params object[] args) {
            ListenerManager.InvokeEvent(e, args);
        }

        public void InvokeEvent(EventArguments e) {
            e.EndpointId = "server";
            ListenerManager.InvokeSpecialEvent(e);
        }

        public string Serialize(object content, bool indented) {
            if (indented)
                return JsonConvert.SerializeObject(content, Formatting.Indented, new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return JsonConvert.SerializeObject(content, Formatting.None, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public T Deserialize<T>(string content) {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public void CancelInternalMessageHandling() {
            CancelMessageHandling = true;
        }

        public void DisplayForm(Form form) {
            DisplayFormEvent?.Invoke(form);
        }

        public List<string> GetAllConnectedIds() {
            var Ids = new List<string>();
            Connections.ForEach(c => Ids.Add(c.Owner.Id));
            return Ids;
        }

        public void Enqueue(F2Core.Message message) {
            MessageQueue.Enqueue(message);
        }

        private List<Account> GetAccountsWithoutPassword() {
            var Accounts = new List<Account>();
            Database.Accounts.ForEach(a => Accounts.Add(new Account { Deposit = a.Deposit, Flags = a.Flags, Id = a.Id, Name = a.Name, Online = a.Online, Password = Database.Ranks.Find(r => r.Id == a.RankId).Color, RankId = a.RankId }));
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
                RequiresAuthentification = Config.MetaRequiresAuthentification,
                RequiresInvitation = Config.MetaRequiresInvitation,
                ServerVersion = new Version(),
                ServerCoreVersion = new F2Core.Compatibility.Version().ToMediumString()
            };
            return Meta;
        }
    }
}