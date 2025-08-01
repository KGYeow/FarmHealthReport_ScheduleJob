using System;
using System.Collections.Generic;

namespace FarmHealthReport_ScheduleJob.Models;

public partial class Collection
{
    public int Id { get; set; }

    public string ReportId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string CpuUsageAvg { get; set; } = null!;

    public string MemoryUsageAvg { get; set; } = null!;

    public string CdriveFreeSpaceAvg { get; set; } = null!;

    public string DdriveFreeSpaceAvg { get; set; } = null!;

    public string SessionsTotalAvg { get; set; } = null!;

    public string SessionsActiveAvg { get; set; } = null!;

    public string SessionsDiscAvg { get; set; } = null!;

    public string SessionsNullAvg { get; set; } = null!;

    public string SessionsTotalSum { get; set; } = null!;

    public string SessionsActiveSum { get; set; } = null!;

    public string SessionsDiscSum { get; set; } = null!;

    public string SessionsNullSum { get; set; } = null!;

    public virtual ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();

    public virtual ServerHealthReport Report { get; set; } = null!;
}
