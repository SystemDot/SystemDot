using System;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Channels.RequestReply.Repeating;
using SystemDot.Messaging.Storage;
using SystemDot.Specifications;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.repeating
{
    public class when_inputting_a_message_into_a_repeater : WithSubject<MessageRepeater>
    {
        static MessagePayload message;
        static MessagePayload processedMessage;
        static DateTime currentDate;

        Establish context = () =>
        {
            With<PersistenceBehaviour>();

            currentDate = DateTime.Today;
            Configure<ICurrentDateProvider>(new TestCurrentDateProvider(currentDate));
            
            message = new MessagePayload();
            Subject.MessageProcessed += m => processedMessage = m;
        };

        Because of = () => Subject.InputMessage(message);

        It should_output_the_message = () => 
            message.ShouldEqual(processedMessage);

        It should_set_the_last_time_the_message_was_sent = () => 
            processedMessage.GetLastTimeSent().ShouldEqual(currentDate);

        It should_increase_the_amount_of_times_the_message_was_sent = () => 
            processedMessage.GetAmountSent().ShouldEqual(1);

        It should_set_the_correct_persistence_id_on_the_message = () =>
            message.GetPersistenceId()
                .ShouldEqual(new MessagePersistenceId(
                    message.Id,
                    The<IPersistence>().Address,
                    The<IPersistence>().UseType));

        It should_persist_the_message = () => The<IPersistence>().GetMessages().ShouldContain(message);
    }
}