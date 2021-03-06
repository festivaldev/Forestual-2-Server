﻿using System;
using System.Linq;
using System.Windows.Forms;
using F2Core;
using F2Core.Extension;
using Forestual2ServerCS.Forms;
using Forestual2ServerCS.Internal;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Management
{
    public class ChannelManager
    {
        public static void CreateChannel(Channel channel) {
            Server.Channels.Add(channel);
            ListenerManager.InvokeEvent(Event.ChannelCreated,  channel.Id);
            MoveAccountTo(channel.OwnerId, channel.Id);
        }

        public static void CloseChannel(Channel channel) {
            MoveAllFromTo(channel.Id, "forestual");
            Server.Channels.Remove(channel);
        }

        public static void SendChannelList() {
            Server.Connections.ForEach(c => c.SetStreamContent(string.Join("|", Enumerations.Action.SetChannelList, JsonConvert.SerializeObject(Server.Channels))));
        }

        public static void MoveAccountTo(Account account, Channel channel) {
            var Connection = Server.Connections.Find(c => c.Owner == account);

            ExtensionPool.Server.SendMessagePacketToChannelExceptTo(account.Id, Connection.Channel.Id, new MessagePacket {
                Content = $"{account.Name} left the channel.",
                Time = DateTime.Now.ToShortTimeString(),
                Type = Enumerations.MessageType.Center
            });

            Connection.Channel.MemberIds.Remove(account.Id);
            channel.MemberIds.Add(account.Id);

            ExtensionPool.Server.SendMessagePacketToChannelExceptTo(account.Id, channel.Id, new MessagePacket {
                Content = $"{account.Name} entered the channel.",
                Time = DateTime.Now.ToShortTimeString(),
                Type = Enumerations.MessageType.Center
            });

            ListenerManager.InvokeEvent(Event.ClientChannelChanged,  Connection.Owner.Id, channel.Id);
            Connection.Channel = channel;
            Connection.SetStreamContent(Enumerations.Action.ClearConversation.ToString());
            Connection.SetStreamContent(string.Join("|", Enumerations.Action.SetChannel, JsonConvert.SerializeObject(channel)));
            Server.Channels.RemoveAll(c => c.MemberIds.Count == 0 && !c.Persistent);
            SendChannelList();
            Application.OpenForms.OfType<MainWindow>().ToList()[0].RefreshAccounts();
        }

        public static void MoveAccountTo(string accountId, string channelId) {
            MoveAccountTo(Server.Database.Accounts.Find(a => a.Id == accountId), Server.Channels.Find(c => c.Id == channelId));
        }

        public static void MoveAllTo(Channel channel) {
            Server.Connections.ForEach(c => MoveAccountTo(c.Owner, channel));
        }

        public static void MoveAllFromTo(Channel channelFrom, Channel channelTo) {
            Server.Connections.FindAll(c => c.Channel == channelFrom).ForEach(c => MoveAccountTo(c.Owner, channelTo));
        }

        public static void MoveAllFromTo(string channelFromId, string channelToId) {
            Server.Connections.FindAll(c => c.Channel.Id == channelFromId).ForEach(c => MoveAccountTo(c.Owner.Id, channelToId));
        }
    }
}
