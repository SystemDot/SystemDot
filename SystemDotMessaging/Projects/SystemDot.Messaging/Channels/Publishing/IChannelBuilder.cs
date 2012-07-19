using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;

namespace SystemDot.Messaging.Channels.Publishing
{
    public interface IChannelBuilder 
    {
        IMessageInputter<MessagePayload> Build(SubscriptionSchema toSchema);
    }
}