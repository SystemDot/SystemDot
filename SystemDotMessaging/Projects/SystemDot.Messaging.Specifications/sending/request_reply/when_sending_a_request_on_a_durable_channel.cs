using SystemDot.Messaging.Storage;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.sending.request_reply
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_request_on_a_durable_channel : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string RecieverAddress = "TestRecieverAddress";
        
        static int message;
        
        Establish context = () =>
        {
            Messaging.Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName).ForRequestReplySendingTo(RecieverAddress).WithDurability()
                .Initialise();

            message = 1;
        };

        Because of = () => Bus.Send(message);

        It should_persist_the_message = () =>
            Resolve<IChangeStore>()
                .GetSendMessages(PersistenceUseType.RequestSend, BuildAddress(ChannelName))
                .ShouldNotBeEmpty();
    }
}