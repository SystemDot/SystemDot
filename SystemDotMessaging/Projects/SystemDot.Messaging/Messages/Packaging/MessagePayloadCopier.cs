using System.Diagnostics.Contracts;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Messages.Packaging
{
    public class MessagePayloadCopier
    {
        readonly ISerialiser serialiser;

        public MessagePayloadCopier(ISerialiser serialiser)
        {
            this.serialiser = serialiser;
        }

        public MessagePayload Copy(MessagePayload toCopy)
        {
            Contract.Requires(toCopy != null);
            return serialiser.Deserialise(serialiser.Serialise(toCopy)).As<MessagePayload>();
        }
    }
}