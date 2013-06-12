using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;

namespace SystemDot.Messaging.Packaging.Headers
{
    public static class MessagePayloadToAddressExtensions
    {
        public static void SetFromAddress(this MessagePayload payload, EndpointAddress address)
        {
            Contract.Requires(address != null);
            payload.AddHeader(new FromAddressHeader(address));
        }

        public static EndpointAddress GetFromAddress(this MessagePayload payload)
        {
            return payload.GetHeader<FromAddressHeader>().Address;
        }

        public static void SetToAddress(this MessagePayload payload, EndpointAddress address)
        {
            Contract.Requires(address != null);
            payload.AddHeader(new ToAddressHeader(address));
        }

        public static EndpointAddress GetToAddress(this MessagePayload payload)
        {
            return payload.GetHeader<ToAddressHeader>().Address;
        }

        public static FromServerAddressHeader GetFromServerAddress(this MessagePayload payload)
        {
            return payload.GetHeader<FromServerAddressHeader>();
        }

        public static bool HasFromServerAddress(this MessagePayload payload)
        {
            return payload.HasHeader<FromServerAddressHeader>();
        }

        public static void SetFromServerAddress(this MessagePayload payload, ServerAddress address)
        {
            Contract.Requires(address != null);
            payload.AddHeader(new FromServerAddressHeader(address));
        }

    }
}