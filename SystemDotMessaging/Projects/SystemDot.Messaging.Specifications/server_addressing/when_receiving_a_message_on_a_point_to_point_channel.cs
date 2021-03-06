using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.server_addressing
{
    [Subject(SpecificationGroup.Description)]
    public class when_receiving_a_message_on_a_point_to_point_channel : WithHttpServerConfigurationSubject
    {
        const string ReceiverServerName = "ReceiverServerName";
        const string ReceiverChannel = "ReceiverChannel";
        const string SenderServerName = "SenderServer";
        const string SenderServerAddress = "SenderServerAddress";
        const string ReceiverServerAddress = "ReceiverServerAddress";

        static MessagePayload messagePayload;

        Establish context = () =>
        {
            ServerAddressConfiguration.AddAddress(ReceiverServerName, ReceiverServerAddress);

            Configuration.Configure.Messaging()
                .UsingHttpTransport().AsAServer(ReceiverServerName)
                .OpenChannel(ReceiverChannel).ForPointToPointReceiving()
                .Initialise();

            messagePayload = new MessagePayload().MakeSequencedReceivable(
                1,
                BuildAddress("SenderChannel", SenderServerName, SenderServerAddress),
                BuildAddress(ReceiverChannel, ReceiverServerName, ReceiverServerAddress),
                PersistenceUseType.RequestSend);
        };

        Because of = () => SendMessageToServer(messagePayload);

        It should_send_an_acknowledgement_to_the_server_address_specified_on_the_incoming_message = () =>
            WebRequestor.DeserialiseSingleRequest<MessagePayload>().IsAcknowledgement();
    }
}