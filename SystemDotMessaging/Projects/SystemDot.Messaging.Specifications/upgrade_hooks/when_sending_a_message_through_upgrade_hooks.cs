﻿using SystemDot.Messaging.Hooks.Upgrading;
using SystemDot.Messaging.Packaging;
using FluentAssertions;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.upgrade_hooks
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_message_through_upgrade_hooks : WithHttpConfigurationSubject
    {
        const string ServerName = "ServerName";
        static OriginalMessage message;

        Establish context = () =>
        {
            message = new OriginalMessage();

            Configuration.Configure.Messaging()
                .UsingHttpTransport()
                .AsAServer(ServerName)
                .OpenChannel("TestSender")
                .ForRequestReplySendingTo("TestReceiver")
                .WithSendHook(UpgradeMessageHook.LoadUp())
                .Initialise();
        };

        Because of = () => Bus.Send(message);

        It should_send_it_as_the_upgraded_message = () =>
            WebRequestor.DeserialiseSingleRequest<MessagePayload>()
                .DeserialiseTo<UpgradedMessage>().Should().NotBeNull();
    }
}