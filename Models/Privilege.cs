using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HandsForPeaceMakingAPI.Models;

public partial class Privilege
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public bool? ProjectManager { get; set; }

    public bool? DonorManager { get; set; }

    public bool? AccountingManager { get; set; }

    public bool? IsActive { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
