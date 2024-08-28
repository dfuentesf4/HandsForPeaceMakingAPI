﻿using System;
using System.Collections.Generic;

namespace HandsForPeaceMakingAPI.Models;

public partial class RevenuesDetailsLog
{
    public int LogId { get; set; }

    public int? RevenuesDetailsId { get; set; }

    public string? OperationType { get; set; }

    public DateTime? OperationTimestamp { get; set; }

    public string? ChangedData { get; set; }

    public string? PerformedBy { get; set; }
}
