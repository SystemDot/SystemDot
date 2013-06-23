using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Expiry;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.RequestReply.Builders;
using SystemDot.Messaging.UnitOfWork;

namespace SystemDot.Messaging.Configuration.RequestReply
{
    public class RequestReplySenderConfiguration : Configurer
    {
        readonly RequestSendChannelSchema sendSchema;
        readonly ReplyReceiveChannelSchema receiveSchema;

        public RequestReplySenderConfiguration(
            EndpointAddress address, 
            EndpointAddress recieverAddress,
            MessagingConfiguration messagingConfiguration)
            : base(messagingConfiguration)
        {
            this.sendSchema = new RequestSendChannelSchema
            {
                FilteringStrategy = new PassThroughMessageFilterStategy(),
                FromAddress = address,
                RecieverAddress = recieverAddress,
                ExpiryStrategy = new PassthroughMessageExpiryStrategy(),
                ExpiryAction = () => { },
                RepeatStrategy = EscalatingTimeRepeatStrategy.Default
            };

            this.receiveSchema = new ReplyReceiveChannelSchema
            {
                Address = address,
                ToAddress = recieverAddress,
                UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<NullUnitOfWorkFactory>
            };
        }

        protected override void Build()
        {
            Resolve<RequestSendChannelBuilder>().Build(this.sendSchema);
            Resolve<ReplyReceiveChannelBuilder>().Build(this.receiveSchema);
        }

        protected override ServerRoute GetServerPath()
        {
            return this.sendSchema.FromAddress.Route;
        }

        public RequestReplySenderConfiguration OnlyForMessages(IMessageFilterStrategy toFilterMessagesWith)
        {
            Contract.Requires(toFilterMessagesWith != null);

            this.sendSchema.FilteringStrategy = toFilterMessagesWith;
            return this;
        }

        public RequestReplySenderConfiguration WithDurability()
        {
            this.sendSchema.IsDurable = true;
            this.receiveSchema.IsDurable = true;
            return this;
        }

        public RequestReplySenderConfiguration WithMessageExpiry(IMessageExpiryStrategy strategy)
        {
            Contract.Requires(strategy != null);

            this.sendSchema.ExpiryStrategy = strategy;
            return this;
        }

        public RequestReplySenderConfiguration WithMessageExpiry(IMessageExpiryStrategy strategy, Action expiryAction)
        {
            Contract.Requires(strategy != null);
            Contract.Requires(expiryAction != null);

            this.sendSchema.ExpiryStrategy = strategy;
            this.sendSchema.ExpiryAction = expiryAction;
            return this;
        }


        public RequestReplySenderConfiguration WithMessageRepeating(IRepeatStrategy strategy)
        {
            Contract.Requires(strategy != null);

            this.sendSchema.RepeatStrategy = strategy;
            return this;
        }

        public RequestReplySenderConfiguration WithUnitOfWork<TUnitOfWorkFactory>() 
            where TUnitOfWorkFactory : class, IUnitOfWorkFactory
        {
            this.receiveSchema.UnitOfWorkRunnerCreator = CreateUnitOfWorkRunner<TUnitOfWorkFactory>;
            return this;
        }

        public RequestReplySenderConfiguration WithReceiveHook(IMessageProcessor<object, object> hook)
        {
            Contract.Requires(hook != null);

            this.receiveSchema.Hooks.Add(hook);
            return this;
        }

        public RequestReplySenderConfiguration WithSendHook(IMessageProcessor<object, object> hook)
        {
            Contract.Requires(hook != null);

            this.sendSchema.Hooks.Add(hook);
            return this;
        }
    }
}