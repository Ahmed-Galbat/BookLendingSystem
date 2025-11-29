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
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
