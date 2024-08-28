using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class VolunteersActivity
{
    public int Id { get; set; }

    public int? ActivityId { get; set; }

    public int? VolunteerId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual Volunteer? Volunteer { get; set; }
}
