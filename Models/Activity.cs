using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? ProjectId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<VolunteersActivity> VolunteersActivities { get; set; } = new List<VolunteersActivity>();
}
