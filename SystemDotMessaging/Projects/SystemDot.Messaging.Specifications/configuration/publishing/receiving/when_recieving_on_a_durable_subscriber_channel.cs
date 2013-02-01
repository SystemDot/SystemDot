using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Storage.Changes;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.publishing.receiving
{
    [Subject(SpecificationGroup.Description)]
    public class when_recieving_on_a_durable_subscriber_channel : WithMessageConfigurationSubject
    {
        const string ChannelName = "TestChannel";
        const string PublisherName = "TestPublisher";
        static MessagePayload payload;
        
        Establish context = () =>
        {
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                    .ForSubscribingTo(PublisherName)
                    .WithDurability()
                .Initialise();

            payload = new MessagePayload().MakeReceiveable(
                1, 
                PublisherName, 
                ChannelName, 
                PersistenceUseType.SubscriberSend);

            payload.SetFirstSequence(1);
            payload.SetSequence(1); 
        };

        Because of = () => MessageReciever.ReceiveMessage(payload);

        It should_have_persisted_the_message = () =>
            Resolve<IChangeStore>()
                .GetAddedMessages(PersistenceUseType.SubscriberReceive, BuildAddress(ChannelName))
                .ShouldContain(payload);
    }
}