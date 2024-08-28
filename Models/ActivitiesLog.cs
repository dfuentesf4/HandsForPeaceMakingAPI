using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class ActivitiesLog
{
    public int LogId { get; set; }

    public int? ActivityId { get; set; }

    public string? OperationType { get; set; }

    public DateTime? OperationTimestamp { get; set; }

    public string? ChangedData { get; set; }

    public string? PerformedBy { get; set; }
}
