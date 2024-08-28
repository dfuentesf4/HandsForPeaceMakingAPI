﻿using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Volunteer
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public int? ProjectId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<VolunteersActivity> VolunteersActivities { get; set; } = new List<VolunteersActivity>();
}
