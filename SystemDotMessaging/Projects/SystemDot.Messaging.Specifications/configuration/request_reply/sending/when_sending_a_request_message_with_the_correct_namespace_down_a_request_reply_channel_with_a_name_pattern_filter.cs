using System.Linq;
using SystemDot.Messaging.Channels.Packaging.Headers;
using SystemDot.Messaging.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.sending
{
    [Subject("Request reply configuration")]
    public class when_sending_a_request_message_with_the_correct_namespace_down_a_request_reply_channel_with_a_name_pattern_filter : WithNoRepeaterMessageConfigurationSubject
    {
        static IBus bus;

        Establish context = () =>
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("Test")
                .ForRequestReplySendingTo("TestRecieverAddress")
                .OnlyForMessages(FilteredBy.Namespace("SystemDot.Messaging.Specifications"))
                .Initialise();

        Because of = () => bus.Send(new TestNamePatternMessage());

        It should_pass_the_message_through = () =>
            Deserialise<TestNamePatternMessage>(MessageSender.SentMessages.First().GetBody()).ShouldNotBeNull();
    }
}