using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Publishing.Builders;
using SystemDot.Messaging.UnitOfWork;

namespace SystemDot.Messaging.Configuration.Publishers
{
    public class SubscribeToConfiguration : Configurer
    {
        readonly SubscriptionRequestChannelSchema requestSchema;
        readonly SubscriberRecieveChannelSchema recieveSchema;

        public SubscribeToConfiguration(
            EndpointAddress subscriberAddress, 
            EndpointAddress publisherAddress, 
            MessagingConfiguration messagingConfiguration)
            : base(messagingConfiguration)
        {
            Contract.Requires(subscriberAddress != EndpointAddress.Empty);
            Contract.Requires(publisherAddress != EndpointAddress.Empty);
            Contract.Requires(messagingConfiguration != null);

            this.requestSchema = new SubscriptionRequestChannelSchema
            {
                PublisherAddress = publisherAddress,
                SubscriberAddress = subscriberAddress
            };

            this.recieveSchema = new SubscriberRecieveChannelSchema
            {
                Address = subscriberAddress,
                ToAddress = publisherAddress,
                UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<NullUnitOfWorkFactory>
            };
        }

        protected override void Build()
        {
            Resolve<SubscriberRecieveChannelBuilder>().Build(this.recieveSchema);
            Resolve<SubscriptionRequestChannelBuilder>().Build(this.requestSchema);
        }

        protected override ServerPath GetServerPath()
        {
            return this.requestSchema.SubscriberAddress.ServerPath;
        }

        public SubscribeToConfiguration WithDurability()
        {
            this.requestSchema.IsDurable = true;
            this.recieveSchema.IsDurable = true;
            return this;
        }

        public SubscribeToConfiguration WithUnitOfWork<TUnitOfWorkFactory>()
            where TUnitOfWorkFactory : class, IUnitOfWorkFactory
        {
            this.recieveSchema.UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<TUnitOfWorkFactory>;
            return this;
        }
    }
}