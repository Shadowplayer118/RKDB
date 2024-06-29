using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RKDB.ViewModel
{
    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public int? LoanId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? Status { get; set; }
        public decimal AdjustedPaymentAmount { get; set; } 
        public decimal TotalPaidAmount { get; set; }
    }
}