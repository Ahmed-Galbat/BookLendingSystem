using BookLendingSystem.Application.Interfaces;
using System;

namespace BookLendingSystem.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
