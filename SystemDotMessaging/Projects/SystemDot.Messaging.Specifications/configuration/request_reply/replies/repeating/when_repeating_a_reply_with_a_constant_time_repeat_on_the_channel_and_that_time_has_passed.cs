using System;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Specifications.configuration.publishing;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Parallelism;
using SystemDot.Specifications;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.replies.repeating      
{
    [Subject(SpecificationGroup.Description)]
    public class when_repeating_a_reply_with_a_constant_time_repeat_on_the_channel_and_that_time_has_passed 
        : WithMessageConfigurationSubject
    {
        const string ChannelName = "Test";
        const string SenderAddress = "TestSender";
        const int Request = 1;
        const int Reply = 2;
        
        static IBus bus;
        static TestCurrentDateProvider currentDateProvider;

        Establish context = () =>
        {
            currentDateProvider = new TestCurrentDateProvider(DateTime.Now);
            ConfigureAndRegister<ICurrentDateProvider>(currentDateProvider);

            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ChannelName)
                .ForRequestReplyRecieving()
                .WithMessageRepeating(RepeatMessages.Every(TimeSpan.FromSeconds(10)))
                .Initialise();

            MessageReciever.ReceiveMessage(
                new MessagePayload().MakeReceiveable(
                    Request,
                    SenderAddress,
                    ChannelName,
                    PersistenceUseType.RequestSend));

            bus.Reply(Reply);

            currentDateProvider.AddToCurrentDate(TimeSpan.FromSeconds(10));
        };

        Because of = () => The<ITaskRepeater>().Start();

        It should_repeat_the_message = () => MessageSender.SentMessages.ExcludeAcknowledgements().Count.ShouldEqual(2);
    }
}