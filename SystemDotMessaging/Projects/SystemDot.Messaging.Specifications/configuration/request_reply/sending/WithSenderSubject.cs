using SystemDot.Messaging.Channels.RequestReply.Builders;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Transport;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.sending
{
    public class WithSenderSubject : WithConfiguationSubject
    {
        Establish context = () =>
        {
            ConfigureAndRegister(new EndpointAddressBuilder());
            ConfigureAndRegister<IRequestSendChannelBuilder>(new TestRequestSendChannelBuilder());
            ConfigureAndRegister<IReplyRecieveChannelBuilder>();
            ConfigureAndRegister<IMessageReciever>();
            ConfigureAndRegister<IBus>();
        };
    }
}