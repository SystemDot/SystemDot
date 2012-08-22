﻿using System;
using SystemDot.Logging;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Configuration.HttpMessaging;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Messages.Processing.Handling;
using SystemDot.Messaging.Test.Messages;

namespace SystemDot.Messaging.TestRequestReply.OtherSender
{
    class Program
    {
        static void Main(string[] args)
        {
            IBus bus = Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .UsingHttpTransport(MessageServer.Local())
                .OpenChannel("TestOtherSender").ForRequestReplySendingTo("TestReciever")
                .Initialise();

            IocContainer.Resolve<MessageHandlerRouter>().RegisterHandler(new MessageConsumer());

            do
            {
                Console.WriteLine("I am the other sender. Press enter to send messages..");
                Console.ReadLine();
                
                Console.WriteLine("Sending messages");

                bus.Send(new TestMessage("Other Hello"));
                bus.Send(new TestMessage("Other Hello1"));
                bus.Send(new TestMessage("Other Hello2"));
                bus.Send(new TestMessage("Other Hello3"));
                bus.Send(new TestMessage("Other Hello4"));
                bus.Send(new TestMessage("Other Hello5"));
                bus.Send(new TestMessage("Other Hello6"));
                bus.Send(new TestMessage("Other Hello7"));
            }
            while (true);
        }
    }
}
