using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Beneficiary
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ProjectId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Project? Project { get; set; }
}
