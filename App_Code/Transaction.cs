using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniBank.App_Code
{
    public class Transaction
    {
        public int Serial { get; set; } // 0 = Rejected, 1 = Valid
        public string Status { get; set; } // "Valid" or "Rejected"
        public string RejectionReason { get; set; }
        public int SenderAccountId { get; set; }
        public int RecipientAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ValueDate { get; set; }

    }
}