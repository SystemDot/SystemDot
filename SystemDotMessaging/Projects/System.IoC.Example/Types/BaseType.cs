namespace System.IoC.Example.Types
{
    public class BaseType : IInterfaceForBaseType
    {
        public virtual string Say()
        {
            return "I am a registered. I am BaseType.";
        }
    }
}