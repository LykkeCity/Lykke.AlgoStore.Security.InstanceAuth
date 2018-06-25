using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class, 
        AllowMultiple = false, 
        Inherited = true)]
    public class RateLimitAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new RateLimitHandler((RateLimitSettings)serviceProvider
                .GetService(typeof(RateLimitSettings)));
        }
    }
}
