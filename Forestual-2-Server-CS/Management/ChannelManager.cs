using Forestual2Core;
using F2CE = Forestual2Core.Enumerations;
using Forestual2ServerCS.Internal;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Management
{
    public class ChannelManager
    {
        public static void CreateChannel(Channel channel) {
            Server.Channels.RemoveAll(c => c.MemberIds.Count == 0 && !c.Persistent);
            Server.Channels.Add(channel);
            MoveAccountTo(channel.Owner, channel);
        }

        public static void SendChannelList() {
            Server.Connections.ForEach(c => c.SetStreamContent(string.Join("|", F2CE.Action.SetChannelList, JsonConvert.SerializeObject(Server.Channels))));
        }

        public static void MoveAccountTo(Account account, Channel channel) {
            var Connection = Server.Connections.Find(c => c.Owner == account);
            Connection.Channel.MemberIds.Remove(account.Id);
            channel.MemberIds.Add(account.Id);
            Connection.Channel = channel;
            Connection.SetStreamContent(string.Join("|", F2CE.Action.SetChannel, JsonConvert.SerializeObject(channel)));
            SendChannelList();
            // Inform old and new Channel
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
