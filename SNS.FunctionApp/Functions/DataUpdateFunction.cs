using AutoMapper;
using Intuit.TSheets.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SNS.Data.Models;
using SNS.FunctionApp.Business;
using System;

namespace SNS.FunctionApp.Functions
{
    public class DataUpdateFunction
    {
        private readonly DataService _client;
        private readonly SNSTimeTrackerContext _context;
        private readonly IMapper _mapper;
        public DataUpdateFunction(SNSTimeTrackerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            string authToken = Environment.GetEnvironmentVariable("AuthToken", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            _client = TSheetsDataServiceFactory.CreateDataService(authToken, null);

        }

        [FunctionName("DataUpdateFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            var tsheetsDataCopy = new TSheetsDataCopy(_context, _mapper, log);

            log.LogInformation("C# HTTP trigger function processed a request.");

            tsheetsDataCopy.TimeSheetCopy();
            tsheetsDataCopy.MasterDataCopy();

            return new OkObjectResult("Success");
        }
    }
}
