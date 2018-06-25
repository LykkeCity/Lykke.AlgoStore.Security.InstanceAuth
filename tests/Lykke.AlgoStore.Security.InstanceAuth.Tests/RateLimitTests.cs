using Lykke.AlgoStore.Security.InstanceAuth.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;

namespace Lykke.AlgoStore.Security.InstanceAuth.Tests
{
    [TestFixture]
    public class RateLimitTests
    {
        [Test]
        public void RateLimitMiddleware_LimitsByIp_WhenTooManyRequests()
        {
            var ip = new IPAddress(new byte[] { 0, 0, 0, 0 });

            var headerDictionary = MockUtils.Given_Verifiable_HeaderDictionaryMock(
                new Dictionary<string, StringValues>()
            {
                ["Authorization"] = ""
            });
            var request = MockUtils.Given_Verifiable_HttpRequestMock(headerDictionary.Object);
            var response = MockUtils.Given_Verifiable_HttpResponseMock(new List<string> { "Retry-After" });
            var context = MockUtils.Given_Verifiable_HttpContextMock(request.Object, response.Object, ip);
            var actionContext = MockUtils.Given_ActionContextMock(context.Object);

            var middleware = new RateLimitHandler(new RateLimitSettings
            {
                MaximumRequestsPerMinute = 1
            });

            middleware.OnActionExecuting(actionContext);
            middleware.OnActionExecuting(actionContext);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();

            Assert.IsTrue(actionContext.Result is StatusCodeResult);
            Assert.AreEqual(429, ((StatusCodeResult)actionContext.Result).StatusCode);
        }

        [Test]
        public void RateLimitMiddleware_LimitsByToken_WhenTooManyRequests()
        {
            var ip = new IPAddress(new byte[] { 0, 0, 0, 0 });
            var ip2 = new IPAddress(new byte[] { 0, 0, 0, 1 });

            var headerDictionary = MockUtils.Given_Verifiable_HeaderDictionaryMock(
                new Dictionary<string, StringValues>()
            {
                ["Authorization"] = "Bearer token"
            });
            var request = MockUtils.Given_Verifiable_HttpRequestMock(headerDictionary.Object);
            var response = MockUtils.Given_Verifiable_HttpResponseMock(new List<string> { "Retry-After" });
            var context = MockUtils.Given_Verifiable_HttpContextMock(request.Object, response.Object, ip, ip2);
            var actionContext = MockUtils.Given_ActionContextMock(context.Object);

            var middleware = new RateLimitHandler(new RateLimitSettings
            {
                MaximumRequestsPerMinute = 1
            });

            middleware.OnActionExecuting(actionContext);
            middleware.OnActionExecuting(actionContext);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();

            Assert.IsTrue(actionContext.Result is StatusCodeResult);
            Assert.AreEqual(429, ((StatusCodeResult)actionContext.Result).StatusCode);
        }
    }
}
