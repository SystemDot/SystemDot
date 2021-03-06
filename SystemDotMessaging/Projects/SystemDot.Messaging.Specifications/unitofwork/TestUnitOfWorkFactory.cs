using SystemDot.Messaging.UnitOfWork;

namespace SystemDot.Messaging.Specifications.unitofwork
{
    public class TestUnitOfWorkFactory : IUnitOfWorkFactory
    {
        readonly TestUnitOfWork unitOfWork;

        public TestUnitOfWorkFactory(TestUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IUnitOfWork Create()
        {
            return this.unitOfWork;
        }
    }
}