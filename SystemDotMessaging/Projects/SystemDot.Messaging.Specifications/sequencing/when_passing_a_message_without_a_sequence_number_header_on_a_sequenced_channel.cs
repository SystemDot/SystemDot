using System;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.sequencing
{
    [Subject(SpecificationGroup.Description)]
    public class when_passing_a_message_without_a_sequence_number_header_on_a_sequenced_channel 
        : WithMessageConfigurationSubject
    {
        const string ReceiverAddress = "ReceiverAddress";
        static Exception exception;
        static MessagePayload messagePayload;
        
        Establish context = () =>
        {
            Messaging.Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ReceiverAddress).ForPointToPointReceiving().Sequenced()
                .Initialise();

            messagePayload = new MessagePayload()
                .MakeSequencedReceivable(1, "SenderAddress", ReceiverAddress, PersistenceUseType.PointToPointSend);
        };

        Because of = () =>
        {
            exception = Catch.Exception(() => GetServer().ReceiveMessage(messagePayload));
        };

        It should_not_fail = () => exception.ShouldBeNull();
    }
}