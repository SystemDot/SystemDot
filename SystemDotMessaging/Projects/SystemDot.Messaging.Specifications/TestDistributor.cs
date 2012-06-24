using System.Collections.Generic;
using SystemDot.Messaging.Channels.Messages;
using SystemDot.Messaging.Channels.Messages.Distribution;
using SystemDot.Messaging.MessageTransportation;

namespace SystemDot.Messaging.Specifications
{
    public class TestDistributor : IDistributor 
    {
        public List<IMessageInputter<MessagePayload>> Subscribers { get; private set; }

        public TestDistributor()
        {
            Subscribers = new List<IMessageInputter<MessagePayload>>();
        }

        public void Subscribe(IMessageInputter<MessagePayload> toSubscribe)
        {
            this.Subscribers.Add(toSubscribe);
        }

        public void InputMessage(MessagePayload toInput)
        {
            throw new System.NotImplementedException();
        }
    }
}