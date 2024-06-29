using System;
using System.Collections.Generic;

namespace RKDB.Entities;

public partial class Clientinfo
{
    public int Id { get; set; }

    public int? Usertype { get; set; }

    public string? Firstname { get; set; }

    public string? Middlename { get; set; }

    public string? Lastname { get; set; }

    public string? Address { get; set; }

    public int? Zipcode { get; set; }

    public DateTime? Birthday { get; set; }

    public int? Age { get; set; }

    public string? Nameoffather { get; set; }

    public string? Nameofmother { get; set; }

    public string? CivilStatus { get; set; }

    public string? Religion { get; set; }

    public string? Occupation { get; set; }
}
