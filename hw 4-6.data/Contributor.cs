using System;
using System.Collections.Generic;
using System.Text;

namespace hw_4_6.data
{
    public class Contributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal DepositAmount { get; set; }
        public DateTime DepositDate { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal Balance { get; set; }
        public bool WantsToContribute { get; set; }
        public bool AlreadyContributed { get; set; }
        public List<Transaction> Transactions = new List<Transaction>();
    }
}
