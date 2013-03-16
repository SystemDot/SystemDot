using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.publishing
{
    [Subject(SpecificationGroup.Description)]
    public class when_publishing_on_a_durable_publish_subscribe_channel : WithPublisherSubject
    {
        const string ChannelName = "Test";
        const string SubscriberName = "TestSubscriber";

        static IBus bus;
        static int message;
        
        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName).ForPublishing()
                    .WithDurability()
                .Initialise();

            message = 1;
            Subscribe(BuildAddress(SubscriberName), BuildAddress(ChannelName));
        };

        Because of = () => bus.Publish(message);

        It should_have_decached_the_message_after_successful_publishing = () =>
            Resolve<InMemoryChangeStore>()
                .GetSendMessages(PersistenceUseType.PublisherSend, BuildAddress(ChannelName))
                .ShouldBeEmpty();
    }
}