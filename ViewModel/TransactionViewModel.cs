using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RKDB.ViewModel
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        public int? ScheduleId { get; set; }

        public decimal? PaymentAmount { get; set; }

        public DateTime? TransactionDate { get; set; }
    }
}