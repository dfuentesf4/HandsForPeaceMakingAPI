using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class ExpensesDetail
{
    public int Id { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public decimal? Amount { get; set; }

    public decimal? DollarExchange { get; set; }

    public decimal? Folder { get; set; }

    public bool? IsActive { get; set; }
}
