using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Infrastructure.Identity;
using BookLendingSystem.Infrastructure.Persistence;
using BookLendingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookLendingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Services
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();

            // HttpContextAccessor 
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
