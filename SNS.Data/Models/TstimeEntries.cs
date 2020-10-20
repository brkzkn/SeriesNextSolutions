using System;
using System.Collections.Generic;

namespace SNS.Data.Models
{
    public partial class TstimeEntries
    {
        public int TstimeEntryId { get; set; }
        public int TsuserId { get; set; }
        public int TsengagementId { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public int? Seconds { get; set; }
        public int? TimeZone { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public bool? OnTheClock { get; set; }
        public bool? Billable { get; set; }
        public string QboserviceItem { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public int? CreatedByUser { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset? EntryDate { get; set; }
    }
}
