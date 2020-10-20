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
    public class MasterDataCopy
    {
        private readonly DataService _client;
        private readonly SNSTimeTrackerContext _context;
        private readonly IMapper _mapper;
        public MasterDataCopy(SNSTimeTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            string authToken = Environment.GetEnvironmentVariable("AuthToken", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            _client = TSheetsDataServiceFactory.CreateDataService(authToken, null);
        }

        [FunctionName("MasterDataCopy")]
        public void Run([TimerTrigger("%MasterDataInterval%")]TimerInfo myTimer, ILogger log)
        {
            var tsheetsDataCopy = new TSheetsDataCopy(_context, _mapper, log);

            log.LogInformation($"Master data copy function executed at: {DateTime.UtcNow}");

            tsheetsDataCopy.MasterDataCopy();

            log.LogInformation($"Master data copy function finished at: {DateTime.UtcNow}");
        }
    }
}
