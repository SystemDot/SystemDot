using SystemDot.Ioc;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Pipelines;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications
{
    public class WithConfigurationSubject : WithSubject<object>
    {
        Establish context = () =>
        {
            ResetIoc();
            MessagePipelineBuilder.BuildSynchronousPipelines = true;
        };

        protected static void ResetIoc()
        {
            IocContainerLocator.SetContainer(new IocContainer());
        }

        Cleanup after = () => IocContainerLocator.SetContainer(null);
        
        protected static void ConfigureAndRegister<T>() where T : class
        {
            ConfigureAndRegister(The<T>());
        }

        protected static void ConfigureAndRegister<T>(T toSet) where T : class
        {
            Configure(toSet);
            var concrete = The<T>();

            Register(concrete);
        }

        protected static void Register<T>(T concrete) where T : class
        {
            Register<T>(IocContainerLocator.Locate(), concrete);
        }
        
        protected static void Register<T>(IIocContainer container, T concrete) where T : class
        {
            container.RegisterInstance(() => concrete);
        }

        protected static T Resolve<T>() where T : class
        {
            return IocContainerLocator.Locate().Resolve<T>();
        }

        protected static EndpointAddress BuildAddress(string toBuild)
        {
            return new EndpointAddress(toBuild, ServerPath.None);
        }

        protected static EndpointAddress GetEndpointAddress(string channelName, string serverName)
        {
            return TestEndpointAddressBuilder.Build(channelName, serverName);
        }
    }
}