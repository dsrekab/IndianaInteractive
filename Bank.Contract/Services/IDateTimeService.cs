using System;

namespace Bank.Contract.Services
{
    public interface IDateTimeService
    {
        DateTime Now();
        DateTime UtcNow();
    }
}
