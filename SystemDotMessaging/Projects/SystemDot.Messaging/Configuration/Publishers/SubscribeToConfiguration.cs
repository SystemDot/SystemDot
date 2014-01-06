using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Configuration.ExceptionHandling;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Hooks;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Publishing.Builders;
using SystemDot.Messaging.UnitOfWork;

namespace SystemDot.Messaging.Configuration.Publishers
{
    public class SubscribeToConfiguration : Configurer, IExceptionHandlingConfigurer
    {
        readonly SubscriptionRequestChannelSchema requestSchema;
        readonly SubscriberRecieveChannelSchema receiveSchema;

        public SubscribeToConfiguration(
            EndpointAddress subscriberAddress, 
            EndpointAddress publisherAddress, 
            MessagingConfiguration messagingConfiguration)
            : base(messagingConfiguration)
        {
            Contract.Requires(subscriberAddress != EndpointAddress.Empty);
            Contract.Requires(publisherAddress != EndpointAddress.Empty);
            Contract.Requires(messagingConfiguration != null);

            requestSchema = new SubscriptionRequestChannelSchema
            {
                PublisherAddress = publisherAddress,
                SubscriberAddress = subscriberAddress
            };

            receiveSchema = new SubscriberRecieveChannelSchema
            {
                Address = subscriberAddress,
                ToAddress = publisherAddress,
                UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<NullUnitOfWorkFactory>,
                FilterStrategy = new PassThroughMessageFilterStategy()
            };
        }

        protected internal override void Build()
        {
            Resolve<SubscriberRecieveChannelBuilder>().Build(receiveSchema);
            Resolve<SubscriptionRequestSendChannelBuilder>().Build(requestSchema);
        }

        protected override MessageServer GetMessageServer()
        {
            return requestSchema.SubscriberAddress.Server;
        }

        public SubscribeToConfiguration Sequenced()
        {
            receiveSchema.IsSequenced = true;
            return this;
        }

        public SubscribeToConfiguration WithDurability()
        {
            requestSchema.IsDurable = true;
            receiveSchema.IsDurable = true;
            return this;
        }

        public SubscribeToConfiguration WithUnitOfWork<TUnitOfWorkFactory>()
            where TUnitOfWorkFactory : class, IUnitOfWorkFactory
        {
            receiveSchema.UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<TUnitOfWorkFactory>;
            return this;
        }

        public SubscribeToConfiguration WithHook(IMessageHook<object> hook)
        {
            Contract.Requires(hook != null);

            receiveSchema.Hooks.Add(hook);
            return this;
        }

        public SubscribeToConfiguration WithHook(IMessageHook<MessagePayload> hook)
        {
            Contract.Requires(hook != null);

            receiveSchema.PreUnpackagingHooks.Add(hook);
            return this;
        }

        public SubscribeToConfiguration OnlyForMessages(IMessageFilterStrategy toFilterWith)
        {
            Contract.Requires(toFilterWith != null);

            receiveSchema.FilterStrategy = toFilterWith;
            return this;
        }

        public SubscribeToConfiguration HandleEventsOnMainThread()
        {
            receiveSchema.HandleEventsOnMainThread = true;
            return this;
        }

        public OnExceptionConfiguration<SubscribeToConfiguration> OnException()
        {
            return new OnExceptionConfiguration<SubscribeToConfiguration>(this);
        }

        public void SetContinueOnException()
        {
            receiveSchema.ContinueOnException = true;
        }

        public SubscribeToConfiguration BlockMessagesIf(bool shouldBlock)
        {
            receiveSchema.BlockMessages = shouldBlock;
            return this;
        }
    }
}