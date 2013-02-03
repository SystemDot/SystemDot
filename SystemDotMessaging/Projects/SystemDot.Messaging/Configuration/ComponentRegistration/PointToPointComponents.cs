using SystemDot.Ioc;
using SystemDot.Messaging.Configuration.RequestReply;
using SystemDot.Messaging.PointToPoint;
using SystemDot.Messaging.PointToPoint.Builders;

namespace SystemDot.Messaging.Configuration.ComponentRegistration
{
    public static class PointToPointComponents
    {
        public static void Register(IIocContainer iocContainer)
        {
            iocContainer.RegisterInstance<PointToPointSendChannelBuilder, PointToPointSendChannelBuilder>();
            iocContainer.RegisterInstance<PointToPointReceiveChannelBuilder, PointToPointReceiveChannelBuilder>();
        }
    }
}