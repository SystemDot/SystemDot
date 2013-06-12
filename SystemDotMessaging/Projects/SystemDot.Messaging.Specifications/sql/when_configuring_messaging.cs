using SystemDot.Messaging.Configuration;
using SystemDot.Sql;
using SystemDot.Storage.Changes;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.sql
{
    [Subject(SpecificationGroup.Description)]
    public class when_configuring_messaging_with_file_persistence : WithConfigurationSubject
    {
        Because of = () => Messaging.Configuration.Configure.Messaging()
            .UsingSqlPersistence("connection")
            .UsingInProcessTransport();

        It should_have_registered_the_sql_change_store = () => 
            Resolve<IChangeStore>().ShouldBeOfType<SqlChangeStore>();  
    }
}