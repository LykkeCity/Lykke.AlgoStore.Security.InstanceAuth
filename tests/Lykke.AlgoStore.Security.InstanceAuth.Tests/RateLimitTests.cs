using Lykke.AlgoStore.Security.InstanceAuth.Tests.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Security.InstanceAuth.Tests
{
    [TestFixture]
    public class RateLimitTests
    {
        [Test]
        public async Task RateLimitMiddleware_LimitsByIp_WhenTooManyRequests()
        {
            var ip = new IPAddress(new byte[] { 0, 0, 0, 0 });

            var headerDictionary = MockUtils.Given_Verifiable_HeaderDictionaryMock(
                new Dictionary<string, StringValues>()
            {
                ["Authorization"] = ""
            });
            var request = MockUtils.Given_Verifiable_HttpRequestMock(headerDictionary.Object);
            var response = MockUtils.Given_Verifiable_HttpResponseMock(429, 
                new List<string> { "Retry-After" });
            var context = MockUtils.Given_Verifiable_HttpContextMock(request.Object, response.Object, ip);

            RateLimitMiddleware.MaxRequestsPerInterval = 1;

            await RateLimitMiddleware.RateLimit(context.Object, () => Task.CompletedTask);
            await RateLimitMiddleware.RateLimit(context.Object, () => Task.CompletedTask);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();
        }

        [Test]
        public async Task RateLimitMiddleware_LimitsByToken_WhenTooManyRequests()
        {
            var ip = new IPAddress(new byte[] { 0, 0, 0, 0 });
            var ip2 = new IPAddress(new byte[] { 0, 0, 0, 1 });

            var headerDictionary = MockUtils.Given_Verifiable_HeaderDictionaryMock(
                new Dictionary<string, StringValues>()
            {
                ["Authorization"] = "Bearer token"
            });
            var request = MockUtils.Given_Verifiable_HttpRequestMock(headerDictionary.Object);
            var response = MockUtils.Given_Verifiable_HttpResponseMock(429, 
                new List<string> { "Retry-After" });
            var context = MockUtils.Given_Verifiable_HttpContextMock(request.Object, response.Object, ip, ip2);

            RateLimitMiddleware.MaxRequestsPerInterval = 1;

            await RateLimitMiddleware.RateLimit(context.Object, () => Task.CompletedTask);
            await RateLimitMiddleware.RateLimit(context.Object, () => Task.CompletedTask);

            context.Verify();
            request.Verify();
            response.Verify();
            headerDictionary.Verify();
        }

        [Test]
        public void UseRateLimiting_RegistersMiddlewareSuccessfully()
        {
            var applicationBuilderMock = new Mock<IApplicationBuilder>(MockBehavior.Strict);
            applicationBuilderMock
                .Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(applicationBuilderMock.Object)
                .Verifiable();

            applicationBuilderMock.Object.UseRateLimiting(new RateLimitSettings
            {
                MaximumRequestsPerMinute = 1
            });

            applicationBuilderMock.Verify();
        }
    }
}
