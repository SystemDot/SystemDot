using System;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Parallelism;
using SystemDot.Specifications;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.point_to_point.sending.repeating
{
    [Subject(SpecificationGroup.Description)]
    public class when_repeating_a_message_with_a_constant_time_repeat_on_the_channel_and_that_time_has_not_passed
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string ReceiverName = "TestReceiver";

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
                .ForPointToPointSendingTo(ReceiverName)
                .WithMessageRepeating(RepeatMessages.Every(TimeSpan.FromSeconds(10)))
                .Initialise();

            message = 1;

            bus.Send(message);

            currentDateProvider.AddToCurrentDate(TimeSpan.FromSeconds(10).Subtract(TimeSpan.FromTicks(1)));
        };

        Because of = () => The<ITaskRepeater>().Start();

        It should_not_repeat_the_message = () => MessageSender.SentMessages.Count.ShouldEqual(1);
    }
}