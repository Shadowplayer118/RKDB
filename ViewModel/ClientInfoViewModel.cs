using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RKDB.ViewModel
{
    public class ClientInfoViewModel
    {       
        public List<LoanViewModel> Loans { get; set; }
        public int Id { get; set; }

        [Required]
        public int? Usertype { get; set; }
        
        public string UserTypeName { get; set; } = "";
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Middlename { get; set; }
        [Required]
        public string? Lastname { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public int? Zipcode { get; set; }
        [Required]
        public DateTime? Birthday { get; set; }
        [Required]
        public int? Age { get; set; }
        [Required]
        public string? Nameoffather { get; set; }
        [Required]
        public string? Nameofmother { get; set; }
        [Required]
        public string? CivilStatus { get; set; }
        [Required]
        public string? Religion { get; set; }
        [Required]
        public string? Occupation { get; set; }
    }
}