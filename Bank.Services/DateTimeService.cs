using Bank.Contract.Services;
using System;

namespace Bank.Services
{
    /// <summary>
    /// Box the DateTime static to allow unit test mocking of the boxing service to return the same datetime for test repeatablitity 
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
