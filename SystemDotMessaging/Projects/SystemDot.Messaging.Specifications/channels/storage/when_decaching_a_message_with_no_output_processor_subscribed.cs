using System;
using SystemDot.Messaging.Channels.Caching;
using SystemDot.Messaging.Channels.Packaging;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.storage
{
    [Subject("Message caching")]
    public class when_decaching_a_message_with_no_output_processor_subscribed : WithSubject<MessageDecacher>
    {
        static Exception exception;

        Because of = () => Subject.InputMessage(new MessagePayload());

        It should_not_fail = () => exception.ShouldBeNull();
    }
}