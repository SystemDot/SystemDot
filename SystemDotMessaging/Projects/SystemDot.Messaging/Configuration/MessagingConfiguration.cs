using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SystemDot.Ioc;
using SystemDot.Logging;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Transport.Http.Configuration;
using SystemDot.Messaging.Transport.InProcess.Configuration;
using SystemDot.Messaging.UnitOfWork;

namespace SystemDot.Messaging.Configuration
{
    public class MessagingConfiguration
    {
        public List<Action> BuildActions { get; private set; }
        internal IIocContainer ExternalContainer { get; private set; }

        public MessagingConfiguration()
        {
            BuildActions = new List<Action>();
            SetExternalContainer(IocContainerLocator.Locate());
        }

        void SetExternalContainer(IIocContainer container)
        {
            this.ExternalContainer = container;
            this.ExternalContainer.RegisterInstance<NullUnitOfWorkFactory, NullUnitOfWorkFactory>();
        }

        public MessagingConfiguration LoggingWith(ILoggingMechanism toLogWith)
        {
            Contract.Requires(toLogWith != null);

            Logger.LoggingMechanism = toLogWith;
            return this;
        }

        public MessagingConfiguration UsingIocContainer(IIocContainer container)
        {
            Contract.Requires(container != null);

            SetExternalContainer(container);
            return this;
        }

        public HandlerBasedOnConfiguration RegisterHandlersFromAssemblyOf<TAssemblyOf>()
        {
            return new HandlerBasedOnConfiguration(
                this,
                typeof(TAssemblyOf).GetTypesInAssembly().WhereNonAbstract().WhereNonGeneric().WhereConcrete());
        }

        public MessageServerConfiguration UsingInProcessTransport()
        {
            InProcessTransportComponents.Register(IocContainerLocator.Locate());
            return new MessageServerConfiguration(this, ServerPath.None);
        }

        public HttpTransportConfiguration UsingHttpTransport()
        {
            HttpTransportComponents.Register(IocContainerLocator.Locate());
            return new HttpTransportConfiguration(this);
        }

        public IIocContainer GetInternalIocContainer()
        {
            return IocContainerLocator.Locate();
        }
    }
}