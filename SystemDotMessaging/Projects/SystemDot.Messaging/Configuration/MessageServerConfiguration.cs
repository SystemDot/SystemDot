using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SystemDot.Messaging.Configuration
{
    public class MessageServerConfiguration : Configurer
    {
        readonly MessageServer messageServer;

        public MessageServerConfiguration(MessageServer messageServer)
        {
            Contract.Requires(messageServer != null);

            this.messageServer = messageServer;
        }

        public ChannelConfiguration OpenChannel(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            return new ChannelConfiguration(
                BuildEndpointAddress(name, this.messageServer.Name),
                this.messageServer.Name,
                new List<Action>());
        }
    }
}