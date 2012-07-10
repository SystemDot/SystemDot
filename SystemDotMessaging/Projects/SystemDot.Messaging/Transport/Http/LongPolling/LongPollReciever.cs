using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemDot.Http;
using SystemDot.Messaging.Messages;
using SystemDot.Messaging.Messages.Packaging;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Transport.Http.LongPolling
{
    public class LongPollReciever : IMessageReciever
    {
        readonly List<EndpointAddress> addresses;
        readonly IWebRequestor requestor;
        readonly ISerialiser formatter;
        
        public event Action<MessagePayload> MessageProcessed;

        public LongPollReciever(IWebRequestor requestor, ISerialiser formatter)
        {
            Contract.Requires(requestor != null);
            Contract.Requires(formatter != null);
            
            this.requestor = requestor;
            this.formatter = formatter;
            this.addresses = new List<EndpointAddress>();
        }

        public void RegisterListeningAddress(EndpointAddress toRegister)
        {
            Contract.Requires(toRegister != EndpointAddress.Empty);
            this.addresses.Add(toRegister);
        }

        public Task Poll()
        {
            return this.requestor.SendPut(
                this.addresses.First().GetUrl(), 
                s => this.formatter.Serialise(s, CreateLongPollPayload(this.addresses)), 
                RecieveResponse);
        }

        MessagePayload CreateLongPollPayload(List<EndpointAddress> toAdd)
        {
            var payload = new MessagePayload();
            payload.SetLongPollRequest(toAdd);

            return payload;
        }

        void RecieveResponse(Stream responseStream)
        {
            var messages = this.formatter.Deserialise(responseStream).As<IEnumerable<MessagePayload>>();

            foreach (var message in messages)
            {
                this.MessageProcessed(message);
            }
        }
    }
}