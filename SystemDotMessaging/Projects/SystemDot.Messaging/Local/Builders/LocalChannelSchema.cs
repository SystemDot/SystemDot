namespace SystemDot.Messaging.Local.Builders
{
    public class LocalChannelSchema
    {
        public IMessageProcessor<object, object> UnitOfWorkRunner { get; set; }
    }
}