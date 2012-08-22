using SystemDot.Messaging.Channels.RequestReply.Builders;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Configuration.ComponentRegistration;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Processing.Filtering;
using SystemDot.Messaging.Transport;
using SystemDot.Parallelism;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.sending
{
    [Subject("Request reply configuration")]
    public class when_configuring_a_request_reply_sender_channel_with_a_specific_namespace_filtering_strategy : WithConfiguationSubject
    {
        Establish context = () =>
        {
            Components.Registration = () =>
            {
                ConfigureAndRegister<IMachineIdentifier>(new MachineIdentifier());
                ConfigureAndRegister(new EndpointAddressBuilder(IocContainer.Resolve<IMachineIdentifier>()));
                ConfigureAndRegister<IRequestSendChannelBuilder>(new TestRequestSendChannelBuilder());
                ConfigureAndRegister<IReplyRecieveChannelBuilder>();
                ConfigureAndRegister<IMessageReciever>();
                ConfigureAndRegister<ITaskLooper>();
                ConfigureAndRegister<IBus>();
            };
        };

        Because of = () => Configuration.Configure.Messaging()
            .UsingHttpTransport(MessageServer.Local())
            .OpenChannel("Test").ForRequestReplySendingTo("TestRecieverAddress").OnlyForMessages(FilteredBy.Namespace("Namespace"))
            .Initialise();

        It should_build_the_send_channel_with_the_correct_namespace_message_filter = () =>
            The<IRequestSendChannelBuilder>().As<TestRequestSendChannelBuilder>().MessageFilter
                .ShouldBeOfType<NamespaceMessageFilterStrategy>();
    }
}