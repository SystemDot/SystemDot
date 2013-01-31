using System;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Parallelism;
using SystemDot.Specifications;
using Machine.Specifications;
using SystemDot.Messaging.Specifications.configuration.publishing;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.replies.repeating
{
    [Subject(requests.SpecificationGroup.Description)]
    public class when_repeating_a_reply_with_and_four_seconds_have_passed
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string SenderAddress = "TestSender";
        const int Request = 1;
        const int Reply = 2;

        static IBus bus;
        static TestCurrentDateProvider currentDateProvider;

        Establish context = () =>
        {
            currentDateProvider = new TestCurrentDateProvider(DateTime.Now);
            ConfigureAndRegister<ICurrentDateProvider>(currentDateProvider);

            bus = Configuration.Configure.Messaging()
                 .UsingInProcessTransport()
                 .OpenChannel(ChannelName)
                 .ForRequestReplyRecieving()
                 .Initialise();

            MessageReciever.ReceiveMessage(
                new MessagePayload().MakeReceiveable(
                    Request,
                    SenderAddress,
                    ChannelName,
                    PersistenceUseType.RequestSend));

            bus.Reply(Reply);

            currentDateProvider.AddToCurrentDate(TimeSpan.FromSeconds(4));
        };

        Because of = () => The<ITaskRepeater>().Start();

        It should_repeat_the_message = () => MessageSender.SentMessages.ExcludeAcknowledgements().Count.ShouldEqual(2);
    }
}