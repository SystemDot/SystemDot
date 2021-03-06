﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace SystemDot
{
    public static class AssemblyExtensions
    {
        public static string GetLocation(this Assembly assembly)
        {
            return "C:\\";
        }

        public static IEnumerable<Type> GetTypesThatImplement<TType>(this Assembly assembly)
        {
            return assembly.ExportedTypes.WhereNormalConcrete().WhereImplements<TType>();
        }
    }
}