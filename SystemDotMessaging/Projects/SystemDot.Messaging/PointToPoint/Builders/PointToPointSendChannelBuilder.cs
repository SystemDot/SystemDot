using System.Diagnostics.Contracts;
using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Batching;
using SystemDot.Messaging.Caching;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport;
using SystemDot.Parallelism;
using SystemDot.Serialisation;
using SystemDot.Storage.Changes;

namespace SystemDot.Messaging.PointToPoint.Builders
{
    public class PointToPointSendChannelBuilder
    {
        readonly IMessageSender messageSender;
        readonly ISerialiser serialiser;
        readonly ISystemTime systemTime;
        readonly ITaskRepeater taskRepeater;
        readonly MessageCacheFactory messageCacheFactory;
        readonly MessageAcknowledgementHandler acknowledgementHandler;

        public PointToPointSendChannelBuilder(
            IMessageSender messageSender, 
            ISerialiser serialiser, 
            ISystemTime systemTime, 
            ITaskRepeater taskRepeater, 
            MessageCacheFactory messageCacheFactory, 
            MessageAcknowledgementHandler acknowledgementHandler)
        {
            Contract.Requires(messageSender != null);
            Contract.Requires(serialiser != null);
            Contract.Requires(systemTime != null);
            Contract.Requires(taskRepeater != null);
            Contract.Requires(messageCacheFactory != null);
            Contract.Requires(acknowledgementHandler != null);

            this.messageSender = messageSender;
            this.serialiser = serialiser;
            this.systemTime = systemTime;
            this.taskRepeater = taskRepeater;
            this.messageCacheFactory = messageCacheFactory;
            this.acknowledgementHandler = acknowledgementHandler;
        }

        public void Build(PointToPointSendChannelSchema schema)
        {
            Contract.Requires(schema != null);

            SendMessageCache messageCache = this.messageCacheFactory.CreateSendCache(
                PersistenceUseType.PointToPointSend, 
                schema.FromAddress);

            this.acknowledgementHandler.RegisterCache(messageCache);

            MessagePipelineBuilder.Build()
                .WithBusSendTo(new MessageFilter(new PassThroughMessageFilterStategy()))
                .ToProcessor(new BatchPackager())
                .ToConverter(new MessagePayloadPackager(this.serialiser))
                .ToProcessor(new Sequencer(messageCache))
                .ToProcessor(new MessageAddresser(schema.FromAddress, schema.RecieverAddress))
                .ToMessageRepeater(messageCache, this.systemTime, this.taskRepeater, schema.RepeatStrategy)
                .ToProcessor(new MessagePayloadCopier(this.serialiser))
                .ToProcessor(new SendChannelMessageCacher(messageCache))
                .ToProcessor(new SequenceOriginRecorder(messageCache))
                .ToProcessor(new PersistenceSourceRecorder())
                .Queue()
                .ToEndPoint(this.messageSender);
        }
    }
}