namespace Forestual2ServerCS.Storage.Configuration
{
    public class Values
    {
        public bool ConsoleRequiresAuthentification { get; set; }
        public bool ConsoleAuthentificationTimeout { get; set; }
        public int CATimeoutTime { get; set; }
        public string ServerPort { get; set; }
        public string ServerLanguage { get; set; }
        public string ServerBroadcastColor { get; set; }
        public string ServerShutdownMessage { get; set; }
        public string MetaServerName { get; set; }
        public string MetaOwnerId { get; set; }
        public string MetaWebsiteUrl { get; set; }
        public bool MetaRequiresAuthentification { get; set; }
        public bool MetaAcceptsGuests { get; set; }
        public bool MetaGuestsCanChooseName { get; set; }
        public bool MetaAcceptsRegistration { get; set; }
        public bool MetaRequiresInvitation { get; set; }
        public bool MetaAccountsInstantlyActivated { get; set; }
    }
}
