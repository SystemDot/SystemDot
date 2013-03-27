using System.Linq;
using SystemDot.Messaging.Batching;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.point_to_point.batching
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_messages_inside_a_batch : WithMessageConfigurationSubject
    {
        const int Message1 = 1;
        const int Message2 = 2;
        
        static IBus bus;

        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("SenderAddress").ForPointToPointSendingTo("ReceiverAddress")
                .Initialise();
        };

        Because of = () =>
        {
            using (Batch batch = bus.BatchSend())
            {
                bus.Send(Message1);
                bus.Send(Message2);
                batch.Complete();
            }
        };

        It should_send_a_batch_containing_both_messages = () =>
            Server.SentMessages.Single().DeserialiseTo<BatchMessage>().Messages.ShouldContain(Message1, Message2);
    }
}