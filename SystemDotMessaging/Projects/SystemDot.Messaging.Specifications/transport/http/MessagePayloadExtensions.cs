using System;
using SystemDot.Messaging.Addressing;
using SystemDot.Messaging.Ioc;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Packaging.Headers;
using SystemDot.Messaging.Publishing;
using SystemDot.Messaging.Sequencing;
using SystemDot.Messaging.Storage;
using SystemDot.Serialisation;

namespace SystemDot.Messaging.Specifications.transport.http
{
    public static class MessagePayloadExtensions
    {
        public static MessagePayload MakeSequencedReceivable(
            this MessagePayload payload,
            object message,
            EndpointAddress from,
            EndpointAddress to,
            PersistenceUseType useType)
        {
            payload.SetMessageBody(message);
            payload.SetFromAddress(from);
            payload.SetToAddress(to);
            payload.SetChannelType(useType);
            payload.Sequenced();
            
            return payload;
        }

        public static MessagePayload SetMessageBody(this MessagePayload payload, object toSet)
        {
            payload.SetBody(Resolve<ISerialiser>().Serialise(toSet));
            return payload;
        }

        public static MessagePayload SetFromChannel(this MessagePayload payload, string toSet)
        {
            payload.SetFromAddress(new EndpointAddress(toSet, ServerRoute.None));
            return payload;
        }

        public static MessagePayload SetFromProxy(this MessagePayload payload, string toSet)
        {
            payload.GetFromAddress().Route.Proxy = MessageServer.Named(toSet, ServerAddress.Local);
            return payload;
        }

        public static MessagePayload SetFromProxyAddress(this MessagePayload payload, string toSet)
        {
            payload.GetFromAddress().Route.Proxy.Address = new ServerAddress(toSet, false);
            return payload;
        }

        public static MessagePayload SetToChannel(this MessagePayload payload, string toSet)
        {
            payload.SetToAddress(new EndpointAddress(toSet, ServerRoute.None));
            return payload;
        }

        public static MessagePayload SetToServer(this MessagePayload payload, string toSet)
        {
            payload.GetToAddress().Route.Server = MessageServer.Named(toSet, ServerAddress.Local);
            payload.SetToProxy(toSet);
            return payload;
        }

        public static MessagePayload SetToProxy(this MessagePayload payload, string toSet)
        {
            payload.GetToAddress().Route.Proxy = MessageServer.Named(toSet, ServerAddress.Local);
            return payload;
        }

        public static MessagePayload SetToServerAddress(this MessagePayload payload, string toSet)
        {
            payload.GetToAddress().Route.Server.Address = new ServerAddress(toSet, false);
            return payload;
        }

        public static MessagePayload SetChannelType(this MessagePayload payload, PersistenceUseType toSet)
        {
            payload.SetPersistenceId(payload.GetFromAddress(), toSet);
            payload.SetSourcePersistenceId(payload.GetPersistenceId());
            payload.SetSequenceOriginSetOn(DateTime.Today);
            return payload;
        }

        public static MessagePayload Sequenced(this MessagePayload payload)
        {
            payload.SetFirstSequence(1);
            payload.SetSequence(1);
            return payload;
        }

        public static MessagePayload SetSubscriptionRequest(this MessagePayload payload)
        {
            payload.SetSubscriptionRequest(new SubscriptionSchema { SubscriberAddress = payload.GetFromAddress() });
            payload.SetSequence(1);
            return payload;
        }
        
        static T Resolve<T>() where T : class
        {
            return IocContainerLocator.Locate().Resolve<T>();
        }

        
    }
}