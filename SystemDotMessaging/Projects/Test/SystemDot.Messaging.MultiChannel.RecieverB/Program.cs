﻿using System;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Channels.Handling;
using SystemDot.Messaging.Configuration;

namespace SystemDot.Messaging.MultiChannel.RecieverB
{
    class Program
    {
        static void Main(string[] args)
        {
            IBus bus = Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .UsingHttpTransport(MessageServer.Local())
                .OpenChannel("TestRecieverB").ForRequestReplyRecieving()
                .Initialise();

            IocContainerLocator.Locate().Resolve<MessageHandlerRouter>().RegisterHandler(new MessageConsumer(bus));

            Console.WriteLine("I am reciever B");
            Console.ReadLine();                
        }
    }
}
