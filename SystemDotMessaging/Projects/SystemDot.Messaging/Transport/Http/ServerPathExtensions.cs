using SystemDot.Http;
using SystemDot.Messaging.Addressing;

namespace SystemDot.Messaging.Transport.Http
{
    public static class ServerPathExtensions
    {
        public static FixedPortAddress GetUrl(this ServerRoute route)
        {
            return new FixedPortAddress(route.Proxy.Address.Path, route.Proxy.Address.IsSecure, route.Proxy.Name);
        }
    }
}