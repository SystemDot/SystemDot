using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Serialisation;
using SystemDot.Storage.Changes;
using SystemDot.Storage.Changes.Upcasting;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.sequencing
{
    [Subject(SpecificationGroup.Description)]
    public class when_passing_messages_in_the_correct_order_on_a_durable_channel_and_then_restarting_messaging 
        : WithMessageConfigurationSubject
    {
        const string ReceiverAddress = "ReceiverAddress";
        const string SenderAddress = "SenderAddress";

        static ChangeStore changeStore;
        static TestMessageHandler<int> handler;
       
        Establish context = () =>
        {
            changeStore = new InMemoryChangeStore();
            ConfigureAndRegister<ChangeStore>(changeStore);

            Messaging.Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ReceiverAddress).ForPointToPointReceiving().WithDurability()
                .Initialise();

            var messagePayload = new MessagePayload()
                .MakeSequencedReceivable(1, SenderAddress, ReceiverAddress, PersistenceUseType.PointToPointSend);
           
            handler = new TestMessageHandler<int>();
            Resolve<MessageHandlerRouter>().RegisterHandler(handler);

            GetServer().ReceiveMessage(messagePayload);

            Reset();
            ReInitialise();

            ConfigureAndRegister<ChangeStore>(changeStore);
            
            handler.HandledMessages.Clear();
            Resolve<MessageHandlerRouter>().RegisterHandler(handler);
        };

        Because of = () => Messaging.Configuration.Configure.Messaging()
            .UsingInProcessTransport()
            .OpenChannel(ReceiverAddress).ForPointToPointReceiving().WithDurability()
            .Initialise();

        It should_have_removed_the_message_from_persistence = () => handler.HandledMessages.ShouldBeEmpty();
    }
}