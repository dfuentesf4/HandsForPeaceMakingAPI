using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Summary
{
    public int Id { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public decimal? DollarExchange { get; set; }

    public decimal? Expenses { get; set; }

    public decimal? Revenues { get; set; }

    public decimal? Outflows { get; set; }

    public bool? IsActive { get; set; }
}
