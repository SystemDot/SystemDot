using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace SystemDot.Messaging.Messages.Processing.Handling
{
    public class MessageHandlerRouter : IMessageInputter<object>
    {
        readonly List<object> handlers;
        readonly ITypeExtender typeExtender;

        public MessageHandlerRouter(ITypeExtender typeExtender)
        {
            this.typeExtender = typeExtender;
            this.handlers = new List<object>();
        }

        public void InputMessage(object toInput)
        {
            RouteMessageToHandlers(toInput);
        }

        void RouteMessageToHandlers(object message)
        {
            this.handlers.ForEach(c => Invoke(c, message));
        }

        void Invoke(object handler, object message)
        {
            MethodInfo method = GetHandlerMethodInfo(handler, message);
            if (method == null) return;

            method.Invoke(handler, new[] { message });
        }

        MethodInfo GetHandlerMethodInfo(object consumer, object message)
        {
            return this.typeExtender.GetMethod(consumer.GetType(), "Handle", new [] { message.GetType() });
        }

        public void RegisterHandler(object toRegister)
        {
            Contract.Requires(toRegister != null);
            this.handlers.Add(toRegister);
        }
    }
}