using System;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Messages;
using Machine.Fakes;

namespace SystemDot.Messaging.Specifications.configuration
{
    public class WithConfiguationSubject : WithSubject<object>
    {
        protected static void ConfigureAndRegister<T>() where T : class
        {
            ConfigureAndRegister(The<T>());
        }

        protected static void ConfigureAndRegister<T>(T toSet) where T : class
        {
            Configure(toSet);
            var concrete = The<T>();

            IocContainer.RegisterInstance(() => concrete);
        }

        protected static EndpointAddress GetEndpointAddress(string channelName, string serverName)
        {
            return The<EndpointAddressBuilder>().Build(channelName, serverName);
        }
    }
}