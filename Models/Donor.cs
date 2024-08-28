using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class Donor
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string? Observations { get; set; }

    public bool? IsActive { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
