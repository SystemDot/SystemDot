using System.Collections.Generic;
using SystemDot.Messaging.Channels;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Channels.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Storage.InMemory;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.sequencing
{
    [Subject("Message processing")]
    public class when_passing_messages_in_the_incorrect_order_through_a_resequencer : WithMessageProcessorSubject<Resequencer>
    {
        static MessagePayload message1;
        static MessagePayload message2;
        static List<MessagePayload> processedMessages;

        Establish context = () =>
        {
            With<PersistenceBehaviour>();
            The<IPersistence>().SetSequence(1);
            
            processedMessages = new List<MessagePayload>();
            Subject.MessageProcessed += payload => processedMessages.Add(payload);

            message1 = new MessagePayload();
            message1.SetSequence(4);
            
            message2 = new MessagePayload();
            message2.SetSequence(2);
        };

        Because of = () =>
        {
            Subject.InputMessage(message1);
            Subject.InputMessage(message2);
        };

        It should_not_pass_the_messages_through = () => processedMessages.ShouldNotContain(message1, message2);
    }
}