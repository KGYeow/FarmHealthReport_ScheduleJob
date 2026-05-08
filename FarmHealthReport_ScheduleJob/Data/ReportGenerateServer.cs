using System;
using System.Collections.Generic;

namespace FarmHealthReport_ScheduleJob.Data;

public partial class ReportGenerateServer
{
    public int Id { get; set; }

    public string ServerName { get; set; } = null!;

    public string? Location { get; set; }

    public string? UtcOffset { get; set; } // e.g. "-05:00", "+08:00"

    // Navigation property (one-to-many)
    public virtual ICollection<ServerHealthReport> ServerHealthReports { get; set; } = new List<ServerHealthReport>();
}
