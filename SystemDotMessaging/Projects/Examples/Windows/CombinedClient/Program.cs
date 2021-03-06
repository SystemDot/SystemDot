﻿using System;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging;
using SystemDot.Messaging.Configuration;
using Messages;

namespace CombinedClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new IocContainer();
            container.RegisterFromAssemblyOf<Program>();

            Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .ResolveReferencesWith(container)
                .RegisterHandlersFromContainer().BasedOn<IMessageConsumer>()
                .UsingFilePersistence()
                .UsingHttpTransport()
                    .AsAServer("SenderServer")
                .OpenChannel("TestSender")
                    .ForRequestReplySendingTo("TestReceiver@ReceiverPublisherServer")
                .Initialise();

            do
            {
                Console.WriteLine("I am the client. Press enter to send messages..");
                Console.ReadLine();
                
                Console.WriteLine("Sending messages");

                Bus.Send(new TestMessage("Hello"));
                Bus.Send(new TestMessage("Hello1"));
                Bus.Send(new TestMessage("Hello2"));
                Bus.Send(new TestMessage("Hello3"));
                Bus.Send(new TestMessage("Hello4"));
                Bus.Send(new TestMessage("Hello5"));
                Bus.Send(new TestMessage("Hello6"));
                Bus.Send(new TestMessage("Hello7"));
            }
            while (true);
        }
    }
}
