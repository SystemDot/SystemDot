using System;
using System.Threading;

namespace SystemDot.Messaging.Direct
{
    public class DirectSendContext : Disposable
    {
        readonly static ThreadLocal<ContextData> current = new ThreadLocal<ContextData>();

        public DirectSendContext(Action<Exception> onServerError)
            : this(onServerError, null)
        {
        }

        public DirectSendContext(Action<Exception> onServerError, object handleReplyWith)
        {
            current.Value = new ContextData(onServerError, handleReplyWith);
        }

        public static bool IsActive() { return current.Value != null; }

        public static Action<Exception> GetServerError() { return current.Value.OnServerError; }

        public static object GetHandleReplyWith() { return current.Value.HandleReplyWith; }

        protected override void DisposeOfManagedResources()
        {
            current.Value = null;
            base.DisposeOfManagedResources();
        }

        class ContextData
        {
            public Action<Exception> OnServerError { get; private set; }

            public object HandleReplyWith { get; private set; }

            public ContextData(Action<Exception> onServerError, object handleReplyWith)
            {
                OnServerError = onServerError;
                HandleReplyWith = handleReplyWith;
            }
        }
    }
}