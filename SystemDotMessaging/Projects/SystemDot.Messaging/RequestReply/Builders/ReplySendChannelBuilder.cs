using System.Diagnostics.Contracts;
using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Builders;
using SystemDot.Messaging.Caching;
using SystemDot.Messaging.Distribution;
using SystemDot.Messaging.Expiry;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport;
using SystemDot.Parallelism;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.RequestReply.Builders
{
    public class ReplySendChannelBuilder
    {
        readonly IMessageSender messageSender;
        readonly ISerialiser serialiser;
        readonly ICurrentDateProvider currentDateProvider;
        readonly ITaskRepeater taskRepeater;
        readonly PersistenceFactorySelector persistenceFactorySelector;
        readonly MessageAcknowledgementHandler acknowledgementHandler;

        public ReplySendChannelBuilder(
            IMessageSender messageSender, 
            ISerialiser serialiser, 
            ICurrentDateProvider currentDateProvider, 
            ITaskRepeater taskRepeater, 
            PersistenceFactorySelector persistenceFactorySelector, 
            MessageAcknowledgementHandler acknowledgementHandler)
        {
            Contract.Requires(messageSender != null);
            Contract.Requires(serialiser != null);
            Contract.Requires(currentDateProvider != null);
            Contract.Requires(taskRepeater != null);
            Contract.Requires(persistenceFactorySelector != null);
            Contract.Requires(acknowledgementHandler != null);
            
            this.messageSender = messageSender;
            this.serialiser = serialiser;
            this.currentDateProvider = currentDateProvider;
            this.taskRepeater = taskRepeater;
            this.persistenceFactorySelector = persistenceFactorySelector;
            this.acknowledgementHandler = acknowledgementHandler;
        }

        public IMessageInputter<object> Build(ReplySendChannelSchema schema, EndpointAddress senderAddress)
        {
            MessageCache messageCache = this.persistenceFactorySelector
                .Select(schema)
                .CreateCache(PersistenceUseType.ReplySend, senderAddress);
            
            this.acknowledgementHandler.RegisterCache(messageCache);

            var startPoint = new Pipe<object>();

            MessagePipelineBuilder.Build()
                .With(startPoint)
                .ToProcessors(schema.Hooks.ToArray())
                .ToConverter(new MessagePayloadPackager(this.serialiser))
                .ToProcessor(new Sequencer(messageCache))
                .ToProcessor(new MessageAddresser(schema.FromAddress, senderAddress))
                .ToMessageRepeater(messageCache, this.currentDateProvider, this.taskRepeater, schema.RepeatStrategy)
                .ToProcessor(new MessagePayloadCopier(this.serialiser))
                .ToProcessor(new SendChannelMessageCacher(messageCache))
                .ToProcessor(new PersistenceSourceRecorder())
                .Queue()
                .ToProcessor(new MessageExpirer(schema.ExpiryStrategy, messageCache))
                .ToEndPoint(this.messageSender);

            return startPoint;
        }
    }
}