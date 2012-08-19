using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SystemDot.Http;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Messaging.Messages.Packaging.Headers;
using SystemDot.Messaging.Transport.Http.LongPolling;
using SystemDot.Parallelism;
using SystemDot.Serialisation;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.transport.long_polling
{
    [Subject("Long polling")]
    public class when_long_polling_for_messages_with_addresses_that_are_not_in_the_queue_ : WithSubject<LongPollReciever>
    {
        static List<MessagePayload> messagePayloads;
        static MessagePayload messagePayload;
            
        Establish context = () =>
        {
            messagePayloads = new List<MessagePayload>();
            messagePayload = new MessagePayload();
            messagePayload.SetToAddress(new EndpointAddress("Address1", "TestServer"));

            Configure<ITaskLooper>(new TestTaskLooper());
            Configure<ISerialiser>(new PlatformAgnosticSerialiser());

            var requestor = new TestWebRequestor(The<ISerialiser>(), new FixedPortAddress());
            Configure<IWebRequestor>(requestor); 
            requestor.AddMessages(messagePayload);
            
            Subject.MessageProcessed += payload => messagePayloads.Add(payload);
            Subject.RegisterListeningAddress(new EndpointAddress("Address2", "TestServer"));
        };

        Because of = () => The<ITaskLooper>().Start();

        It should_output_any_messages_from_the_queue = () => messagePayloads.ShouldBeEmpty();
    }
}