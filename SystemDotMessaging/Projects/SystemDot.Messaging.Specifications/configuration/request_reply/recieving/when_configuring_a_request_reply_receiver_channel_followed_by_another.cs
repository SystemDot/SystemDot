using SystemDot.Ioc;
using SystemDot.Messaging.Channels.RequestReply.Builders;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Configuration.ComponentRegistration;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Transport;
using SystemDot.Parallelism;
using Machine.Fakes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.configuration.request_reply.recieving
{
    [Subject("Request reply configuration")] 
    public class when_configuring_a_request_reply_receiver_channel_followed_by_another : WithConfiguationSubject
    {
        const string Channel2Name = "TestChannel2";

        Establish context = () =>
        {
            IocContainerLocator.SetContainer(new IocContainer(new TypeExtender()));
            ConfigureAndRegister<IMachineIdentifier>(new MachineIdentifier());
            ConfigureAndRegister(new EndpointAddressBuilder(IocContainerLocator.Locate().Resolve<IMachineIdentifier>()));
            ConfigureAndRegister<IRequestRecieveChannelBuilder>();
            ConfigureAndRegister<IReplySendChannelBuilder>();
            ConfigureAndRegister<IMessageReciever>();
            ConfigureAndRegister<IBus>();
        };

        Because of = () => Configuration.Configure.Messaging(IocContainerLocator.Locate())
            .UsingHttpTransport(MessageServer.Local())
                .OpenChannel("TestChannel1").ForRequestReplyRecieving()
                .OpenChannel(Channel2Name).ForRequestReplyRecieving()
            .Initialise();

        It should_register_the_listening_address_of_the_second_channel_with_the_message_reciever = () =>
            The<IMessageReciever>().WasToldTo(r => 
                r.StartPolling(GetEndpointAddress(Channel2Name, The<IMachineIdentifier>().GetMachineName())));
    }
}