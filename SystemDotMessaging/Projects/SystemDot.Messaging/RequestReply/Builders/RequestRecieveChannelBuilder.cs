using System.Diagnostics.Contracts;
using SystemDot.Ioc;
using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Batching;
using SystemDot.Messaging.Builders;
using SystemDot.Messaging.Caching;
using SystemDot.Messaging.Expiry;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Hooks;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.RequestReply.ExceptionHandling;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Parallelism;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.RequestReply.Builders
{
    class RequestRecieveChannelBuilder
    {
        readonly ReplyAddressLookup replyAddressLookup;
        readonly ISerialiser serialiser;
        readonly MessageHandlerRouter messageHandlerRouter;
        readonly AcknowledgementSender acknowledgementSender;
        readonly PersistenceFactorySelector persistenceFactorySelector;
        readonly ISystemTime systemTime;
        readonly ITaskRepeater taskRepeater;
        readonly ServerAddressRegistry serverAddressRegistry;

        internal RequestRecieveChannelBuilder(
            ReplyAddressLookup replyAddressLookup, 
            ISerialiser serialiser, 
            MessageHandlerRouter messageHandlerRouter, 
            AcknowledgementSender acknowledgementSender,
            PersistenceFactorySelector persistenceFactorySelector, 
            ISystemTime systemTime, 
            ITaskRepeater taskRepeater, 
            ServerAddressRegistry serverAddressRegistry)
        {
            Contract.Requires(replyAddressLookup != null);
            Contract.Requires(serialiser != null);
            Contract.Requires(messageHandlerRouter != null);
            Contract.Requires(acknowledgementSender != null);
            Contract.Requires(persistenceFactorySelector != null);
            Contract.Requires(systemTime != null);
            Contract.Requires(taskRepeater != null);
            Contract.Requires(serverAddressRegistry != null);
            
            this.replyAddressLookup = replyAddressLookup;
            this.serialiser = serialiser;
            this.messageHandlerRouter = messageHandlerRouter;
            this.acknowledgementSender = acknowledgementSender;
            this.persistenceFactorySelector = persistenceFactorySelector;
            this.systemTime = systemTime;
            this.taskRepeater = taskRepeater;
            this.serverAddressRegistry = serverAddressRegistry;
        }

        public IMessageInputter<MessagePayload> Build(RequestRecieveChannelSchema schema, EndpointAddress senderAddress)
        {
            ReceiveMessageCache messageCache = this.persistenceFactorySelector
                .Select(schema)
                .CreateReceiveCache(PersistenceUseType.RequestReceive, senderAddress);

            var startPoint = new MessageLocalAddressReassigner(serverAddressRegistry);

            MessagePipelineBuilder.Build()
                .With(startPoint)
                .ToProcessor(new SequenceOriginApplier(messageCache))
                .ToProcessor(new MessageSendTimeRemover())
                .ToProcessor(new ReceiveChannelMessageCacher(messageCache))
                .ToProcessor(new MessageAcknowledger(acknowledgementSender))
                .ToSimpleMessageRepeater(messageCache, systemTime, taskRepeater)
                .ToProcessor(new ReceiveChannelMessageCacher(messageCache))
                .Queue()
                .ToResequencerIfSequenced(messageCache, schema)
                .ToProcessor(new ReplyChannelSelector(replyAddressLookup))
                .ToProcessor(new ExceptionReplier())
                .ToProcessor(new MessageHookRunner<MessagePayload>(schema.PreUnpackagingHooks))
                .ToConverter(new MessagePayloadUnpackager(serialiser))
                .ToProcessor(new MessageFilter(schema.FilterStrategy))
                .ToProcessor(schema.UnitOfWorkRunnerCreator())
                .ToProcessor(new BatchUnpackager())
                .ToProcessor(new MessageHookRunner<object>(schema.Hooks))
                .ToEndPoint(messageHandlerRouter);

            Messenger.Send(new RequestReceiveChannelBuilt
            {
                CacheAddress = senderAddress,
                SenderAddress = senderAddress,
                ReceiverAddress = schema.Address
            });

            return startPoint;
        }
    }
}