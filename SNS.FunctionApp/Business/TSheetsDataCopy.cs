using AutoMapper;
using EFCore.BulkExtensions;
using Intuit.TSheets.Api;
using Intuit.TSheets.Model;
using Microsoft.Extensions.Logging;
using Polly;
using SNS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void TimeSheetCopy()
        {
            try
            {
                DateTime functionStartedTime = DateTime.UtcNow;
                _logger.LogInformation($"[TimeSheets]: Data fetching from TSheet API");
                var getLatestTime = _context.ScheduledJobInfo.SingleOrDefault(x => x.TableName == nameof(TstimeEntries));
                DateTimeOffset lastUpdateTime = DateTimeOffset.Now.AddYears(-1);
                if (getLatestTime != null)
                    lastUpdateTime = DateTime.SpecifyKind(getLatestTime.LastUpdateTime, DateTimeKind.Utc);

                var deletedTimeSheets = _client.GetTimesheetsDeleted(new Intuit.TSheets.Model.Filters.TimesheetsDeletedFilter()
                {
                    ModifiedSince = lastUpdateTime
                }).Item1;

                var timeSheets = _client.GetTimesheets(new Intuit.TSheets.Model.Filters.TimesheetFilter()
                {
                    ModifiedSince = lastUpdateTime
                }).Item1;
                

                var timeEntries = _mapper.Map<IList<TstimeEntries>>(timeSheets);
                var deletedTimeEntries = _mapper.Map<IList<TstimeEntriesDeleted>>(deletedTimeSheets);

                var policy = Policy.Handle<Exception>()
                                   .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                policy.Execute(() =>
                {
                    _logger.LogInformation($"[TimeSheets]: Bulk insert is starting");
                    if (timeSheets.Count > 0)
                        _context.BulkInsertOrUpdate<TstimeEntries>(timeEntries);
                    if (deletedTimeSheets.Count > 0)
                        _context.BulkInsertOrUpdate<TstimeEntriesDeleted>(deletedTimeEntries);

                    _logger.LogInformation($"[TimeSheets]: Bulk insert completed");

                    UpdateTableInfo(new string[] { nameof(TstimeEntries) }, functionStartedTime);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MasterDataCopy()
        {
            DateTime functionStartedTime = DateTime.UtcNow;

            Task<(IList<Jobcode>, ResultsMeta)> jobCodeTask = _client.GetJobcodesAsync();
            Task<(IList<User>, ResultsMeta)> userTask = _client.GetUsersAsync();

            _logger.LogInformation($"[Engagements & Clients]: Data fetching from TSheet API");

            Task.WaitAll(jobCodeTask, userTask);

            var engagementEntities = _mapper.Map<IList<Tsengagements>>(jobCodeTask.Result.Item1.Where(x => x.ParentId.HasValue && x.ParentId > 0));
            var clientEntities = _mapper.Map<IList<Tsclients>>(jobCodeTask.Result.Item1.Where(x => !x.ParentId.HasValue || (x.ParentId.HasValue && x.ParentId == 0)));
            var userEntities = _mapper.Map<IList<Tsusers>>(userTask.Result.Item1);

            var policy = Policy.Handle<Exception>()
                               .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _logger.LogInformation($"[Engagements & Clients]: Bulk insert is starting");
                if (engagementEntities.Count > 0)
                    _context.BulkInsertOrUpdate<Tsengagements>(engagementEntities);

                if (clientEntities.Count > 0)
                    _context.BulkInsertOrUpdate<Tsclients>(clientEntities);
                _logger.LogInformation($"[Engagements & Clients]: Bulk insert completed");

                _logger.LogInformation($"[User]: Bulk insert is starting");
                if (userEntities.Count > 0)
                    _context.BulkInsertOrUpdate<Tsusers>(userEntities);
                _logger.LogInformation($"[User]: Bulk insert completed");

                UpdateTableInfo(new string[] { nameof(Tsusers), nameof(Tsclients), nameof(Tsengagements) }, functionStartedTime);
            });
        }

        private void UpdateTableInfo(string[] tableNames, DateTime updatedTime)
        {
            var updateInfoTables = new List<ScheduledJobInfo>();
            foreach (var tableName in tableNames)
            {
                updateInfoTables.Add(new ScheduledJobInfo()
                {
                    TableName = tableName,
                    LastUpdateTime = updatedTime
                });
            }
            _context.BulkInsertOrUpdate<ScheduledJobInfo>(updateInfoTables);
            _context.SaveChanges();
        }
    }
}
