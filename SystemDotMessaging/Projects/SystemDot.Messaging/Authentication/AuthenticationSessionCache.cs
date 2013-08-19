using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using SystemDot.Messaging.Addressing;

namespace SystemDot.Messaging.Authentication
{
    public class AuthenticationSessionCache
    {
        readonly ConcurrentDictionary<MessageServer, Guid> sessions;

        public AuthenticationSessionCache()
        {
            sessions = new ConcurrentDictionary<MessageServer, Guid>();
        }

        public Guid GetCurrentSessionFor(MessageServer forServer)
        {
            Contract.Requires(forServer != null);
            
            Guid temp;

            sessions.TryGetValue(forServer, out temp);

            return temp;
        }

        public void CacheNewSessionFor(MessageServer forServer)
        {
            CacheSessionFor(forServer, Guid.NewGuid());
        }

        public void CacheSessionFor(MessageServer forServer, Guid session)
        {
            Contract.Requires(forServer != null);
            Contract.Requires(session != Guid.Empty);

            sessions.TryAdd(forServer, session);
        }

        public bool HasCurrentSessionFor(MessageServer forServer)
        {
            Contract.Requires(forServer != null);

            return forServer != MessageServer.None && sessions.ContainsKey(forServer);
        }
    }
}