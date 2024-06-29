using System;
using System.Collections.Generic;

namespace RKDB.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? ScheduleId { get; set; }

    public decimal? PaymentAmount { get; set; }

    public DateTime? TransactionDate { get; set; }
}
