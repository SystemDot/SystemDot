using SystemDot.Ioc;
using SystemDot.Messaging.Configuration;
using SystemDot.Storage.Changes;

namespace SystemDot.Sql
{
    public static class SqlMessageServerConfigurationExtensions
    {
        public static MessageServerConfiguration UsingSqlPersistence(this MessageServerConfiguration configuration, string connectionString)
        {
            IocContainerLocator.Locate().RegisterInstance<IChangeStore, SqlChangeStore>();
            IocContainerLocator.Locate().Resolve<IChangeStore>().Initialise(connectionString);
            return configuration;
        }
    }
}