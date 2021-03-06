using System.Linq;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.Http.Remote.Clients;
using SystemDot.Parallelism;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.servers_using_proxies
{
    [Subject(SpecificationGroup.Description)]
    public class when_a_message_recieved_has_been_processed : WithHttpConfigurationSubject
    {
        const string ReceiverName = "ReceiverName";
        const string SenderName = "SenderName";

        static TestTaskStarter taskStarter; 
        static MessagePayload messagePayload;

        Establish context = () =>
        {
            taskStarter = new TestTaskStarter(2);
            taskStarter.Pause();

            ConfigureAndRegister<ITaskStarter>(taskStarter); 

            Configuration.Configure.Messaging()
                .UsingHttpTransport().AsAServerUsingAProxy("Server")
                .OpenChannel(ReceiverName).ForPointToPointReceiving()
                .Initialise();

            messagePayload = new MessagePayload()
                .MakeReceivable(1, SenderName, ReceiverName, PersistenceUseType.PointToPointSend);
        };

        Because of = () =>
        {
            WebRequestor.AddMessages(messagePayload);
            taskStarter.UnPause();
        };

        It should_poll_again = () => 
            WebRequestor.RequestsMade
                .DeserialiseToPayloads()
                .Count(p => p.HasHeader<LongPollRequestHeader>()).ShouldEqual(2);
    }
}