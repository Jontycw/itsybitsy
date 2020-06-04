using ItsyBitsy.Domain;
using ItsyBitsy.UnitTes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ItsyBitsy.UnitTest
{
    public static class RegisterUTestFactory
    {
        public static void Register()
        {
            Factory.Register<IRepository, MockRepository>();
        }

        public static void Clear()
        {
            Factory.Clear();
        }
    }
}
