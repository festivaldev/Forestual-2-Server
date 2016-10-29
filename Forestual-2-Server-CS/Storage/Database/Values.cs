using System.Collections.Generic;
using Forestual2Core;

namespace Forestual2ServerCS.Storage.Database
{
    public class Values
    {
        public List<Account> Accounts { get; set; }
        public List<Rank> Ranks { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Punishment> Punishments { get; set; }
    }
}
