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
using F2Core.Extension;
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
        private Account ServerAccount = new Account();

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
        private Queue<MessagePacket> MessageQueue = new Queue<MessagePacket>();

        public bool Lockdown { get; set; }
        private bool Running;

        public bool Start() {
            Eskaemo.Trace("Eskaemo started tracing.", "ESKM");
            ExtensionManager.Extensions.Clear();
            ListenerManager.Listeners.Clear();

            foreach (var File in Directory.GetFiles(Application.StartupPath + "\\Extensions\\")) {
                try {
                    ExtensionManager.LoadExtension(File);
                } catch {
                    PrintToConsole($"[ExtensionSystem] Loading the Extension \"{File.Split('\\').Last()}\" failed.", ColorTranslator.FromHtml("#FC3539"));
                    Eskaemo.Trace($"Loading the Extension \"{File.Split('\\').Last()}\" failed.", "EXTS");
                }
            }

            foreach (var Extension in ExtensionManager.Extensions) {
                var ExtensionVersion = Extension.Version;
                var CoreVersion = new F2Core.Compatibility.Version();
                if (!CoreVersion.InRange(ExtensionVersion)) {
                    var RangeStart = ExtensionVersion.SupportedVersion;

                    if (ExtensionVersion.SupportedVersion == VersioningProfiler.Lowest.ToMediumString()) {
                        RangeStart = "Lowest";
                    }

                    PrintToConsole($"[{Extension.Name}] Extension skipped due to a Version Conflict.\nRequired CoreVersion: {RangeStart}\nCurrent CoreVersion: {CoreVersion.ToMediumString()}", ColorTranslator.FromHtml("#FC3539"));
                    Eskaemo.Trace($"Skipped \"{Extension.Name}\" due to a version conflict. Required CoreVersion: {RangeStart}. Current CoreVersion: {CoreVersion.ToMediumString()}.", "EXTS");
                    Extension.Disabled = true;
                    continue;
                }

                PrintToConsole($"[{Extension.Name}] Enabling...", Color.DarkSlateGray);

                try {
                    Extension.OnEnable();
                    Extension.ServerListeners.ToList().ForEach(ListenerManager.RegisterListener);
                    PrintToConsole($"[{Extension.Name}] Extension enabled. Version: {Extension.Version.ToShortString()}", Color.LimeGreen);
                    Eskaemo.Trace($"Extension \"{Extension.Name}\" (Version {Extension.Version.ToShortString()}) enabled.", "EXTS");

                } catch {
                    PrintToConsole($"[{Extension.Name}] Enabling failed.", ColorTranslator.FromHtml("#FC3539"));
                    Eskaemo.Trace($"Enabling the extension \"{Extension.Name}\" failed.", "EXTS");
                }
            }

            ExtensionManager.Extensions.RemoveAll(e => e.Disabled);

            foreach (var Extension in ExtensionManager.Extensions) {
                if (Extension.StorageNeeded) {
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace))) {
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace));

                        Eskaemo.Trace($"Extension \"{Extension.Name}\" created \"{Extension.Namespace}\".", "EXTS");

                    }
                    Extension.StoragePath = Path.Combine(Application.StartupPath, "Extensions", Extension.Namespace);
                }
                Extension.OnRun();
            }

            try {
                File.Copy(Application.StartupPath + "\\database.json", Application.StartupPath + "\\database.json.bak");
                File.Copy(Application.StartupPath + "\\config.json", Application.StartupPath + "\\config.json.bak");
            } catch { }

            Connections.Clear();
            ExitThreadOnPurpose = false;
            ServiceProvider.FromXmlString(PrivateKey);
            PreServiceProvider.FromXmlString(PublicKey);

            if (Storage.Localization.Helper.Exists(Config.ServerLanguage)) {
                Lcl = Storage.Localization.Helper.GetLocalization(Config.ServerLanguage);
            }

            ServerAccount.Id = "Server";
            Forestual.Id = "forestual";
            Forestual.Name = "Forestual";
            Forestual.OwnerId = ServerAccount.Id;
            Forestual.Capacity = -1;
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
            Eskaemo.Trace($"Forestual 2 Server started on {Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Find(ip => ip.AddressFamily == AddressFamily.InterNetwork)}:{Config.ServerPort}.", "SRVR");
            ListenerManager.InvokeEvent(Event.ServerStarted, null);
            Running = true;
            return true;
        }

        public void Stop() {
            try {
                if (Running) {
                    Eskaemo.Trace("Eskaemo stopped tracing.", "ESKM");
                    Eskaemo.EndSession();
                }

                ListenerManager.InvokeEvent(Event.ServerStopped, null);
                foreach (var Extension in ExtensionManager.Extensions) {
                    Extension.OnDisable();
                    PrintToConsole($"[{Extension.Name}] Extension disabled.", Color.DarkSlateGray);
                }

                try {
                    SendPacketToAll(string.Join("|", Enumerations.Action.Disconnect, Config.ServerShutdownMessage));
                    Database.Accounts.ForEach(a => a.Online = false);
                    Helper.Save();
                } catch { }

                Running = false;
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
                    var DRawStreamContent = Cryptography.RSADecrypt(RawStreamContent, ServiceProvider).Split('|');

                    if (DRawStreamContent[0] == Enumerations.Action.GetServerMetaData.ToString()) {
                        Eskaemo.Trace($"{((IPEndPoint) FClient.Client.RemoteEndPoint).Address} requested meta data.", "NTWK");

                        Connection.SetRawStreamContent(Cryptography.RSAEncrypt(JsonConvert.SerializeObject(GetMetaData()), PreServiceProvider));

                        Connection.Dispose();
                        PunishmentManager.DisposeExceededRecords();
                    } else {
                        Eskaemo.Trace($"{((IPEndPoint) FClient.Client.RemoteEndPoint).Address} started the login procedure.", "NTWK");

                        var AesData = new AesData {
                            Key = Convert.FromBase64String(DRawStreamContent[0]),
                            IV = Convert.FromBase64String(DRawStreamContent[1])
                        };

                        Connection.AesData = AesData;
                        Connection.HmacKey = Convert.FromBase64String(DRawStreamContent[2]);

                        var AuthData = Connection.GetStreamContent().Split('|');
                        var AccountIdData = AuthData[0].Split(':').ToList();
                        AccountIdData.Add("offset");

                        ListenerManager.InvokeEvent(Event.ConnectionEstablished, ((IPEndPoint) FClient.Client.RemoteEndPoint).Address.ToString(), AccountIdData[0], AccountIdData[1], AuthData[1]);

                        if (Helper.AccountExists(AccountIdData[0])) {

                            var Account = Helper.GetAccount(AccountIdData[0]);

                            if (Connections.Any(c => c.Owner.Id == AccountIdData[0].ToString())) {
                                continue;
                            }

                            if (Account.Password == AuthData[1]) {

                                ListenerManager.InvokeEvent(Event.ClientConnect, ((IPEndPoint) FClient.Client.RemoteEndPoint).Address.ToString(), AccountIdData[0], AccountIdData[1]);

                                var PunishmentId = PunishmentManager.CheckForRecords(Account.Id, Enumerations.PunishmentType.Bann, Enumerations.PunishmentType.BannTemporarily);

                                if (PunishmentId != "-1") {
                                    var Punishment = PunishmentManager.GetRecord(PunishmentId);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetState, Enumerations.ClientState.Banned.ToString(), JsonConvert.SerializeObject(Punishment)));
                                } else {
                                    Connection.Owner = Account;
                                    Connection.Channel = Forestual;
                                    Connections.Add(Connection);
                                    var ListeningThread = new Thread(Listen);
                                    ListeningThread.Start(Connection);
                                    Connection.SessionId = PunishmentManager.GetRandomIdentifier(16);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.LoginResult, "hej", Connection.SessionId, Connection.Owner.Id));
                                    var LuvaValues = new List<string>();
                                    LuvaValues.AddRange(Connection.Owner.LuvaValues);
                                    LuvaValues.AddRange(Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).LuvaValues);
                                    Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetLuvaValues, JsonConvert.SerializeObject(LuvaValues)));
                                    Forestual.MemberIds.Add(Account.Id);
                                    ChannelManager.SendChannelList();
                                    var ExtensionPaths = new List<string>();
                                    ExtensionManager.Extensions.FindAll(e => e.ClientInstance).ForEach(e => ExtensionPaths.Add(e.Path));
                                    ExtensionPaths.ForEach(e => Connection.SetStreamContent(string.Join("|", Enumerations.Action.ExtensionTransport, JsonConvert.SerializeObject(File.ReadAllBytes(e)))));

                                    if (!CancelMessageHandling) {
                                        var Message = new MessagePacket {
                                            Time = DateTime.Now.ToShortTimeString(),
                                            Type = Enumerations.MessageType.Center,
                                            Content = $"{Connection.Owner.Name} (@{Connection.Owner.Id}) hat den Chat betreten."
                                        };

                                        SendMessagePacketToAll(Message);
                                        Connection.Owner.Online = true;
                                        Database.Accounts.Find(a => a.Id == Connection.Owner.Id).Online = true;
                                    }
                                    CancelMessageHandling = false;

                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.SetRankList, JsonConvert.SerializeObject(Database.Ranks)));
                                    SendPacketToAll(string.Join("|", Enumerations.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                                    PrintToConsole($"{Connection.Owner.Name} (@{Connection.Owner.Id}) joined. <{((IPEndPoint) FClient.Client.RemoteEndPoint).Address}>", ColorTranslator.FromHtml("#07D159"));
                                    Eskaemo.Trace($"Connection from {((IPEndPoint) FClient.Client.RemoteEndPoint).Address} assigned to \"{Connection.Owner.Id}\".", "NTWK");
                                    Eskaemo.TraceIndented($"SessionId: {Connection.SessionId}");
                                    RefreshAccounts?.Invoke();
                                    ListenerManager.InvokeEvent(Event.ClientConnected, Connection.Owner.Id);
                                }

                            } else {
                                Eskaemo.Trace($"{((IPEndPoint) FClient.Client.RemoteEndPoint).Address} pretended to be \"{AuthData[0]}\" but didn't provided the correct password.", "NTWK");
                                Eskaemo.TraceIndented($"Connection from {((IPEndPoint) FClient.Client.RemoteEndPoint).Address} refused.");
                                Connection.SetStreamContent(string.Join("|", Enumerations.Action.LoginResult, "authentificationFailed"));
                            }

                        } else {
                            Eskaemo.Trace($"{((IPEndPoint) FClient.Client.RemoteEndPoint).Address} pretended to be \"{AuthData[0]}\" but this account is unknown.", "NTWK");
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
                    if (Connection.Disposable) {
                        throw new Exception();
                    }

                    var Packet = Connection.GetStreamContent().Split('|');
                    var PunishmentId = PunishmentManager.CheckForRecords(Connection.Owner.Id, Enumerations.PunishmentType.Mute);

                    if (PunishmentId != "-1") {
                        var RemainingTime = PunishmentManager.GetRecord(PunishmentId).EndDate.Subtract(DateTime.Now);

                        var Message = new MessagePacket {
                            Time = DateTime.Now.ToShortTimeString(),
                            Type = Enumerations.MessageType.Center,
                            RankColor = "#FC3539",
                            Content = $"Your mute still lasts {RemainingTime.Days} days, {RemainingTime.Hours} hours, {RemainingTime.Minutes} minutes and {RemainingTime.Seconds} seconds."
                        };

                        SendMessagePacketTo(Connection.Owner.Id, Message);
                        continue;
                    }

                    try {
                        var Type = (Enumerations.Action) Enum.Parse(typeof(Enumerations.Action), Packet[0]);

                        switch (Type) {

                        case Enumerations.Action.Plain:
                            ListenerManager.InvokeEvent(Event.ClientMessageReceived, Connection.Owner.Id, Packet[1]);
                            if (CancelMessageHandling) {
                                CancelMessageHandling = false;
                                continue;
                            }

                            MessagePacket Result;

                            if (Packet[1].StartsWith("/")) {
                                string[] CommandParts = Packet[1].Remove(0, 1).Split(' ');
                                switch (CommandParts[0]) {
                                case "debug:versions":
                                    Result = new MessagePacket {
                                        Time = DateTime.Now.ToShortTimeString(),
                                        Type = Enumerations.MessageType.Left,
                                        RankColor = "#1E90FF",
                                        SenderId = "server",
                                        SenderPrefix = "[Server] Forestual 2",
                                        Content = $"Forestual 2 Server<br /><small>Version { new Version().ToLongString() }</small><br /><br />Forestual 2 Core<br /><small>Version { new F2Core.Compatibility.Version().ToLongString() }</small>"
                                    };

                                    SendMessagePacketTo(Connection.Owner.Id, Result);
                                    break;
                                case "debug:channel":
                                    Result = new MessagePacket {
                                        Time = DateTime.Now.ToShortTimeString(),
                                        Type = Enumerations.MessageType.Left,
                                        RankColor = "#1E90FF",
                                        SenderId = "server",
                                        SenderPrefix = "[Server] Forestual 2",
                                        Content = $"You are in Channel { Connection.Channel.Name } (#{ Connection.Channel.Id })."
                                    };

                                    SendMessagePacketTo(Connection.Owner.Id, Result);
                                    break;
                                case "lockdown":
                                    Lockdown = !Lockdown;

                                    Result = new MessagePacket {
                                        Time = DateTime.Now.ToShortTimeString(),
                                        Type = Enumerations.MessageType.Left,
                                        RankColor = "#1E90FF",
                                        SenderId = "server",
                                        SenderPrefix = "[Server] Forestual 2",
                                        Content = $"Lockdown {(Lockdown ? "enabled" : "disabled")}."
                                    };

                                    SendMessagePacketTo(Connection.Owner.Id, Result);
                                    break;
                                case "clear":
                                    SendPacketToChannel(Connection.Channel.Id, string.Join("|", Enumerations.Action.ClearConversation, ""));
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

                                            Result = new MessagePacket {
                                                Time = DateTime.Now.ToShortTimeString(),
                                                Type = Enumerations.MessageType.Center,
                                                RankColor = "#FC3539",
                                                Content = "Your balance isn't high enough to perform this transaction."
                                            };

                                            SendMessagePacketTo(Connection.Owner.Id, Result);
                                        } else {
                                            MyAccount.Deposit -= Value;
                                            Account.Deposit += Value;
                                        }
                                        break;
                                    case "balance":
                                        Result = new MessagePacket {
                                            Time = DateTime.Now.ToShortTimeString(),
                                            Type = Enumerations.MessageType.Center,
                                            RankColor = Config.ServerBroadcastColor,
                                            Content = $"Your balance is {MyAccount.Deposit}."
                                        };

                                        SendMessagePacketTo(Connection.Owner.Id, Result);
                                        break;
                                    }

                                    Helper.Save();
                                    Database = Helper.GetDatabase();
                                    break;
                                }
                            } else {
                                Result = new MessagePacket {
                                    SenderId = Connection.Owner.Id,
                                    SenderPrefix = ComposePrefix(Connection.Owner.Id),
                                    RankColor = Database.Ranks.Find(r => r.Id == Connection.Owner.RankId).Color,
                                    Time = DateTime.Now.ToShortTimeString(),
                                    Content = Packet[1],
                                    Type = Enumerations.MessageType.Left
                                };

                                SendMessagePacketToChannelExceptTo(Result.SenderId, Connection.Channel.Id, Result);
                                Result.Type = Enumerations.MessageType.Right;
                                SendMessagePacketTo(Result.SenderId, Result);
                            }
                            break;
                        case Enumerations.Action.RegisterRecord:
                            var Punishment = JsonConvert.DeserializeObject<Punishment>(Packet[1]);
                            Punishment.CreatorId = Connection.Owner.Id;
                            Punishment.Id = PunishmentManager.GetRandomIdentifier(6);
                            PunishmentManager.CreateRecord(Punishment);
                            break;
                        case Enumerations.Action.Extension:
                            var EventArgs = JsonConvert.DeserializeObject<EventArguments>(Packet[1]);
                            EventArgs.EndpointId = Connection.Owner.Id;
                            ListenerManager.InvokeSpecialEvent(EventArgs);
                            break;
                        case Enumerations.Action.GetAccountData:
                            var AccountName = Packet[1];
                            var AccountId = Helper.GetAccountId(AccountName);
                            var Avatar = File.Exists(Path.Combine(Application.StartupPath, $"Resources\\Avatars\\{AccountId}.png")) ? File.ReadAllBytes(Path.Combine(Application.StartupPath, $"Resources\\Avatars\\{AccountId}.png")) : File.ReadAllBytes(Path.Combine(Application.StartupPath, "Resources\\Avatars\\default.png"));
                            var Header = File.Exists(Path.Combine(Application.StartupPath, $"Resources\\Headers\\{AccountId}.png")) ? File.ReadAllBytes(Path.Combine(Application.StartupPath, $"Resources\\Headers\\{AccountId}.png")) : File.ReadAllBytes(Path.Combine(Application.StartupPath, "Resources\\Headers\\default.png"));
                            var SelectedAccount = Database.Accounts.Find(a => a.Id == AccountId);
                            var Online = SelectedAccount.Online;
                            var Editable = Connection.Owner.Id == AccountId;
                            var Rank = Database.Ranks.Find(r => r.Id == SelectedAccount.RankId).Name;
                            var Money = SelectedAccount.Deposit.ToString();
                            var LastSeen = Online ? Connections.Find(c => c.Owner.Id == AccountId).Channel.Name : "Offline";
                            SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.SetAccountData, JsonConvert.SerializeObject(Avatar), JsonConvert.SerializeObject(Header), Online.ToString(), AccountName, Editable.ToString(), Rank, Money, LastSeen));
                            break;
                        case Enumerations.Action.SendLuvaNotice:
                            SendLuvaNotice(Packet[1], Connection);
                            break;
                        case Enumerations.Action.CreateChannel:
                            var ChannelToCreate = JsonConvert.DeserializeObject<Channel>(Packet[1]);
                            if (Connection.Owner.AccountHasLuvaValue("forestual.canCreateChannels")) {
                                if (Channels.Any(c => c.Id == ChannelToCreate.Id)) {
                                    // Channel exists
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "exists"));
                                } else {
                                    ChannelToCreate.OwnerId = Connection.Owner.Id;
                                    ChannelToCreate.Persistent = false;
                                    ChannelManager.CreateChannel(ChannelToCreate);
                                }
                            } else {
                                SendLuvaNotice("forestual.canCreateChannels", Connection);
                            }
                            break;
                        case Enumerations.Action.TryChannelJoin:
                            var ChannelToJoin = Channels.Find(c => c.Id == Packet[1]);

                            if (ChannelToJoin != null) {

                                if (ChannelToJoin == Connection.Channel) {
                                    // Already joined
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "redundant"));
                                } else if (ChannelToJoin.MemberIds.Count == ChannelToJoin.Capacity && !Connection.Owner.AccountHasLuvaValue("forestual.exceedChannelLimit")) {
                                    // Channel full
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "full"));
                                } else if (ChannelToJoin.JoinRestrictionMode == Enumerations.ChannelJoinMode.Protected && Packet[2] == "") {
                                    // No Password
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "authRequired"));
                                } else if (ChannelToJoin.JoinRestrictionMode == Enumerations.ChannelJoinMode.Protected && Packet[2] != ChannelToJoin.Predicate) {
                                    // Wrong Password
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "authFailed"));
                                } else if (ChannelToJoin.JoinRestrictionMode == Enumerations.ChannelJoinMode.Ranked && Connection.Owner.RankId != ChannelToJoin.Predicate) {
                                    // Wrong Rank
                                    SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "ranked"));
                                } else {
                                    ChannelManager.MoveAccountTo(Connection.Owner, ChannelToJoin);
                                }

                            } else {
                                // Channel doesnt exist
                                SendPacketTo(Connection.Owner.Id, string.Join("|", Enumerations.Action.ChannelJoinResult, "unknown"));
                            }

                            break;
                        case Enumerations.Action.CloseChannel:
                            var ChannelToClose = Channels.Find(c => c.Id == Packet[1]);

                            if (ChannelToClose != null && (Connection.Owner.AccountHasLuvaValue("forestual.canCloseChannels") || ChannelToClose.OwnerId == Connection.Owner.Id)) {
                                ChannelManager.CloseChannel(ChannelToClose);
                            } else {
                                SendLuvaNotice("forestual.canCloseChannels", Connection);
                            }

                            break;
                        }

                        while (MessageQueue.Count > 0) {
                            SendMessagePacketToAll(MessageQueue.Dequeue());
                        }

                    } catch {
                        // Syntax Error in Command
                        Eskaemo.Trace($"Received a message from \"{Connection.Owner.Id}\" that couldn't be interpreted!", "EXCP");
                        Eskaemo.TraceIndented($"Content: {Packet}");
                    }
                } catch {
                    ListenerManager.InvokeEvent(Event.ClientDisconnected, null);
                    Connections.Remove(Connection);

                    var Message = new MessagePacket {
                        Time = DateTime.Now.ToShortTimeString(),
                        Type = Enumerations.MessageType.Center,
                        Content = $"{Connection.Owner.Name} disconnected."
                    };

                    SendMessagePacketToAll(Message);
                    Connection.Channel.MemberIds.Remove(Connection.Owner.Id);

                    if (Connection.Channel.OwnerId == Connection.Owner.Id) {
                        Connection.Channel.OwnerId = ServerAccount.Id;
                    }

                    if (Connection.Channel.MemberIds.Count == 0 && !Connection.Channel.Persistent) {
                        Channels.Remove(Connection.Channel);
                        ChannelManager.SendChannelList();
                    }

                    Connection.Owner.Online = false;
                    Database.Accounts.Find(a => a.Id == Connection.Owner.Id).Online = false;
                    SendPacketToAll(string.Join("|", Enumerations.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
                    PrintToConsole($"{Connection.Owner.Name} (@{Connection.Owner.Id}) disconnected.", ColorTranslator.FromHtml("#FC3539"));
                    Eskaemo.Trace($"Connection to \"{Connection.Owner.Id}\" lost.", "NTWK");
                    Eskaemo.TraceIndented("Connection disposed.");
                    RefreshAccounts?.Invoke();
                    Connection.Dispose();
                    return;
                }
            }
        }

        private void SendLuvaNotice(string luvaValue, Connection connection) {
            var Severity = Luva.GetSeverity(luvaValue);
            Eskaemo.Trace($"Noticed violation from \"{connection.Owner.Id}\".", "LUVA");
            Eskaemo.TraceIndented($"Encountered action: {luvaValue}");
            Eskaemo.TraceIndented($"Severity: {Severity.Description}");
            SendPacketTo(connection.Owner.Id, string.Join("|", Enumerations.Action.ShowLuvaNotice, luvaValue, JsonConvert.SerializeObject(Severity)));
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

        public void SendPacketTo(string id, string packet) {
            Connections.Find(c => c.Owner.Id == id).SetStreamContent(packet);
        }

        public void SendPacketToAll(string packet) {
            Connections.ForEach(c => c.SetStreamContent(packet));
        }

        public void SendPacketToChannel(string channelId, string packet) {
            Connections.FindAll(c => c.Channel.Id == channelId).ForEach(c => c.SetStreamContent(packet));
        }

        public void SendPacketToAllExceptTo(string exceptId, string packet) {
            Connections.ForEach(c => c.SetStreamContent(packet));
        }

        public void SendPacketToChannelExceptTo(string exceptId, string channelId, string packet) {
            Connections.FindAll(c => c.Owner.Id != exceptId && c.Channel.Id == channelId).ForEach(c => c.SetStreamContent(packet));
        }

        public void SendMessagePacketTo(string id, MessagePacket packet) {
            Connections.Find(c => c.Owner.Id == id).SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(packet)));
        }

        public void SendMessagePacketToAll(MessagePacket packet) {
            Connections.ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(packet))));
        }

        public void SendMessagePacketToAllExceptTo(string exceptId, MessagePacket packet) {
            Connections.FindAll(c => c.Owner.Id != exceptId).ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(packet))));
        }

        public void SendMessagePacketToChannelExceptTo(string exceptId, string channelId, MessagePacket packet) {
            Connections.FindAll(c => c.Owner.Id != exceptId && c.Channel.Id == channelId).ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.Plain, JsonConvert.SerializeObject(packet))));
        }

        public void CreatePunishment(Punishment punishment) {
            punishment.CreatorId = ServerAccount.Id;
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
            return JsonConvert.SerializeObject(content, indented ? Formatting.Indented : Formatting.None, new JsonSerializerSettings {
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

        public void Enqueue(MessagePacket message) {
            MessageQueue.Enqueue(message);
        }

        public bool AccountHasLuvaValue(string accountId, string luvaValue) {
            return Database.Accounts.Find(a => a.Id == accountId).AccountHasLuvaValue(luvaValue);
        }

        public void SendLuvaNoticeTo(string accountId, string luvaValue) {
            SendLuvaNotice(luvaValue, Connections.Find(c => c.Owner.Id == accountId));
        }

        public void RegisterSeverity(string luvaValue, int severityLevel) {
            if (!Luva.ExtensionSeverities.ContainsKey(luvaValue)) {
                Luva.ExtensionSeverities.Add(luvaValue, severityLevel);
            }
        }

        public void DisposeConnectionById(string accountId) {
            var Connection = Connections.Find(c => c.Owner.Id == accountId);

            if (Connection == null) {
                return;
            }

            Connection.Disposable = true;
        }

        public void SetAccountState(string accountId, bool online) {
            var Connection = Connections.Find(c => c.Owner.Id == accountId);

            if (Connection == null) {
                return;
            }

            Connection.Owner.Online = online;
            Database.Accounts.Find(a => a.Id == Connection.Owner.Id).Online = online;
            SendPacketToAll(string.Join("|", Enumerations.Action.SetAccountList, JsonConvert.SerializeObject(GetAccountsWithoutPassword())));
            RefreshAccounts?.Invoke();
        }

        private List<Account> GetAccountsWithoutPassword() {
            var Accounts = new List<Account>();
            Database.Accounts.ForEach(a => Accounts.Add(new Account { Deposit = a.Deposit, LuvaValues = a.LuvaValues, Id = a.Id, Name = a.Name, Online = a.Online, Password = Database.Ranks.Find(r => r.Id == a.RankId).Color, RankId = a.RankId }));
            return Accounts;
        }

        private ServerMetaData GetMetaData() {
            var Meta = new ServerMetaData {
                AcceptsGuests = Config.MetaAcceptsGuests,
                AcceptsRegistration = Config.MetaAcceptsRegistration,
                GuestsCanChooseName = Config.MetaGuestsCanChooseName,
                Language = Config.ServerLanguage,
                Name = Config.MetaServerName,
                OperatorWebsiteUrl = Config.MetaWebsiteUrl,
                OwnerId = Config.MetaOwnerId,
                RequiresInvitation = Config.MetaRequiresInvitation,
                IsLockdown = Lockdown,
                ServerVersion = new Version(),
                ServerCoreVersion = new F2Core.Compatibility.Version().ToMediumString()
            };

            return Meta;
        }
    }
}