using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Lykke.AlgoStore.Security.InstanceAuth.Tests.Utils
{
    internal static class MockUtils
    {
        public static ActionExecutingContext Given_ActionContextMock(
            HttpContext context)
        {
            return new ActionExecutingContext(
                new Microsoft.AspNetCore.Mvc.ActionContext(
                    context, 
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                null);
        }

        public static Mock<HttpContext> Given_Verifiable_HttpContextMock(
            HttpRequest request,
            HttpResponse response,
            params IPAddress[] connections)
        {
            var mock = new Mock<HttpContext>(MockBehavior.Strict);

            if (request != null)
            {
                mock.SetupGet(c => c.Request)
                    .Returns(request)
                    .Verifiable();
            }

            if (response != null)
            {
                mock.SetupGet(c => c.Response)
                    .Returns(response)
                    .Verifiable();
            }

            if (connections.Length > 0)
            {
                var index = 0;
                var connectionMock = new Mock<ConnectionInfo>(MockBehavior.Strict);

                connectionMock.SetupGet(c => c.RemoteIpAddress)
                              .Returns(() => index >= connections.Length ?
                                             connections[connections.Length - 1] :
                                             connections[index++]);

                mock.SetupGet(c => c.Connection)
                    .Returns(connectionMock.Object)
                    .Verifiable();
            }

            return mock;
        }

        public static Mock<HttpRequest> Given_Verifiable_HttpRequestMock(
            IHeaderDictionary headers = null)
        {
            var mock = new Mock<HttpRequest>(MockBehavior.Strict);

            if (headers != null)
            {
                mock.SetupGet(r => r.Headers)
                    .Returns(headers)
                    .Verifiable();
            }

            return mock;
        }

        public static Mock<HttpResponse> Given_Verifiable_HttpResponseMock(
            List<string> expectedHeaders = null)
        {
            var mock = new Mock<HttpResponse>(MockBehavior.Strict);

            if (expectedHeaders != null)
            {
                var dictMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
                dictMock.Setup(d => d.Add(It.IsAny<string>(), It.IsAny<StringValues>()))
                        .Callback((string key, StringValues value) =>
                        {
                            if (!expectedHeaders.Contains(key))
                                Assert.Fail($"Unexpected header {key}");
                        });

                mock.Setup(r => r.Headers)
                    .Returns(dictMock.Object)
                    .Verifiable();
            }

            return mock;
        }

        public static Mock<IHeaderDictionary> Given_Verifiable_HeaderDictionaryMock(
            Dictionary<string, StringValues> headers)
        {
            var mock = new Mock<IHeaderDictionary>(MockBehavior.Strict);

            mock.Setup(d => d[It.IsAny<string>()])
                .Returns((string key) => headers[key])
                .Verifiable();

            return mock;
        }
    }
}
