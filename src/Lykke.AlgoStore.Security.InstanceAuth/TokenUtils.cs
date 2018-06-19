using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal static class TokenUtils
    {
        public static string GetToken(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(header))
                return null;

            var values = header.Split(' ');

            if (values.Length != 2)
                return null;

            if (values[0] != "Bearer")
                return null;

            return values[1];
        }
    }
}
