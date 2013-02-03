using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.local
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_local_message_after_configuring_a_channel_and_then_a_local_channel 
        : WithMessageConfigurationSubject
    {
        static int message;
        static IBus bus;
        static TestMessageHandler<int> handler;

        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("Channel").ForRequestReplyRecieving()
                .OpenLocalChannel()
                .Initialise();

            message = 1;

            handler = new TestMessageHandler<int>();
            Resolve<MessageHandlerRouter>().RegisterHandler(handler);
        };

        Because of = () => bus.SendLocal(message);

        It should_send_the_message_to_any_handlers_registered_for_that_message = () => 
            handler.HandledMessage.ShouldEqual(message);
    }
}