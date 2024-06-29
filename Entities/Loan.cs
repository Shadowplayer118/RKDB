using System;
using System.Collections.Generic;

namespace RKDB.Entities;

public partial class Loan
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public int? Amount { get; set; }

    public int? Term { get; set; }

    public DateTime? Duedate { get; set; }

    public string? Payment { get; set; }

    public decimal? PaymentAmount { get; set; }

    public decimal? Total { get; set; }

    public int? Interest { get; set; }

    public decimal? Deduction { get; set; }

    public string? Status { get; set; }

    public DateTime? DateCreated { get; set; }

    public int? Penalty { get; set; }

    public decimal? PayableAmount { get; set; }
}
