using System;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.local
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_local_message_after_configuring_a_local_channel_and_then_another : WithNoRepeaterMessageConfigurationSubject
    {
        static IBus bus;
        static Exception exception;

        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenLocalChannel()
                .OpenChannel("Channel").ForRequestReplyRecieving()
                .Initialise();
        };

        Because of = () => exception = Catch.Exception(() => bus.SendLocal(new object()));

        It should_not_fail = () => exception.ShouldBeNull();
    }
}