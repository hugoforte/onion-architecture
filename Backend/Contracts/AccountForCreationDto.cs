using System;

namespace Payments.Contracts
{
    public class AccountForCreationDto
    {
        public DateTime DateCreated { get; set; }

        public string AccountType { get; set; }
    }
}