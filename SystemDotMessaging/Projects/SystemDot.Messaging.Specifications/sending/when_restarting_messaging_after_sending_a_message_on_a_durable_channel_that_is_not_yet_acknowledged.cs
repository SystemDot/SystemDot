using System;
using System.Collections.Generic;
using System.Linq;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Parallelism;
using SystemDot.Serialisation;
using SystemDot.Specifications;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.sending
{
    [Subject(SpecificationGroup.Description)]
    public class when_restarting_messaging_after_sending_a_message_on_a_durable_channel_that_is_not_yet_acknowledged
        : WithMessageConfigurationSubject
    {
        const string SenderAddress = "SenderAddress";
        const string ReceiverAddress = "ReceiverAddress";
        const int Message1 = 1;
        const int Message2 = 2;

        static IChangeStore changeStore;
        static List<MessageLoadedToCache> messagesLoadedToCacheEvents;

        Establish context = () =>
        {
            messagesLoadedToCacheEvents = new List<MessageLoadedToCache>();
            
            changeStore = new InMemoryChangeStore(new PlatformAgnosticSerialiser());

            ConfigureAndRegister<IChangeStore>(changeStore);
            ConfigureAndRegister<ITaskRepeater>(new TestTaskRepeater());

            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(SenderAddress)
                .ForPointToPointSendingTo(ReceiverAddress).WithDurability()
                .Initialise();

            Bus.Send(Message1);
            Bus.Send(Message2);

            Reset();
            Initialise();

            ConfigureAndRegister<IChangeStore>(changeStore);
            ConfigureAndRegister<ITaskRepeater>(new TestTaskRepeater());
            ConfigureAndRegister<ISystemTime>(new TestSystemTime(DateTime.Now.AddDays(1)));

            Messenger.Register<MessageLoadedToCache>(e => messagesLoadedToCacheEvents.Add(e));
        };

        Because of = () =>
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(SenderAddress)
                .ForPointToPointSendingTo(ReceiverAddress).WithDurability()
                .Initialise();

        It should_notify_that_the_first_sent_message_was_loaded_into_the_cache = () =>
            messagesLoadedToCacheEvents.ShouldContain(m =>
                m.CacheAddress == BuildAddress(SenderAddress)
                && m.UseType == PersistenceUseType.PointToPointSend
                && m.Message == Server.SentMessages.First());

        It should_notify_that_the_second_sent_message_was_loaded_into_the_cache = () =>
            messagesLoadedToCacheEvents.ShouldContain(m =>
                m.CacheAddress == BuildAddress(SenderAddress)
                && m.UseType == PersistenceUseType.PointToPointSend
                && m.Message == Server.SentMessages.ElementAt(1));
    }
}