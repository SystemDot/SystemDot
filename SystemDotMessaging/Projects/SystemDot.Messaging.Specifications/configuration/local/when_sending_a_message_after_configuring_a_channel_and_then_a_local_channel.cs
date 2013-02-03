using System.Linq;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.local
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_message_after_configuring_a_channel_and_then_a_local_channel 
        : WithMessageConfigurationSubject
    {
        static int message;
        static IBus bus;
        
        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("Channel").ForRequestReplySendingTo("Reciever")
                .OpenLocalChannel()
                .Initialise();

            message = 1;
        };

        Because of = () => bus.Send(message);

        It should_send_the_message_down_the_channel = () => 
            MessageSender.SentMessages.First().DeserialiseTo<int>().ShouldEqual(message);
    }
}