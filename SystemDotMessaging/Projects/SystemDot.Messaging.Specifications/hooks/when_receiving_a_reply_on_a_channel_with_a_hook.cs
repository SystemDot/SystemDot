using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.hooks
{
    [Subject(SpecificationGroup.Description)]
    public class when_receiving_a_reply_on_a_channel_with_a_hook 
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string RecieverAddress = "TestRecieverAddress";

        static int message;
        static MessagePayload payload;
        static TestMessageProcessorHook hook;

        Establish context = () =>
        {
            hook = new TestMessageProcessorHook();

            Messaging.Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                    .ForRequestReplySendingTo(RecieverAddress)
                    .WithReceiveHook(hook)
                .Initialise();

            message = 1;

            payload = new MessagePayload().MakeSequencedReceivable(
                message, 
                RecieverAddress,
                ChannelName, 
                PersistenceUseType.ReplySend);
        };

        Because of = () => Server.ReceiveMessage(payload);

        It should_run_the_message_through_the_hook = () => hook.Message.ShouldEqual(message);
    }
}
