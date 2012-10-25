using SystemDot.Messaging.Channels;
using SystemDot.Messaging.Channels.Acknowledgement;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Storage.InMemory;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.acknowledgement
{
    public class when_handling_a_recieved_acknowledgement : WithSubject<MessageAcknowledgementHandler>
    {
        static MessagePayload acknowledgement;
        static MessagePayload message;
        static IPersistence persistence;

        Establish context = () =>
        {
            var store = new InMemoryDatatore();
            
            persistence = new InMemoryPersistence(
                store, 
                PersistenceUseType.Other, 
                new EndpointAddress("Channel", "Server"));

            Subject.RegisterPersistence(persistence);
            
            message = new MessagePayload();
            var id = new MessagePersistenceId(message.Id, persistence.Address, persistence.UseType);
            
            acknowledgement = new MessagePayload();
            acknowledgement.SetAcknowledgementId(id);

            persistence.AddOrUpdateMessage(message);
        };

        Because of = () => Subject.InputMessage(acknowledgement);

        It should_remove_the_corresponding_message_from_the_message_store = () =>
            persistence.GetMessages().ShouldNotContain(message);
    }
}