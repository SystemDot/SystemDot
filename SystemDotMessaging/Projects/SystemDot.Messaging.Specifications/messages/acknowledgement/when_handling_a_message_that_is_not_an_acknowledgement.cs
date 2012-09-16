using System;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Messaging.Messages.Storage;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.messages.acknowledgement
{
    public class when_handling_a_message_that_is_not_an_acknowledgement : WithSubject<MessageAcknowledgementHandler>
    {
        static Exception exception; 
        static MessagePayload message;

        Establish context = () =>
        {
            Configure<IMessageStore>(new InMemoryMessageStore());
            message = new MessagePayload();
        };

        Because of = () => exception = Catch.Exception(() => Subject.InputMessage(message));

        It should_not_fail = () => exception.ShouldBeNull();
    }
}