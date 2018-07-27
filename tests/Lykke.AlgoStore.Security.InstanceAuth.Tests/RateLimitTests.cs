using Lykke.AlgoStore.Security.InstanceAuth.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Moq;
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
            var actionExecutingContext = MockUtils.Given_ActionExecutingContextMock(context.Object);
            var actionExecutedContext = MockUtils.Given_ActionExecutedContextMock(context.Object);


            var middleware = new RateLimitHandler(new RateLimitSettings
            {
                MaximumRequestsPerTimeframe = 1,
                TimeframeDurationInSeconds = 1,
                StatusCodesToIgnore = new ushort[0]
            });

            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);
            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();

            Assert.IsTrue(actionExecutingContext.Result is StatusCodeResult);
            Assert.AreEqual(429, ((StatusCodeResult)actionExecutingContext.Result).StatusCode);
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
            var actionExecutingContext = MockUtils.Given_ActionExecutingContextMock(context.Object);
            var actionExecutedContext = MockUtils.Given_ActionExecutedContextMock(context.Object);

            var middleware = new RateLimitHandler(new RateLimitSettings
            {
                MaximumRequestsPerTimeframe = 1,
                TimeframeDurationInSeconds = 1,
                StatusCodesToIgnore = new ushort[0]
            });

            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);                
            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();

            Assert.IsTrue(actionExecutingContext.Result is StatusCodeResult);
            Assert.AreEqual(429, ((StatusCodeResult)actionExecutingContext.Result).StatusCode);
        }

        [Test]
        public void RateLimitMiddleware_DoesNotLimit_WhenServerReturns5xx()
        {
            var ip = new IPAddress(new byte[] { 0, 0, 0, 0 });
            var headerDictionary = MockUtils.Given_Verifiable_HeaderDictionaryMock(
                new Dictionary<string, StringValues>()
                {
                    ["Authorization"] = "Bearer token"
                });

            var request = MockUtils.Given_Verifiable_HttpRequestMock(headerDictionary.Object);
            var response = MockUtils.Given_Verifiable_HttpResponseMock(statusCode: 500);
            var context = MockUtils.Given_Verifiable_HttpContextMock(request.Object, response.Object, ip);
            var actionExecutingContext = MockUtils.Given_ActionExecutingContextMock(context.Object);
            var actionExecutedContext = MockUtils.Given_ActionExecutedContextMock(context.Object);

            var middleware = new RateLimitHandler(new RateLimitSettings
            {
                MaximumRequestsPerTimeframe = 1,
                TimeframeDurationInSeconds = 1,
                StatusCodesToIgnore = new ushort[] { 500 }
            });

            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);
            middleware.OnActionExecuting(actionExecutingContext);
            middleware.OnActionExecuted(actionExecutedContext);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();

            Assert.IsNull(actionExecutingContext.Result);

        }
    }
}
