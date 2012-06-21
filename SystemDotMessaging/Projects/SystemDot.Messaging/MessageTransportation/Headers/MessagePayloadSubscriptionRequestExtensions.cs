using System.Diagnostics.Contracts;
using System.Linq;
using SystemDot.Messaging.Channels.PubSub;

namespace SystemDot.Messaging.MessageTransportation.Headers
{
    public static class MessagePayloadSubscriptionRequestExtensions
    {
        public static void SetSubscriptionRequest(this MessagePayload payload, SubscriptionSchema schema)
        {
            Contract.Requires(schema != null);

            payload.AddHeader(new SubscriptionRequestHeader(schema));
        }

        public static bool IsSubscriptionRequest(this MessagePayload payload)
        {
            return payload.Headers.OfType<SubscriptionRequestHeader>().Any();
        }

        public static SubscriptionSchema GetSubscriptionRequestSchema(this MessagePayload payload)
        {
            Contract.Requires(payload.IsSubscriptionRequest());

            return payload.Headers.OfType<SubscriptionRequestHeader>().Single().Schema;
        }
    }
}