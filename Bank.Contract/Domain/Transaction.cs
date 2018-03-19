using System;

namespace Bank.Contract.Domain
{
    public class Transaction : IEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }
}
