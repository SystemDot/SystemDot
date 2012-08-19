using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Messaging.Messages.Packaging.Headers;
using SystemDot.Messaging.Transport.Http.LongPolling;
using SystemDot.Messaging.Transport.Http.LongPolling.Servers;
using SystemDot.Serialisation;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.transport.long_polling.serving
{
    [Subject("Message serving")]
    public class when_handling_a_long_poll_request_with_a_different_address_to_the_queued_message
    {
        static HttpMessagingServer server;
        static ISerialiser formatter; 
        static MemoryStream inputStream;
        static MemoryStream outputStream;
        static MessagePayload sentMessageInQueue;
        static MessagePayload longPollRequest;
        static MessagePayloadQueue outgoingQueue;

        Establish context = () =>
        {
            formatter = new PlatformAgnosticSerialiser();
            outgoingQueue = new MessagePayloadQueue(new TimeSpan(0, 0, 0));
            inputStream = new MemoryStream();
            outputStream = new MemoryStream();
            
            server = new HttpMessagingServer(
                formatter,
                new SentMessageHandler(outgoingQueue),
                new LongPollHandler(outgoingQueue));

            sentMessageInQueue = new MessagePayload();
            sentMessageInQueue.SetToAddress(new EndpointAddress("Address1", "TestServer"));
            outgoingQueue.Enqueue(sentMessageInQueue);

            longPollRequest = new MessagePayload();
            longPollRequest.SetLongPollRequest(new EndpointAddress("Address2", "TestServer"));

            inputStream.Serialise(longPollRequest, formatter);
        };

        Because of = () => server.HandleRequest(inputStream, outputStream);

        It should_not_output_the_message_in_the_response_stream = () =>
            outputStream.Deserialise<IEnumerable<MessagePayload>>(formatter).ShouldBeEmpty();        
        
    }
}