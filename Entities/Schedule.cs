using System;
using System.Collections.Generic;

namespace RKDB.Entities;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? LoanId { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public decimal? PaymentAmount { get; set; }

    public string? Status { get; set; }
}
