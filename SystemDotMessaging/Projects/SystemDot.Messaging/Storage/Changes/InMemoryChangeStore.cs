using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Storage.Changes
{
    public class InMemoryChangeStore : IChangeStore
    {
        readonly ConcurrentDictionary<Guid, ChangeContainer> changes;
        readonly ISerialiser serialiser;

        public InMemoryChangeStore(ISerialiser serialiser)
        {
            this.serialiser = serialiser;
            this.changes = new ConcurrentDictionary<Guid, ChangeContainer>();
        }

        public Guid StoreChange(string changeRootId, Change change)
        {
            var id = Guid.NewGuid();
            this.changes.TryAdd(id, new ChangeContainer(changeRootId, this.serialiser.Serialise(change)));
            return id;
        }

        public Change GetChange(Guid id)
        {
            return DerserialiseChange(this.changes[id]);
        }

        public IEnumerable<Change> GetChanges(string changeRootId)
        {
            return this.changes.Values
                .Where(c => c.ChangeRootId == changeRootId)
                .Select(DerserialiseChange);
        }

        Change DerserialiseChange(ChangeContainer container)
        {
            return this.serialiser.Deserialise(container.Change).As<Change>();
        }
    }
}