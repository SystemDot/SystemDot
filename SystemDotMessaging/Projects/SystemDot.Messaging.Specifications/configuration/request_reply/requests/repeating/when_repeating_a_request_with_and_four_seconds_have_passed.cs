using System;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Parallelism;
using SystemDot.Specifications;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.requests.repeating
{
    [Subject(SpecificationGroup.Description)]
    public class when_repeating_a_request_with_and_four_seconds_have_passed
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string SenderChannelName = "TestSender";

        static IBus bus;
        static int message;
        static TestCurrentDateProvider currentDateProvider;

        Establish context = () =>
        {
            currentDateProvider = new TestCurrentDateProvider(DateTime.Now);
            ConfigureAndRegister<ICurrentDateProvider>(currentDateProvider);

            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                .ForRequestReplySendingTo(SenderChannelName)
                .Initialise();

            message = 1;

            bus.Send(message);

            currentDateProvider.AddToCurrentDate(TimeSpan.FromSeconds(4));
        };

        Because of = () => The<ITaskRepeater>().Start();

        It should_repeat_the_message = () => MessageSender.SentMessages.Count.ShouldEqual(2);
    }
}