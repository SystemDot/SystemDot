﻿using System;
using SystemDot.Messaging.Configuration;
using SystemDot.Messaging.Test.Messages;

namespace SystemDot.Messaging.TestRequestReply.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            IBus bus = Configure.WithLocalMessageServer()
                .OpenChannel("TestSender")
                .AsRequestReplyReciever()
                .Initialise();

            do
            {
                Console.WriteLine("I am the sender. Press enter to send messages..");
                Console.ReadLine();
                
                Console.WriteLine("Sending message");

                bus.Send(new TestMessage("Hello"));
                bus.Send(new TestMessage("Hello1"));
                bus.Send(new TestMessage("Hello2"));
                bus.Send(new TestMessage("Hello3"));
                bus.Send(new TestMessage("Hello4"));
                bus.Send(new TestMessage("Hello5"));
                bus.Send(new TestMessage("Hello6"));
                bus.Send(new TestMessage("Hello7"));
            }
            while (true);
        }
    }
}
