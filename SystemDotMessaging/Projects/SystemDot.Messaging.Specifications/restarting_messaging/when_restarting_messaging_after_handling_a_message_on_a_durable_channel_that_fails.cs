using System;
using System.Collections.Generic;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Serialisation;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.restarting_messaging
{
    [Subject(SpecificationGroup.Description)]
    public class when_restarting_messaging_after_handling_a_message_on_a_durable_channel_that_fails
        : WithMessageConfigurationSubject
    {
        const string ReceiverName = "ReceiverName";
        const string SenderAddress = "SenderAddress";
        const Int64 Message1 = 1;
        const Int64 Message2 = 2;

        static TestMessageHandler<Int64> handler;
        static IChangeStore changeStore;
        static MessagePayload payload1;
        static MessagePayload payload2; 
        static List<MessageLoadedToCache> messagesLoadedToCacheEvents;

        Establish context = () =>
        {
            messagesLoadedToCacheEvents = new List<MessageLoadedToCache>();
            
            changeStore = new InMemoryChangeStore(new JsonSerialiser());
            ConfigureAndRegister<IChangeStore>(changeStore);

            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ReceiverName)
                .ForPointToPointReceiving()
                .WithDurability()
                .RegisterHandlers(r => r.RegisterHandler(new FailingMessageHandler<Int64>()))
                .Initialise();

            Catch.Exception(() => Server.ReceiveMessage(
                new MessagePayload().MakeSequencedReceivable(
                    Message1,
                    SenderAddress,
                    ReceiverName,
                    PersistenceUseType.PointToPointSend)));

            payload1 = new MessagePayload().MakeSequencedReceivable(
                Message1,
                SenderAddress,
                ReceiverName,
                PersistenceUseType.PointToPointSend);

            Catch.Exception(() => Server.ReceiveMessage(payload1));

            payload2 = new MessagePayload().MakeSequencedReceivable(
                Message2,
                SenderAddress,
                ReceiverName,
                PersistenceUseType.PointToPointSend);

            Catch.Exception(() => Server.ReceiveMessage(payload2));

            Reset();
            ReInitialise();
            Messenger.Register<MessageLoadedToCache>(e => messagesLoadedToCacheEvents.Add(e));

            ConfigureAndRegister<IChangeStore>(changeStore);
            handler = new TestMessageHandler<Int64>();
        };

        Because of = () =>
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(ReceiverName)
                .ForPointToPointReceiving()
                .WithDurability()
                .RegisterHandlers(r => r.RegisterHandler(handler))
                .Initialise();

        It should_notify_that_the_first_sent_message_was_loaded_into_the_cache = () =>
            messagesLoadedToCacheEvents.ShouldContain(m =>
                m.CacheAddress == BuildAddress(ReceiverName)
                    && m.UseType == PersistenceUseType.PointToPointReceive
                    && m.Message == payload1);

        It should_notify_that_the_second_sent_message_was_loaded_into_the_cache = () =>
            messagesLoadedToCacheEvents.ShouldContain(m =>
                m.CacheAddress == BuildAddress(ReceiverName)
                    && m.UseType == PersistenceUseType.PointToPointReceive
                    && m.Message == payload2);
    }
}