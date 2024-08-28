using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? JobPosition { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();
}
