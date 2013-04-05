using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Serialisation;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.request_reply.replies
{
    [Subject(SpecificationGroup.Description)]
    public class when_handling_a_reply_on_a_non_durable_channel_that_fails_and_messaging_is_restarted 
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string ReceiverAddress = "TestReceiveAddress";
        const int Message = 1;
        
        static TestMessageHandler<int> handler;
        static IChangeStore changeStore;
            
        Establish context = () =>
        {
            changeStore = new InMemoryChangeStore(new PlatformAgnosticSerialiser());
            ConfigureAndRegister<IChangeStore>(changeStore);
            
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                .ForRequestReplySendingTo(ReceiverAddress)
                .RegisterHandlers(r => r.RegisterHandler(new FailingMessageHandler<int>()))
                .Initialise();

            Catch.Exception(() => Server.ReceiveMessage(
                new MessagePayload().MakeSequencedReceivable(
                    Message,
                    ReceiverAddress, 
                    ChannelName, 
                    PersistenceUseType.ReplyReceive)));
           
            ResetIoc();
            Initialise();

            ConfigureAndRegister<IChangeStore>(changeStore);
            handler = new TestMessageHandler<int>();
        };

        Because of = () => 
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                .ForRequestReplySendingTo(ReceiverAddress)
                .RegisterHandlers(r => r.RegisterHandler(handler))
                .Initialise();

        It should_repeat_the_message_when_restarted = () => handler.LastHandledMessage.ShouldNotEqual(Message);
    }
}