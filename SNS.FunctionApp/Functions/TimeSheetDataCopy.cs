using AutoMapper;
using Intuit.TSheets.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SNS.Data.Models;
using SNS.FunctionApp.Business;
using System;

namespace SNS.FunctionApp.Functions
{
    public class TimeSheetDataCopy
    {
        private readonly DataService _client;
        private readonly SNSTimeTrackerContext _context;
        private readonly IMapper _mapper;
        public TimeSheetDataCopy(SNSTimeTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            string authToken = Environment.GetEnvironmentVariable("AuthToken", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            _client = TSheetsDataServiceFactory.CreateDataService(authToken, null);
        }

        [FunctionName("TimeSheetDataCopy")]
        public void Run([TimerTrigger("%TimeSheetDataInterval%")]TimerInfo myTimer, ILogger log)
        {
            var tsheetsDataCopy = new TSheetsDataCopy(_context, _mapper, log);

            log.LogInformation($"TimeSheet data copy function executed at: {DateTime.UtcNow}");

            tsheetsDataCopy.TimeSheetCopy();

            log.LogInformation($"TimeSheet data copy function finished at: {DateTime.UtcNow}");
        }
    }
}
