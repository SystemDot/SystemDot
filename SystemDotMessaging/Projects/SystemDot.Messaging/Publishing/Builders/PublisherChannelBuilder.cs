using System.Diagnostics.Contracts;
using SystemDot.Messaging.Builders;
using SystemDot.Messaging.Caching;
using SystemDot.Messaging.Filtering;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Parallelism;
using SystemDot.Serialisation;
using SystemDot.Storage.Changes;

namespace SystemDot.Messaging.Publishing.Builders
{
    public class PublisherChannelBuilder
    {
        readonly IPublisherRegistry publisherRegistry;
        readonly ISerialiser serialiser;
        readonly ITaskRepeater taskRepeater;
        readonly PersistenceFactorySelector persistenceFactorySelector;
        readonly ISubscriberSendChannelBuilder subscriberChannelBuilder;
        readonly ICurrentDateProvider currentDateProvider;
        readonly IChangeStore changeStore;

        public PublisherChannelBuilder(
            IPublisherRegistry publisherRegistry, 
            ISerialiser serialiser, 
            ITaskRepeater taskRepeater,
            PersistenceFactorySelector persistenceFactorySelector, 
            ISubscriberSendChannelBuilder subscriberChannelBuilder, 
            ICurrentDateProvider currentDateProvider, 
            IChangeStore changeStore)
        {
            Contract.Requires(publisherRegistry != null);
            Contract.Requires(serialiser != null);
            Contract.Requires(taskRepeater != null);
            Contract.Requires(persistenceFactorySelector != null);
            Contract.Requires(subscriberChannelBuilder != null);
            Contract.Requires(currentDateProvider != null);
            Contract.Requires(changeStore != null);
            
            this.publisherRegistry = publisherRegistry;
            this.serialiser = serialiser;
            this.taskRepeater = taskRepeater;
            this.persistenceFactorySelector = persistenceFactorySelector;
            this.subscriberChannelBuilder = subscriberChannelBuilder;
            this.currentDateProvider = currentDateProvider;
            this.changeStore = changeStore;
        }

        public void Build(PublisherChannelSchema schema)
        {
            MessageCache messageCache = this.persistenceFactorySelector
                .Select(schema)
                .CreateCache(PersistenceUseType.PublisherSend, schema.FromAddress);

            var publisherEndpoint = new Publisher(schema.FromAddress, this.subscriberChannelBuilder, this.changeStore);

            MessagePipelineBuilder.Build()
                .WithBusPublishTo(new MessageFilter(schema.MessageFilterStrategy))
                .ToConverter(new MessagePayloadPackager(this.serialiser))
                .ToProcessor(new Sequencer(messageCache))
                .ToMessageRepeater(messageCache, this.currentDateProvider, this.taskRepeater, EscalatingTimeRepeatStrategy.Default)
                .ToProcessor(new MessagePayloadCopier(this.serialiser))
                .ToProcessor(new SendChannelMessageCacher(messageCache))
                .Queue()
                .ToProcessor(publisherEndpoint)
                .ToEndPoint(new MessageDecacher(messageCache));

            this.publisherRegistry.RegisterPublisher(schema.FromAddress, publisherEndpoint);
        }
    }
}