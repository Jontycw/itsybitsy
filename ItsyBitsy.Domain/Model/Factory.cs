using System;
using System.Collections.Generic;

namespace ItsyBitsy.Domain
{
    public static class Factory
    {
        private static readonly Dictionary<Type, Type> _instances = new Dictionary<Type, Type>();

        public static void Register<I, C>()
            where C : class, I, new()
        {
            _instances.Add(typeof(I), typeof(C));
        }

        public static I GetInstance<I>()
        {
            var type = _instances[typeof(I)];
            return (I)Activator.CreateInstance(type);
        }

        public static void Clear()
        {
            _instances.Clear();
        }
    }
}
