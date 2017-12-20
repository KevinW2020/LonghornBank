using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LonghornBank.Models;

namespace LonghornBank.Models
{
    public class TransactionViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<Transfer> Transfers { get; set; }
    }
}