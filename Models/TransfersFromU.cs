using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class TransfersFromU
{
    public int Id { get; set; }

    public int? Date { get; set; }

    public string? Folder { get; set; }

    public decimal? Amount { get; set; }

    public decimal? DollarExchange { get; set; }

    public decimal? DepositedQs { get; set; }

    public bool? IsActive { get; set; }
}
