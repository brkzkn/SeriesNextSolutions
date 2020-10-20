using AutoMapper;
using Intuit.TSheets.Model;
using SNS.Data.Models;
using System;

namespace SNS.FunctionApp.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<TimesheetsDeleted, TstimeEntriesDeleted>()
                .ForMember(dest => dest.TstimeEntryId, opt => opt.MapFrom(s => s.Id));

            CreateMap<Jobcode, Tsclients>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(s => s.Active))
                .ForMember(dest => dest.Billable, opt => opt.MapFrom(s => s.Billable))
                .ForMember(dest => dest.BillRate, opt => opt.MapFrom(s => s.BillableRate))
                .ForMember(dest => dest.ClientCode, opt => opt.MapFrom(s => s.ShortCode))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(s => s.Created))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(s => s.LastModified))
                .ForMember(dest => dest.TsclientId, opt => opt.MapFrom(s => s.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(s => s.JobcodeType));

            CreateMap<Jobcode, Tsengagements>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(s => s.Active))
                .ForMember(dest => dest.Billable, opt => opt.MapFrom(s => s.Billable))
                .ForMember(dest => dest.BillRate, opt => opt.MapFrom(s => s.BillableRate))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(s => s.Created))
                .ForMember(dest => dest.EngagementName, opt => opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(s => s.LastModified))
                .ForMember(dest => dest.TsclientId, opt => opt.MapFrom(s => s.ParentId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(s => s.JobcodeType))
                .ForMember(dest => dest.TsengagementId, opt => opt.MapFrom(s => s.Id));

            CreateMap<Timesheet, TstimeEntries>()
                .ForMember(dest => dest.Billable, opt => opt.MapFrom<BillableResolver>())
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(s => s.CreatedByUserId))
                .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(s => s.End))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(s => s.LastModified))
                //.ForMember(dest => dest.Location, opt => opt.MapFrom(s => s.))
                .ForMember(dest => dest.Seconds, opt => opt.MapFrom(s => s.Duration))
                .ForMember(dest => dest.OnTheClock, opt => opt.MapFrom(s => s.OnTheClock))
                .ForMember(dest => dest.QboserviceItem, opt => opt.MapFrom<QboserviceItemResolver>())
                .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(s => s.Start))
                //.ForMember(dest => dest.TimeZone, opt => opt.MapFrom(s => s.))
                .ForMember(dest => dest.TsengagementId, opt => opt.MapFrom(s => s.JobcodeId))
                .ForMember(dest => dest.TstimeEntryId, opt => opt.MapFrom(s => s.Id))
                .ForMember(dest => dest.TsuserId, opt => opt.MapFrom(s => s.UserId))
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(s => s.Date))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(s => s.Notes))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(s => s.Type));

            CreateMap<User, Tsusers>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(s => s.Active))
                .ForMember(dest => dest.ApprovedToDate, opt => opt.MapFrom(s => s.ApprovedTo))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(s => s.Created))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(s => s.Email))
                .ForMember(dest => dest.EmployeeNumber, opt => opt.MapFrom(s => s.EmployeeNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(dest => dest.LastActiveDate, opt => opt.MapFrom(s => s.LastActive))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(s => s.LastModified))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(dest => dest.SubmittedToDate, opt => opt.MapFrom(s => s.SubmittedTo))
                .ForMember(dest => dest.TsuserId, opt => opt.MapFrom(s => s.Id));
        }

        public class BillableResolver : IValueResolver<Timesheet, TstimeEntries, bool?>
        {
            public bool? Resolve(Timesheet source, TstimeEntries destination, bool? destMember, ResolutionContext context)
            {
                if (!source.CustomFields.TryGetValue("26827", out string boolString) || string.IsNullOrEmpty(boolString))
                    return null;

                return boolString.ToLower() == "yes" ? true : false ;
            }
        }


        public class QboserviceItemResolver : IValueResolver<Timesheet, TstimeEntries, string>
        {
            public string Resolve(Timesheet source, TstimeEntries destination, string destMember, ResolutionContext context)
            {
                if (!source.CustomFields.TryGetValue("26825", out string qboServiceItemValue) || string.IsNullOrEmpty(qboServiceItemValue))
                    return null;

                return qboServiceItemValue;
            }
        }
    }
}
