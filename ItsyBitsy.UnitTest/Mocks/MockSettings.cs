using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest.Mocks
{
    public class MockSettings : ISettings
    {
        public bool FollowExtenalLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DownloadExternalContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FollowRedirects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseCookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
