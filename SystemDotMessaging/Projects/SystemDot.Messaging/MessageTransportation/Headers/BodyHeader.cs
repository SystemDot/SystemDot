using System;
using System.Diagnostics.Contracts;

namespace SystemDot.Messaging.MessageTransportation.Headers
{
    [Serializable]
    public class BodyHeader : IMessageHeader
    {
        public byte[] Body { get; private set; }

        public BodyHeader(byte[] body)
        {
            Contract.Requires(body != null);
            Body = body;
        }
    }
}