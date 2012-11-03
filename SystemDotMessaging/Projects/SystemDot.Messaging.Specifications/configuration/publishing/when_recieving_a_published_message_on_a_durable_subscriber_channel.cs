using SystemDot.Messaging.Channels.Handling;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Storage;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.publishing
{
    [Subject("Publishing configuration")]
    public class when_recieving_a_published_message_on_a_durable_subscriber_channel : WithMessageConfigurationSubject
    {
        const string ChannelName = "TestChannel";
        const string PublisherName = "TestPublisher";
        static MessagePayload payload;
        
        Establish context = () =>
        {
            ConfigureAndRegister<IDatastore>(new TestDatastore());

            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                    .ForSubscribingTo(PublisherName)
                    .WithDurability()
                .Initialise();

            payload = CreateRecieveablePayload(1, PublisherName, ChannelName, PersistenceUseType.SubscriberSend);
        };

        Because of = () => MessageReciever.RecieveMessage(payload);

        It should_have_persisted_the_message = () => 
            Resolve<IDatastore>().As<TestDatastore>().AddedMessages.ShouldContain(payload);
    }
}