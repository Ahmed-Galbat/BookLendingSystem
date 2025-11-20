using AutoMapper;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Application.MappingProfiles;
using BookLendingSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BookLendingSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(BookLendingProfile).Assembly);

            // Services
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();

            return services;
        }
    }
}
