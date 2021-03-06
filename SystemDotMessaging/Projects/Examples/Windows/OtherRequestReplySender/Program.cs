﻿using System;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging;
using SystemDot.Messaging.Batching;
using SystemDot.Messaging.Configuration;
using Messages;

namespace OtherRequestReplySender
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new IocContainer();
            container.RegisterFromAssemblyOf<Program>();

            Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false, ShowDebug = false })
                .UsingFilePersistence()
                .ResolveReferencesWith(container)
                .RegisterHandlersFromContainer().BasedOn<IMessageConsumer>()
                .UsingHttpTransport()
                    .AsAServer("OtherSenderServer")
                .OpenChannel("OtherTestRequest")
                    .ForRequestReplySendingTo("TestReply@ReceiverServer")
                    .WithDurability()
                .Initialise();

            do
            {
                Console.WriteLine("I am the sender. Press enter to send messages..");
                Console.ReadLine();

                Console.WriteLine("Sending messages");

                using (Batch batch = Bus.BatchSend())
                {
                    Bus.Send(new TestMessage("Hello"));
                    Bus.Send(new TestMessage("Hello1"));
                    Bus.Send(new TestMessage("Hello2"));
                    Bus.Send(new TestMessage("Hello3"));
                    Bus.Send(new TestMessage("Hello4"));
                    Bus.Send(new TestMessage("Hello5"));
                    Bus.Send(new TestMessage("Hello6"));
                    Bus.Send(new TestMessage("Hello7"));

                    batch.Complete();
                }
            }
            while (true);
        }
    }
}
