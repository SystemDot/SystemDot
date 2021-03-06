using System;
using System.Diagnostics.Contracts;

namespace SystemDot
{
    public class ServerAddress
    {
        public static ServerAddress Local
        {
            get { return new ServerAddress(Environment.MachineName, false); }
        }

        public string Path { get; set; }

        public bool IsSecure { get; set; }

        public ServerAddress()
        {
        }

        public ServerAddress(string path, bool isSecure)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            Path = path;
            IsSecure = isSecure;
        }

        public override string ToString()
        {
            return String.Concat(Path, IsSecure ? "(Secure)" : string.Empty);
        }
    }
}