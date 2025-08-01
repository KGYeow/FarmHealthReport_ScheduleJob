using System;
using System.Collections.Generic;

namespace FarmHealthReport_ScheduleJob.Models;

public partial class CollectionRecord
{
    public int Id { get; set; }

    public int CollectionId { get; set; }

    public string ServerName { get; set; } = null!;

    public string Enabled { get; set; } = null!;

    public string CpuUsage { get; set; } = null!;

    public string MemoryUsage { get; set; } = null!;

    public string CdriveFreeSpace { get; set; } = null!;

    public string DdriveFreeSpace { get; set; } = null!;

    public string Uptime { get; set; } = null!;

    public string PendingReboot { get; set; } = null!;

    public string SessionsTotal { get; set; } = null!;

    public string SessionsActive { get; set; } = null!;

    public string SessionsDisc { get; set; } = null!;

    public string SessionsNull { get; set; } = null!;

    public virtual Collection Collection { get; set; } = null!;
}
