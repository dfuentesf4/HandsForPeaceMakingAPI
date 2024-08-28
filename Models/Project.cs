using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? State { get; set; }

    public decimal? Budget { get; set; }

    public string? Location { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
}
