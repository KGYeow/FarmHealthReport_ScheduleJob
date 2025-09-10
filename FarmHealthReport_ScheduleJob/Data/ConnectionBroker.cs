using System;
using System.Collections.Generic;

namespace FarmHealthReport_ScheduleJob.Data;

public partial class ConnectionBroker
{
    public string Name { get; set; } = null!;

    public virtual ICollection<ServerHealthReport> Reports { get; set; } = new List<ServerHealthReport>();
}
