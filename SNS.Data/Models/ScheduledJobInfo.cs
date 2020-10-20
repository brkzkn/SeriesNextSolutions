using System;
using System.Collections.Generic;

namespace SNS.Data.Models
{
    public partial class ScheduledJobInfo
    {
        public string TableName { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
