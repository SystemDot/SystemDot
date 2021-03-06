using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Publishing.Builders;
using SystemDot.Storage.Changes;

namespace SystemDot.Messaging.Publishing
{
    class PersistentSubscriberCollection : ChangeRoot, IEnumerable<Subscriber>
    {
        readonly EndpointAddress address;
        readonly ISubscriberSendChannelBuilder builder;
        readonly ConcurrentDictionary<EndpointAddress, Subscriber> subscribers;

        public PersistentSubscriberCollection(
            EndpointAddress address, 
            ChangeStore changeStore, 
            ISubscriberSendChannelBuilder builder, 
            ICheckpointStrategy checkPointStrategy)
            : base(changeStore, checkPointStrategy)
        {
            Contract.Requires(address != null);
            Contract.Requires(address != EndpointAddress.Empty);
            Contract.Requires(changeStore != null);
            Contract.Requires(builder != null);

            this.address = address;
            this.builder = builder;
            this.subscribers = new ConcurrentDictionary<EndpointAddress, Subscriber>();
            
            Id = address.ToString();
            Initialise();
        }

        public void AddSubscriber(SubscriptionSchema schema)
        {
            if (SubscriberExists(schema)) return;
            AddChange(new SubscribeChange { Schema = schema });
        }

        bool SubscriberExists(SubscriptionSchema schema)
        {
            return subscribers.ContainsKey(schema.SubscriberAddress);
        }

        public void ApplyChange(SubscribeChange change)
        {
            var subscriber = new Subscriber(builder);

            if(subscribers.TryAdd(change.Schema.SubscriberAddress, subscriber))
                subscriber.BuildChannel(address, change.Schema);
        }

        public IEnumerator<Subscriber> GetEnumerator()
        {
            return subscribers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override void UrgeCheckPoint()
        {
        }
    }
}