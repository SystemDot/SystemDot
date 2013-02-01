using System.Collections.Generic;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.repeating
{
    public class when_starting_a_repeater_with_a_simple_repeat_strategy_for_the_second_time : WithSubject<MessageRepeater>
    {
        static List<MessagePayload> messages;
        static MessagePayload message;

        Establish context = () =>
        {
            With<PersistenceBehaviour>();
            Configure<IRepeatStrategy>(new SimpleRepeatStrategy());

            messages = new List<MessagePayload>();

            message = new MessagePayload();
            message.SetSequence(1);
            The<MessageCache>().AddMessage(message);

            Subject.MessageProcessed += m => messages.Add(m);
        };

        Because of = () =>
        {
            Subject.Start();
            Subject.Start();
        };

        It should_only_repeat_the_messages_once = () => messages.Count.ShouldEqual(1);
    }
}