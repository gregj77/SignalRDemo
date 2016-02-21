using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace WebApplication.Infrastructure
{
    public class AuthenticationMiddleware : OwinMiddleware
    {
        public AuthenticationMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            bool isValid = false;
            string headerAsString = context.Request.Headers.Get("Authorization");
            if (!string.IsNullOrWhiteSpace(headerAsString))
            {
                var headerValue = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(headerAsString);
                if (string.Equals(headerValue.Scheme, "BASIC", StringComparison.OrdinalIgnoreCase))
                {
                    string userName =
                        Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter)).Split(':')[0];

                    if (UsersRepository.AuthenticateUser(userName))
                    {
                        isValid = true;
                        context.Request.User = new GenericPrincipal(new GenericIdentity(userName, "BASIC"), null);
                    }
                }
            }
            if (isValid)
                await Next.Invoke(context);
            else
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }
}
