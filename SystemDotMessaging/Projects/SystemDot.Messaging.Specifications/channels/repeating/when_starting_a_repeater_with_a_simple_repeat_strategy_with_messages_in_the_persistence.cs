using System.Collections.Generic;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Channels.Repeating;
using SystemDot.Messaging.Channels.Sequencing;
using SystemDot.Messaging.Storage;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.repeating
{
    public class when_starting_a_repeater_with_a_simple_repeat_strategy_with_messages_in_the_persistence : WithSubject<MessageRepeater>
    {
        static List<MessagePayload> messages;
        static MessagePayload message1;
        static MessagePayload message2;

        Establish context = () =>
        {
            With<PersistenceBehaviour>();
            Configure<IRepeatStrategy>(new SimpleRepeatStrategy());
            
            messages = new List<MessagePayload>();
                
            message1 = new MessagePayload();
            message1.SetSequence(1);
            The<MessageCache>().AddMessage(message1);

            message2 = new MessagePayload();
            message2.SetSequence(2);
            The<MessageCache>().AddMessage(message2);

            Subject.MessageProcessed += m => messages.Add(m);
        };

        Because of = () => Subject.Start();

        It should_repeat_the_messages = () => messages.ShouldContain(message1, message2);
    }
}