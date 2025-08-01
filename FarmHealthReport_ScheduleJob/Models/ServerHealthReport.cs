using System;
using System.Collections.Generic;

namespace FarmHealthReport_ScheduleJob.Models;

public partial class ServerHealthReport
{
    public string Id { get; set; } = null!;

    public string ReportName { get; set; } = null!;

    public DateTime ScriptStartTime { get; set; }

    public DateTime ScriptEndTime { get; set; }

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();

    public virtual ICollection<ConnectionBroker> ConnectionBrokerNames { get; set; } = new List<ConnectionBroker>();
}
