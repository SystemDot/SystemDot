using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SystemDot
{
    public static class IocContainer
    {
        static readonly Dictionary<Type, object> components = new Dictionary<Type, object>();
        
        public static void Register<T>(T toSet)
        {
            Contract.Requires(!toSet.Equals(default(T)));
            components[typeof (T)] = toSet;
        }

        public static void Register<T>(Func<T> creator)
        {
            Contract.Requires(creator != null);
            components[typeof(T)] = creator;
        }

        public static void Register<TType, TConstructorArg>(Func<TConstructorArg, TType> creator)
        {
            Contract.Requires(!creator.Equals(default(TType)));
            components[typeof(TType)] = creator;
        }

        public static void Register<TType, TConstructorArg1, TConstructorArg2>(
            Func<TConstructorArg1, TConstructorArg2, TType> creator)
        {
            Contract.Requires(!creator.Equals(default(TType)));
            components[typeof(TType)] = creator;
        }

        public static T Resolve<T>()
        {
            object component = components[typeof (T)];

            if (component.GetType() == typeof(Func<T>))
                return component
                    .As<Func<T>>()
                    .Invoke();

            return component.As<T>();
        }

        public static TType Resolve<TType, TConstructorArg>(TConstructorArg arg)
        {
            Contract.Requires(!arg.Equals(default(TConstructorArg)));

            return components[typeof (TType)]
                .As<Func<TConstructorArg, TType>>()
                .Invoke(arg);
        }

        public static TType Resolve<TType, TConstructorArg1, TConstructorArg2>(
            TConstructorArg1 arg1, 
            TConstructorArg2 arg2)
        {
            Contract.Requires(!arg1.Equals(default(TConstructorArg1)));
            Contract.Requires(!arg2.Equals(default(TConstructorArg2)));

            return components[typeof(TType)]
                .As<Func<TConstructorArg1, TConstructorArg2, TType>>()
                .Invoke(arg1, arg2);
        }
    }
}