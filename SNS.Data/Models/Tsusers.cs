using System;
using System.Collections.Generic;

namespace SNS.Data.Models
{
    public partial class Tsusers
    {
        public int TsuserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Active { get; set; }
        public int? EmployeeNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public DateTimeOffset? LastActiveDate { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string Company { get; set; }
        public DateTimeOffset? SubmittedToDate { get; set; }
        public DateTimeOffset? ApprovedToDate { get; set; }
    }
}
