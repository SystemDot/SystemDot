﻿using System;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Channels.Handling;
using SystemDot.Messaging.Configuration;

namespace SystemDot.Messaging.Combined.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            IBus bus = Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .UsingHttpTransport(MessageServer.Local())
                //.UsingEsentPersistence("Esent\\Subscriber")
                .OpenChannel("TestSubscriber").ForSubscribingTo("TestPublisher")
                .Initialise();

            IocContainerLocator.Locate().Resolve<MessageHandlerRouter>().RegisterHandler(new MessageConsumer());
            
            Console.WriteLine("I am a subscriber, listening for messages..");

            Console.ReadLine();
        }
    }

   
}
