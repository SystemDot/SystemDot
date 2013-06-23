using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Packaging;

namespace SystemDot.Messaging.Transport.Http.Remote.Clients
{
    public static class MessagePayloadLongPollRequestExtensions
    {
        public static void SetLongPollRequest(this MessagePayload payload, ServerRoute serverRoute)
        {
            payload.AddHeader(new LongPollRequestHeader(serverRoute));
        }

        public static bool IsLongPollRequest(this MessagePayload payload)
        {
            return payload.HasHeader<LongPollRequestHeader>();
        }

        public static ServerRoute GetLongPollRequestServerPath(this MessagePayload payload)
        {
            return payload.GetHeader<LongPollRequestHeader>().ServerRoute;
        }
    }
}