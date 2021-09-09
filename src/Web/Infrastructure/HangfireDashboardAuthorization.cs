using System;
using System.Security.Claims;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Web.Infrastructure
{
    public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return true;
            // return httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated;
        }
    }
}