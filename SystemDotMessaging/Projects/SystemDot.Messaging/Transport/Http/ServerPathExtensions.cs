using SystemDot.Http;
using SystemDot.Messaging.Addressing;

namespace SystemDot.Messaging.Transport.Http
{
    public static class ServerPathExtensions
    {
        public static FixedPortAddress GetUrl(this ServerPath path)
        {
            return new FixedPortAddress(path.Proxy.Address, path.Proxy.IsSecure, path.Proxy.Name);
        }
    }
}