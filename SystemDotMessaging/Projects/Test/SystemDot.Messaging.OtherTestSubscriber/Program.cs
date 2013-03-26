﻿using System;
using SystemDot.Esent;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Transport.Http.Configuration;

namespace SystemDot.Messaging.OtherTestSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            IocContainerLocator.Locate().RegisterFromAssemblyOf<Program>();

            Logger.LoggingMechanism = new ConsoleLoggingMechanism { ShowDebug = false };
            Configure.Messaging()
                .UsingHttpTransport()
                .AsAServer("OtherSubscriberServer")
                .UsingFilePersistence()
                .OpenChannel("TestOtherSubscriber")
                    .ForSubscribingTo(string.Format("TestPublisher@{0}/PublisherServer.{0}/PublisherServer", Environment.MachineName))
                    .WithDurability()
                    .RegisterHandlersFromAssemblyOf<Program>()
                    .BasedOn<IMessageConsumer>()
                .Initialise();
            
            Console.WriteLine("I am the other subscriber, listening for messages..");

            Console.ReadLine();
        }
    }

   
}
