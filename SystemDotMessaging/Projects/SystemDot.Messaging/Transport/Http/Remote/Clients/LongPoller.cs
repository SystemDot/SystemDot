using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using SystemDot.Http;
using SystemDot.Logging;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Packaging;
using SystemDot.Parallelism;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Transport.Http.Remote.Clients
{
    class LongPoller
    {
        readonly IWebRequestor requestor;
        readonly ISerialiser formatter;
        readonly ITaskScheduler scheduler;
        readonly MessageReceiver receiver;
        readonly ITaskStarter starter;

        public LongPoller(
            IWebRequestor requestor, 
            ISerialiser formatter, 
            ITaskScheduler scheduler, 
            MessageReceiver receiver, 
            ITaskStarter starter)
        {
            Contract.Requires(requestor != null);
            Contract.Requires(formatter != null);
            Contract.Requires(scheduler != null);
            Contract.Requires(receiver != null);
            Contract.Requires(starter != null);

            this.requestor = requestor;
            this.formatter = formatter;
            this.scheduler = scheduler;
            this.receiver = receiver;
            this.starter = starter;
        }

        public void ListenTo(MessageServer toListenFor)
        {
            Contract.Requires(toListenFor != null);

            StartNextPoll(toListenFor);
        }

        void Poll(MessageServer toListenFor)
        {
            Logger.Debug("Long polling for messages for {0}", toListenFor);

            this.requestor.SendPut(
                toListenFor.GetUrl(),
                requestStream => this.formatter.Serialise(requestStream, CreateLongPollPayload(toListenFor)),
                RecieveResponse,
                e =>
                {
                    Logger.Info("Cannot long poll server: {0}", e.Message);
                    StartNextPoll(toListenFor, TimeSpan.FromSeconds(4));
                },
                () => StartNextPoll(toListenFor));
        }

        void StartNextPoll(MessageServer toListenFor)
        {
            this.starter.StartTask(() => Poll(toListenFor));
        }

        void StartNextPoll(MessageServer toListenFor, TimeSpan after)
        {
            this.scheduler.ScheduleTask(after,() => Poll(toListenFor));
        }

        MessagePayload CreateLongPollPayload(MessageServer toListenFor)
        {
            var payload = new MessagePayload();
            payload.SetLongPollRequest(toListenFor);

            return payload;
        }

        void RecieveResponse(Stream responseStream)
        {
            var messages = this.formatter.Deserialise(responseStream).As<IEnumerable<MessagePayload>>();

            Logger.Debug("Recieved {0} messages from long poll", messages.Count());
                
            foreach (var message in messages)
            {
                receiver.InputMessage(message);
            }
        }
    }
}