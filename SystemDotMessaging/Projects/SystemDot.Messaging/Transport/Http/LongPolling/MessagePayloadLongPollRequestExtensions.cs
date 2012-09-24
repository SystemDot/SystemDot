using System.Collections.Generic;
using System.Linq;
using SystemDot.Messaging.Channels;
using SystemDot.Messaging.Channels.Packaging;

namespace SystemDot.Messaging.Transport.Http.LongPolling
{
    public static class MessagePayloadLongPollRequestExtensions
    {
        public static void SetLongPollRequest(this MessagePayload payload, EndpointAddress address)
        {
            payload.AddHeader(new LongPollRequestHeader(address));
        }

        public static bool IsLongPollRequest(this MessagePayload payload)
        {
            return payload.Headers.OfType<LongPollRequestHeader>().Any();
        }

        public static EndpointAddress GetLongPollRequestAddress(this MessagePayload payload)
        {
            return payload.Headers.OfType<LongPollRequestHeader>().Single().Address;
        }
    }
}