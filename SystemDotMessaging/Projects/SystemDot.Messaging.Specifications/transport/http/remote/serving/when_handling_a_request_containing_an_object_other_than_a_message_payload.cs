using System;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.transport.http.remote.serving
{
    [Subject(SpecificationGroup.Description)]
    public class when_handling_a_request_containing_an_object_other_than_a_message_payload
        : WithServerConfigurationSubject
    {
        static Exception exception;

        Establish context = () => Configuration.Configure.Messaging()
            .UsingHttpTransport()
            .AsAProxy("RemoteServerName")
            .Initialise();

        Because of = () => exception = Catch.Exception(() => SendObjectsToServer(new object()));

        It should_not_fail = () => exception.ShouldBeNull();         
    }
}