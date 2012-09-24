using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SystemDot.Messaging.Channels;
using SystemDot.Messaging.Storage;
using SystemDot.Parallelism;

namespace SystemDot.Messaging.Configuration
{
    public abstract class Initialiser : Configurer
    {
        readonly List<Action> buildActions;

        protected Initialiser(List<Action> buildActions)
        {
            Contract.Requires(buildActions != null);

            this.buildActions = buildActions;
            this.buildActions.Add(Build);
        }

        protected abstract void Build();

        public IBus Initialise()
        {
            this.buildActions.ForEach(a => a());
            
            Resolve<IPersistence>().Initialise();
            Resolve<ITaskRepeater>().Start();
            
            return Resolve<IBus>();
        }

        public ChannelConfiguration OpenChannel(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            return new ChannelConfiguration(
                BuildEndpointAddress(name, GetAddress().ServerName),
                GetAddress().ServerName,
                this.buildActions);
        }

        protected abstract EndpointAddress GetAddress();
    }
}