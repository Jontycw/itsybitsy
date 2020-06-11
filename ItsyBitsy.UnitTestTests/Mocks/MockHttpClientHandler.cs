using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ItsyBitsy.UnitTest.Mocks
{
    public class MockHttpClientHandler : HttpClientHandler
    {
        public MockHttpClientHandler()
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsoluteUri == Const.LINK1)
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(MockHtml.Has1Link, Encoding.UTF8, @"text/html"),
                    RequestMessage = request
                });

            if (request.RequestUri.AbsoluteUri == Const.LINK2)
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(MockHtml.HasLink2, Encoding.UTF8, @"text/html"),
                    RequestMessage = request
                });

            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("OK", Encoding.UTF8, @"text/html"),
                RequestMessage = request
            });
        }
    }
}
