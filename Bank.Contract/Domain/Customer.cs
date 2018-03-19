using System;

namespace Bank.Contract.Domain
{
    public class Customer:IEntity
    {
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
