using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace BookLendingSystem.Api
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IWebHostEnvironment _env;

        public HangfireAuthorizationFilter(IWebHostEnvironment env)
        {
            _env = env;
        }
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (_env.IsDevelopment())
                return true;

            return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin");
        }
    }
}
