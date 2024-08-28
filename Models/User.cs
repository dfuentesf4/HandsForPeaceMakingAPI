using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? JobPosition { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();
}
