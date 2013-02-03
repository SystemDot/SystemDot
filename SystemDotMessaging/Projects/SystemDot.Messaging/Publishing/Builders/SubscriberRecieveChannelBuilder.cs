using System.Diagnostics.Contracts;
using SystemDot.Ioc;
using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Builders;
using SystemDot.Messaging.Caching;
using SystemDot.Messaging.Expiry;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport;
using SystemDot.Parallelism;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Publishing.Builders
{
    public class SubscriberRecieveChannelBuilder
    {
        readonly ISerialiser serialiser;
        readonly MessageHandlerRouter messageHandlerRouter;
        readonly IMessageReciever messageReciever;
        readonly AcknowledgementSender acknowledgementSender;
        readonly PersistenceFactorySelector persistenceFactory;
        readonly ICurrentDateProvider currentDateProvider;
        readonly ITaskRepeater taskRepeater;
        readonly IIocContainer iocContainer;

        public SubscriberRecieveChannelBuilder(
            ISerialiser serialiser, 
            MessageHandlerRouter messageHandlerRouter, 
            IMessageReciever messageReciever,
            AcknowledgementSender acknowledgementSender,
            PersistenceFactorySelector persistenceFactory, 
            ICurrentDateProvider currentDateProvider, 
            ITaskRepeater taskRepeater, 
            IIocContainer iocContainer)
        {
            Contract.Requires(serialiser != null);
            Contract.Requires(messageHandlerRouter != null);
            Contract.Requires(messageReciever != null);
            Contract.Requires(acknowledgementSender != null);
            Contract.Requires(persistenceFactory != null);
            Contract.Requires(currentDateProvider != null);
            Contract.Requires(taskRepeater != null);
            Contract.Requires(iocContainer != null);
            
            this.serialiser = serialiser;
            this.messageHandlerRouter = messageHandlerRouter;
            this.messageReciever = messageReciever;
            this.acknowledgementSender = acknowledgementSender;
            this.persistenceFactory = persistenceFactory;
            this.currentDateProvider = currentDateProvider;
            this.taskRepeater = taskRepeater;
            this.iocContainer = iocContainer;
        }

        public void Build(SubscriberRecieveChannelSchema schema)
        {
            MessageCache messageCache = this.persistenceFactory
                .Select(schema)
                .CreateCache(PersistenceUseType.SubscriberReceive, schema.Address);

            MessagePipelineBuilder.Build()
                .With(this.messageReciever)
                .ToProcessor(new MessagePayloadCopier(this.serialiser))
                .ToProcessor(new BodyMessageFilter(schema.Address))
                .ToProcessor(new MessageSendTimeRemover())
                .ToProcessor(new StartSequenceApplier(messageCache))
                .ToSimpleMessageRepeater(messageCache, this.currentDateProvider, this.taskRepeater)
                .ToProcessor(new MessagePayloadCopier(this.serialiser))
                .ToProcessor(new ReceiveChannelMessageCacher(messageCache))
                .ToProcessor(new MessageAcknowledger(this.acknowledgementSender))
                .Queue()
                .ToResequencerIfSequenced(messageCache, schema)
                .ToConverter(new MessagePayloadUnpackager(this.serialiser))
                .ToProcessor(schema.UnitOfWorkRunner)
                .ToEndPoint(this.messageHandlerRouter);
        }
    }
}