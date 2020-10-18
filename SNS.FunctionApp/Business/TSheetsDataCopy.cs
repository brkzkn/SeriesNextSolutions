using AutoMapper;
using EFCore.BulkExtensions;
using Intuit.TSheets.Api;
using Microsoft.Extensions.Logging;
using Polly;
using SNS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNS.FunctionApp.Business
{
    public class TSheetsDataCopy
    {
        private readonly IMapper _mapper;
        private readonly SNSTimeTrackerContext _context;
        private readonly ILogger _logger;
        private readonly DataService _client;

        public TSheetsDataCopy(SNSTimeTrackerContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;

            string authToken = Environment.GetEnvironmentVariable("AuthToken", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            _client = TSheetsDataServiceFactory.CreateDataService(authToken, null);
        }

        public void UserCopy()
        {
            var tsheetUsers = _client.GetUsers().Item1;

            var userEntities = _mapper.Map<IList<Tsusers>>(tsheetUsers);

            var policy = Policy.Handle<Exception>()
                               .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _logger.LogInformation($"[User]: Bulk insert is starting");

                _context.BulkInsertOrUpdate<Tsusers>(userEntities);

                _logger.LogInformation($"[User]: Bulk insert completed");
            });
        }

        public void TimeSheetCopy()
        {
            try
            {
                var timeSheets = _client.GetTimesheets(new Intuit.TSheets.Model.Filters.TimesheetFilter()
                {
                    StartDate = DateTimeOffset.Now.AddYears(-1)
                }).Item1;
                var timeEntries = _mapper.Map<IList<TstimeEntries>>(timeSheets);

                var policy = Policy.Handle<Exception>()
                                   .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                policy.Execute(() =>
                {
                    _logger.LogInformation($"[TimeSheets]: Bulk insert is starting");

                    _context.BulkInsertOrUpdate<TstimeEntries>(timeEntries);

                    _logger.LogInformation($"[TimeSheets]: Bulk insert completed");
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void JobcodeCopy()
        {
            var jobCodes = _client.GetJobcodes().Item1;
            
            var engagementEntities = _mapper.Map<IList<Tsengagements>>(jobCodes.Where(x => x.ParentId.HasValue && x.ParentId > 0));
            var clientEntities = _mapper.Map<IList<Tsclients>>(jobCodes.Where(x => !x.ParentId.HasValue || (x.ParentId.HasValue && x.ParentId == 0)));

            var policy = Policy.Handle<Exception>()
                               .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _logger.LogInformation($"[Engagements & Clients]: Bulk insert is starting");

                _context.BulkInsertOrUpdate<Tsengagements>(engagementEntities);
                _context.BulkInsertOrUpdate<Tsclients>(clientEntities);

                _logger.LogInformation($"[Engagements & Clients]: Bulk insert completed");
            });
        }
    }
}
