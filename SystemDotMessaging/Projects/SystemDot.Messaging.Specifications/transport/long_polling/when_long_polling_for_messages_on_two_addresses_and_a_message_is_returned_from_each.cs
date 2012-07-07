using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Messaging.Messages.Packaging.Headers;
using SystemDot.Messaging.Transport.Http;
using SystemDot.Messaging.Transport.Http.LongPolling;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.transport.long_polling
{
    [Subject("Long polling")]
    public class when_long_polling_for_messages_on_two_addresses_and_a_message_is_returned_from_each
    {
        static TestWebRequestor requestor;
        static BinaryFormatter formatter;
        static LongPollReciever reciever;
        static List<MessagePayload> messagePayloads;
        static MessagePayload messagePayload1;
        static MessagePayload messagePayload2;
            
        Establish context = () =>
        {
            messagePayloads = new List<MessagePayload>();
            messagePayload1 = new MessagePayload();
            messagePayload1.SetToAddress(new EndpointAddress("Node1@Server1"));
            messagePayload2 = new MessagePayload();
            messagePayload2.SetToAddress(new EndpointAddress("Node2@Server2"));
            
            formatter = new BinaryFormatter();
            requestor = new TestWebRequestor(formatter);
            requestor.AddMessages(messagePayload1.GetToAddress().GetUrl(), messagePayload1);
            requestor.AddMessages(messagePayload2.GetToAddress().GetUrl(), messagePayload2);

            reciever = new LongPollReciever(requestor, formatter);
            reciever.RegisterListeningAddress(messagePayload1.GetToAddress());
            reciever.RegisterListeningAddress(messagePayload2.GetToAddress());
            reciever.MessageProcessed += payload => messagePayloads.Add(payload);
        };

        Because of = () => reciever.Poll();

        It should_output_the_first_recieved_message = () =>
            messagePayloads.First().GetToAddress().ShouldEqual(messagePayload1.GetToAddress());

        It should_output_the_second_recieved_message = () =>
             messagePayloads.Last().GetToAddress().ShouldEqual(messagePayload2.GetToAddress());
    }
}