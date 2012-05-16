using System;
using System.Collections.Generic;
using SystemDot.Messaging.Threading;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.blocking_queue
{
    public class when_queuing_an_item
    {
        static BlockingQueue<string> queue;
        static IEnumerable<string> dequeued;

        Establish context = () =>
        {
            queue = new BlockingQueue<string>(new TimeSpan(0, 0, 1));
            queue.Enqueue("Test");
        };

        Because of = () => dequeued = queue.DequeueAll();

        It should_be_able_to_be_dequeued_from_the_queue = () => dequeued.ShouldContain("Test");
    }
}