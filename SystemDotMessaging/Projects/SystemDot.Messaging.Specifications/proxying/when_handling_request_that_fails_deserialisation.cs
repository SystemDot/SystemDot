using System;
using SystemDot.Serialisation;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.proxying
{
    [Subject(SpecificationGroup.Description)]
    public class when_handling_request_that_fails_deserialisation : WithHttpServerConfigurationSubject
    {
        static Exception exception;
        
        Establish context = () =>
        {
            ConfigureAndRegister<ISerialiser>(new FailingSerialiser());

            Configuration.Configure.Messaging()
                .UsingHttpTransport()
                .AsAProxyFor("RemoteServerName")
                .Initialise();
        };

        Because of = () => exception = Catch.Exception(() => SendObjectToServer(new object()));

        It should_not_fail = () => exception.ShouldBeNull();
    }
}