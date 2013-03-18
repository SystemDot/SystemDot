using SystemDot.Messaging.Addressing;

namespace SystemDot.Messaging.RequestReply.Builders
{
    public class RequestSendChannelBuilt
    {
        public EndpointAddress CacheAddress { get; set; }

        public EndpointAddress SenderAddress { get; set; }

        public EndpointAddress ReceiverAddress { get; set; }
    }
}