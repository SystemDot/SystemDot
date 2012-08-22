﻿using System;
using SystemDot.Logging;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Messages.Processing.Handling;

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

            IocContainer.Resolve<MessageHandlerRouter>().RegisterHandler(new MessageConsumer(bus));

            Console.WriteLine("I am reciever B");
            Console.ReadLine();                
        }
    }
}
