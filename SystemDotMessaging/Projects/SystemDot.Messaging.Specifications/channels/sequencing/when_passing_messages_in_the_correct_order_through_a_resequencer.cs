using System.Collections.Generic;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.sequencing
{
    [Subject("Message processing")]
    public class when_passing_messages_in_the_correct_order_through_a_resequencer : WithMessageProcessorSubject<Resequencer>
    {
        static MessagePayload message1;
        static MessagePayload message2;
        static List<MessagePayload> processedMessages;

        Establish context = () =>
        {
            With<PersistenceBehaviour>();
            The<MessageCache>().SetSequence(2);
            
            processedMessages = new List<MessagePayload>();
            Subject.MessageProcessed += payload => processedMessages.Add(payload);
            
            message1 = new MessagePayload();
            message1.SetSequence(2);
            The<MessageCache>().AddMessage(message1);
            
            message2 = new MessagePayload();
            message2.SetSequence(3);
            The<MessageCache>().AddMessage(message2);
        };

        Because of = () =>
        {
            Subject.InputMessage(message1);
            Subject.InputMessage(message2);
        };

        It should_pass_the_messages_through = () => processedMessages.ShouldContain(message1, message2);

        It should_decache_the_messages = () => The<MessageCache>().GetMessages().ShouldNotContain(message1, message2);

        It should_persist_the_sequence_of_the_last_message_processed = () => The<MessageCache>().GetSequence().ShouldEqual(4);
    }
}