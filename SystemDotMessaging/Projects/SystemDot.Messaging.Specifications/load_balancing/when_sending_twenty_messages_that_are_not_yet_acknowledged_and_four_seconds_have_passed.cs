﻿using System;
using System.Linq;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.load_balancing
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_twenty_messages_that_are_not_yet_acknowledged_and_four_seconds_have_passed 
        : WithMessageConfigurationSubject
    {
        Establish context = () =>
        {
            Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenChannel("SenderAddress")
                .ForPointToPointSendingTo("ReceiverAddress")
                .Initialise();

            var messages = Enumerable.Range(1, 20).ToList();

            messages.ForEach(m => Bus.Send(m));
        };

        Because of = () => SystemTime.AdvanceTime(TimeSpan.FromSeconds(4));

        It should_repeat_the_messages_to_feel_out_the_connection = () => GetServer().SentMessages.Count.ShouldEqual(40);
    }
}
