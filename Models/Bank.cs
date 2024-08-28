using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Bank
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BankBook> BankBooks { get; set; } = new List<BankBook>();

    public virtual ICollection<BankSummary> BankSummaries { get; set; } = new List<BankSummary>();

    public virtual ICollection<FolderBank> FolderBanks { get; set; } = new List<FolderBank>();
}
