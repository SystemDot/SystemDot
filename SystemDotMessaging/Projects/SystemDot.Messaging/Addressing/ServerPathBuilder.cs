using System.Diagnostics.Contracts;

namespace SystemDot.Messaging.Addressing
{
    public class ServerPathBuilder
    {
        readonly ServerAddressRegistry serverAddressRegistry;

        public ServerPathBuilder(ServerAddressRegistry serverAddressRegistry)
        {
            Contract.Requires(serverAddressRegistry != null);
            this.serverAddressRegistry = serverAddressRegistry;
        }

        public ServerRoute Build(string server, string proxy)
        {
            return new ServerRoute(GetMessageServer(server), GetMessageServer(proxy));
        }

        MessageServer GetMessageServer(string name)
        {
            return MessageServer.Named(name, this.serverAddressRegistry.Lookup(name));
        }
    }
}