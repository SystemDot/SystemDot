﻿using System;
using SystemDot.Esent;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Handling;
using SystemDot.Messaging.Test.Messages;
using SystemDot.Messaging.Transport.Http.Configuration;

namespace SystemDot.Messaging.MultiChannel.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new IocContainer();
            container.RegisterFromAssemblyOf<Program>();

            Configure.Messaging()
                .LoggingWith(new ConsoleLoggingMechanism { ShowInfo = false })
                .UsingFilePersistence()
                .UsingHttpTransport()
                .AsAServer("Server")
                .OpenChannel("TestSenderA")
                    .ForRequestReplySendingTo("TestRecieverA@/ServerA")
                    .OnlyForMessages(FilteredBy.NamePattern("Channel1"))
                    .WithDurability()
                    .RegisterHandlersFromAssemblyOf<Program>()
                    .BasedOn<IMessageConsumer>()
                    .ResolveBy(container.Resolve)
                .OpenChannel("TestSenderB")
                    .ForRequestReplySendingTo("TestRecieverB@/ServerB")
                    .OnlyForMessages(FilteredBy.NamePattern("Channel2"))
                    .WithDurability()
                    .RegisterHandlersFromAssemblyOf<Program>()
                    .BasedOn<IMessageConsumer>()
                    .ResolveBy(container.Resolve)
                .Initialise();

            do
            {
                Console.WriteLine("I am the sender. Press A to send messages to reciever A, press B to send messages to reciever B..");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.A)
                {
                    Console.WriteLine("Sending messages to GetChannel A");

                    Bus.Send(new Channel1Request("GetChannel A Hello"));
                    Bus.Send(new Channel1Request("GetChannel A Hello1"));
                    Bus.Send(new Channel1Request("GetChannel A Hello2"));
                    Bus.Send(new Channel1Request("GetChannel A Hello3"));
                    Bus.Send(new Channel1Request("GetChannel A Hello4"));
                    Bus.Send(new Channel1Request("GetChannel A Hello5"));
                    Bus.Send(new Channel1Request("GetChannel A Hello6"));
                    Bus.Send(new Channel1Request("GetChannel A Hello7"));
                }

                if (key.Key == ConsoleKey.B)
                {
                    Console.WriteLine("Sending messages to GetChannel B");

                    Bus.Send(new Channel2Request("GetChannel B Hello"));
                    Bus.Send(new Channel2Request("GetChannel B Hello1"));
                    Bus.Send(new Channel2Request("GetChannel B Hello2"));
                    Bus.Send(new Channel2Request("GetChannel B Hello3"));
                    Bus.Send(new Channel2Request("GetChannel B Hello4"));
                    Bus.Send(new Channel2Request("GetChannel B Hello5"));
                    Bus.Send(new Channel2Request("GetChannel B Hello6"));
                    Bus.Send(new Channel2Request("GetChannel B Hello7"));
                }
            }
            while (true);
        }
    }
}
