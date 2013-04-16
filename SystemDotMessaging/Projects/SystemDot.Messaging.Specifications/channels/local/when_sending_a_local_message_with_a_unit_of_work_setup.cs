using SystemDot.Messaging.Transport.InProcess.Configuration;
using Machine.Specifications;

namespace SystemDot.Messaging.Specifications.channels.local
{
    [Subject(SpecificationGroup.Description)]
    public class when_sending_a_local_message_with_a_unit_of_work_setup : WithMessageConfigurationSubject
    {
        static TestUnitOfWork unitOfWork;
        static IBus bus;
        
        Establish context = () =>
        {
            unitOfWork = new TestUnitOfWork();
            ConfigureAndRegister<TestUnitOfWorkFactory>(new TestUnitOfWorkFactory(unitOfWork));

            bus = Configuration.Configure.Messaging()
                .UsingInProcessTransport()
                .OpenLocalChannel()
                .WithUnitOfWork<TestUnitOfWorkFactory>()
                .Initialise();
        };

        Because of = () => bus.SendLocal(new object());

        It should_begin_the_unit_of_work = () => unitOfWork.HasBegun().ShouldBeTrue();
    }
}