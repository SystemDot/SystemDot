using System.Diagnostics.Contracts;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Messaging.Messages.Packaging.Headers;
using SystemDot.Messaging.Messages.Processing.Caching;

namespace SystemDot.Messaging.Messages.Processing.Acknowledgement
{
    public class MessageAcknowledgementHandler : IMessageInputter<MessagePayload>
    {
        readonly IMessageCache cache;
        readonly EndpointAddress address;

        public MessageAcknowledgementHandler(IMessageCache cache, EndpointAddress address)
        {
            Contract.Requires(cache != null);

            this.cache = cache;
            this.address = address;
        }

        public void InputMessage(MessagePayload toInput)
        {
            if (!toInput.IsAcknowledgement()) return;
            if (toInput.GetToAddress() != this.address) return;
            
            this.cache.Remove(toInput.GetAcknowledgementId());
        }
    }
}