﻿using System.Collections.Generic;
using System.Linq;
using SystemDot.Messaging.Acknowledgement;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Packaging.Headers;
using SystemDot.Messaging.Storage;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.point_to_point.load_balancing
{
    [Subject(SpecificationGroup.Description)]
    public class when_receiving_an_acknowledgement_for_one_of_twenty_one_sent_messages_but_on_a_different_address : WithMessageConfigurationSubject
    {
        const string SenderAddress = "SenderAddress";
        const string RecieverAddress = "ReceiverAddress";
        const string OtherSenderAddress = "OtherSenderAddress";
        const string OtherRecieverAddress = "OtherReceiverAddress";

        static List<int> messages;
        static IBus bus;
        static MessagePayload acknowledgement;

        Establish context = () =>
        {
            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel(SenderAddress)
                    .ForPointToPointSendingTo(RecieverAddress)
                .OpenChannel(OtherSenderAddress)
                    .ForPointToPointSendingTo(OtherRecieverAddress)
                        .OnlyForMessages(FilteredBy.NamePattern("NotInt"))
                .Initialise();

            messages = Enumerable.Range(1, 21).ToList();
            messages.ForEach(m => bus.Send(m));

            acknowledgement = new MessagePayload();

            acknowledgement.SetAcknowledgementId(new MessagePersistenceId(
                Server.SentMessages.First().Id,
                BuildAddress(OtherSenderAddress), 
                PersistenceUseType.PointToPointSend));

            acknowledgement.SetToAddress(Server.SentMessages.First().GetFromAddress());
        };

        Because of = () => Server.ReceiveMessage(acknowledgement);

        It should_not_send_the_twenty_first_message = () => Server.SentMessages.Count.ShouldEqual(20);
    }
}