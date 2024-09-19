using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class BankBook
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public decimal? DollarExchange { get; set; }

    public decimal? Amount { get; set; }

    public string? PayrollNumber { get; set; }

    public string? Beneficiarie { get; set; }

    public bool? Expenses { get; set; }

    public string? Justification { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Bank? Bank { get; set; }
}
