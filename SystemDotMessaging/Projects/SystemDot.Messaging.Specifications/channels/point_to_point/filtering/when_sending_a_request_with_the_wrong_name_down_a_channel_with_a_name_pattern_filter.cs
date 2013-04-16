using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.point_to_point.filtering
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_request_with_the_wrong_name_down_a_channel_with_a_name_pattern_filter 
        : WithMessageConfigurationSubject
    {
        static IBus bus;

        Establish context = () =>
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("Test")
                    .ForPointToPointSendingTo("TestRecieverAddress")
                        .OnlyForMessages(FilteredBy.NamePattern("SomethingElse"))
                .Initialise();

        Because of = () => bus.Send(new TestNamePatternMessage());

        It should_not_pass_the_message_through = () => Server.SentMessages.ShouldBeEmpty();
    }
}