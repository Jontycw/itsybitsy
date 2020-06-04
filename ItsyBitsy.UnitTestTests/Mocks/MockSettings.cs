using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest.Mocks
{
    public class MockSettings : ISettings
    {
        public bool FollowExtenalLinks { get; set; }
        public bool DownloadExternalContent { get; set; }
        public bool FollowRedirects { get; set; }
        public bool UseCookies { get; set; }
    }
}
