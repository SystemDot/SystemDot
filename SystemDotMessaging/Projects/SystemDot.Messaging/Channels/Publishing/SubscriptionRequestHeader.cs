using System.Diagnostics.Contracts;
using SystemDot.Messaging.Messages.Packaging;

namespace SystemDot.Messaging.Channels.Publishing
{
    public class SubscriptionRequestHeader : IMessageHeader 
    {
        public SubscriptionSchema Schema { get; set; }

        public SubscriptionRequestHeader() {}

        public SubscriptionRequestHeader(SubscriptionSchema schema) 
        {
            Contract.Requires(schema != null);
            Schema = schema;
        }
    }
}