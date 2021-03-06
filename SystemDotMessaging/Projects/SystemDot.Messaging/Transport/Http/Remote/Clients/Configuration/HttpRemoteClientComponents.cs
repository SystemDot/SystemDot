using System.Diagnostics.Contracts;
using SystemDot.Ioc;

namespace SystemDot.Messaging.Transport.Http.Remote.Clients.Configuration
{
    static class HttpRemoteClientComponents
    {
        public static void Configure(IIocContainer container)
        {
            Contract.Requires(container != null);

            container.RegisterInstance<IMessageTransporter, MessageTransporter>();
            container.RegisterInstance<LongPoller, LongPoller>();
            container.RegisterInstance<ITransportBuilder, HttpRemoteClientTransportBuilder>();
        }
    }
}