using System;
using System.Linq;
using SystemDot.Http;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.Http.Configuration;
using SystemDot.Parallelism;
using SystemDot.Serialisation;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.transport.http.remote.client
{
    [Subject(SpecificationGroup.Description)]
    public class when_two_messages_are_received_by_long_polling : WithConfigurationSubject
    {
        const string ReceiverName = "ReceiverName";
        const string SenderName = "SenderName";
        const int Message1 = 1;
        const int Message2 = 2;
        const string RemoteClientInstance = "RemoteClientInstance";
        const string ProxyInstance = "RemoteProxyInstance";

        static TestTaskStarter taskStarter;
        static TestWebRequestor requestor;
        static MessagePayload messagePayload1;
        static MessagePayload messagePayload2;
        static TestMessageHandler<int> handler;

        Establish context = () =>
        {
            requestor = new TestWebRequestor(
                new PlatformAgnosticSerialiser(),
                new FixedPortAddress(Environment.MachineName, ProxyInstance));

            ConfigureAndRegister<IWebRequestor>(requestor);

            taskStarter = new TestTaskStarter(1);
            taskStarter.Pause(); 
            ConfigureAndRegister<ITaskStarter>(taskStarter);

            Configuration.Configure.Messaging()
                .UsingHttpTransport()
                .AsARemoteClient(RemoteClientInstance)
                .UsingProxy(MessageServer.Local(ProxyInstance))
                .OpenChannel(ReceiverName)
                .ForPointToPointReceiving()
                .Initialise();

            messagePayload1 =
                new MessagePayload().MakeReceiveable(
                    Message1,
                    SenderName,
                    ReceiverName,
                    RemoteClientInstance,
                    ProxyInstance,
                    PersistenceUseType.PointToPointSend);
            messagePayload1.SetSequenceOriginSetOn(DateTime.Today);
            messagePayload1.SetFirstSequence(1);
            messagePayload1.SetSequence(1);

            messagePayload2 =
                new MessagePayload().MakeReceiveable(
                    Message2,
                    SenderName,
                    ReceiverName,
                    RemoteClientInstance,
                    ProxyInstance,
                    PersistenceUseType.PointToPointSend);
            messagePayload2.SetSequenceOriginSetOn(DateTime.Today);
            messagePayload2.SetFirstSequence(1);
            messagePayload2.SetSequence(1);

            handler = new TestMessageHandler<int>();
            Resolve<MessageHandlerRouter>().RegisterHandler(handler);
        };

        Because of = () =>
        {
            requestor.AddMessages(messagePayload1, messagePayload2);
            taskStarter.UnPause();
        };

        It should_output_the_first_recieved_message = () => handler.HandledMessages.First().ShouldEqual(Message1);

        It should_output_the_second_recieved_message = () => handler.HandledMessages.Last().ShouldEqual(Message2);
    }
}