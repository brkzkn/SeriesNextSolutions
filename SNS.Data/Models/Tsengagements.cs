using System;
using System.Collections.Generic;

namespace SNS.Data.Models
{
    public partial class Tsengagements
    {
        public int TsengagementId { get; set; }
        public int? TsclientId { get; set; }
        public string EngagementName { get; set; }
        public string Type { get; set; }
        public bool? Billable { get; set; }
        public double? BillRate { get; set; }
        public bool? Active { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
