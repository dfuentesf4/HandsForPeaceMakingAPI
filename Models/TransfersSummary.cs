using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class TransfersSummary
{
    public int Id { get; set; }

    public int? Year { get; set; }

    public decimal? TotalIncome { get; set; }

    public decimal? TotalExpenses { get; set; }

    public decimal? NetIncome { get; set; }

    public decimal? RetainedEarning { get; set; }

    public decimal? BankBox { get; set; }

    public bool? IsActive { get; set; }
}
