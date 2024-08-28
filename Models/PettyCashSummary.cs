using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class PettyCashSummary
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public decimal? DollarExchange { get; set; }

    public decimal? Amount { get; set; }

    public bool? IsActive { get; set; }
}
