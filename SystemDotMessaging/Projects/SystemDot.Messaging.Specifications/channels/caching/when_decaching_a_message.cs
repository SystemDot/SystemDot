﻿using SystemDot.Messaging.Channels.Caching;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Channels.Repeating;
using SystemDot.Messaging.Storage;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.caching
{
    [Subject("Message caching")]
    public class when_decaching_a_message : WithSubject<MessageDecacher>
    {
        static MessagePayload message;
        static MessagePayload processedMessage;

        Establish context = () =>
        {
            message = new MessagePayload();
            message.IncreaseAmountSent();

            With<PersistenceBehaviour>();
            Subject.MessageProcessed += payload => processedMessage = payload;
        };

        Because of = () => Subject.InputMessage(message);

        It should_decache_the_message = () => The<MessageCache>().GetMessages().ShouldNotContain(message);

        It should_process_the_message = () => processedMessage.ShouldBeTheSameAs(message);
    }
}