﻿using System;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Channels.Handling;
using SystemDot.Messaging.Configuration;

namespace SystemDot.Messaging.MultiChannel.RecieverA
{
    class Program
    {
        static void Main(string[] args)
        {
            IBus bus = Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .UsingHttpTransport(MessageServer.Local())
                .OpenChannel("TestRecieverA").ForRequestReplyRecieving()
                .Initialise();

            IocContainerLocator.Locate().Resolve<MessageHandlerRouter>().RegisterHandler(new MessageConsumer(bus));

            Console.WriteLine("I am reciever A");
            Console.ReadLine();                
        }
    }
}
