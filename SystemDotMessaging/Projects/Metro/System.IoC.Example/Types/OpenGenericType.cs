﻿namespace System.IoC.Example.Types
{
    public class OpenGenericType<T>
    {
        public string Say()
        {
            return "I should not be registered. I am OpenGenericType<T>.";
        }
    }
}