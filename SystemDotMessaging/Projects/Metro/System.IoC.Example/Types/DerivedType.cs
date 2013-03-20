﻿namespace System.IoC.Example.Types
{
    public class DerivedType : BaseType, IInterfaceForDerivedType
    {
        public override string Say()
        {
            return "I am a registered. I am DerivedType.";
        }
    }
}