using System;
using System.Linq;
using SystemDot.Messaging.Packaging.Headers;
using SystemDot.Messaging.Repeating;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Specifications.publishing;
using SystemDot.Messaging.Storage;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.request_reply
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_request : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string RecieverAddress = "TestRecieverAddress";

        static Int64 message;
        
        Establish context = () =>
        {
            Messaging.Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName).ForRequestReplySendingTo(RecieverAddress)
                .Initialise();

            message = 1;
        };

        Because of = () => Bus.Send(message);

        It should_send_a_message_with_the_correct_to_address = () =>
            GetServer().SentMessages.First().GetToAddress().ShouldEqual(BuildAddress(RecieverAddress));

        It should_send_a_message_with_the_correct_from_address = () =>
            GetServer().SentMessages.First().GetFromAddress().ShouldEqual(BuildAddress(ChannelName));

        It should_send_a_message_with_the_correct_content = () =>
            GetServer().SentMessages.First().DeserialiseTo<Int64>().ShouldEqual(message);

        It should_mark_the_message_with_the_persistence_id = () =>
            GetServer().SentMessages.First()
                .ShouldHaveCorrectPersistenceId(ChannelName, PersistenceUseType.RequestSend);

        It should_set_original_persistence_id_to_the_persistence_id_of_the_message_with_the_persistence_id = () =>
           GetServer().SentMessages.First().GetSourcePersistenceId()
               .ShouldEqual(GetServer().SentMessages.First().GetPersistenceId());

        It should_mark_the_message_with_the_time_the_message_is_sent = () =>
            GetServer().SentMessages.First().GetLastTimeSent().ShouldBeGreaterThan(DateTime.MinValue);

        It should_mark_the_message_with_the_amount_of_times_the_message_has_been_sent = () =>
            GetServer().SentMessages.First().GetAmountSent().ShouldEqual(1);

        It should_mark_the_message_with_the_sequence = () =>
            GetServer().SentMessages.First().GetSequence().ShouldEqual(1);

        It should_mark_the_message_with_first_sequence = () =>
            GetServer().SentMessages.ExcludeAcknowledgements().First().GetFirstSequence().ShouldEqual(1);

        It should_start_the_task_repeater = () => TaskRepeater.Started.ShouldBeTrue();
    }
}