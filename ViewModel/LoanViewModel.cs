using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RKDB.ViewModel;

public partial class LoanViewModel
{
    // public List<ScheduleViewModel> Schedules { get; set; }
    public int Id { get; set; }
    public int? ClientId { get; set; }
    [Required]
    public int? Amount { get; set; }
    [Required]
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
