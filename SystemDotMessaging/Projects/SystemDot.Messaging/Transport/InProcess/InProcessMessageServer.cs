using System;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;

namespace SystemDot.Messaging.Transport.InProcess
{
    public class InProcessMessageServer : IMessageProcessor<MessagePayload, MessagePayload>
    {
        public void InputMessage(MessagePayload toInput)
        {
            MessageProcessed(toInput);
        }

        public event Action<MessagePayload> MessageProcessed;
    }
}