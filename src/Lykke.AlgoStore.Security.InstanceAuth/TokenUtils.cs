using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal static class TokenUtils
    {
        /// <summary>
        /// Retrieves the authentication token from a request
        /// </summary>
        /// <param name="context">The context to retrieve the token from</param>
        /// <returns>
        /// The token if the request contains a valid authorization header, null otherwise
        /// </returns>
        public static string GetToken(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(header))
                return null;

            var values = header.Split(' ');

            if (values.Length != 2)
                return null;

            if (values[0] != InstanceAuthConstants.AUTH_SCHEME)
                return null;

            return values[1];
        }
    }
}
