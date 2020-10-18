using AutoMapper;
using Intuit.TSheets.Api;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Data.Models;
using SNS.FunctionApp;
using SNS.FunctionApp.Helper;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(Startup))]
namespace SNS.FunctionApp
{
    class Startup : IWebJobsStartup
    {
        public IConfiguration Configuration { get; }
        public void Configure(IWebJobsBuilder builder)
        {
            // "Server=CPX-U31NCQLACF4;Database=avivus;Trusted_Connection=True;";
            var cnnStr = Configuration.GetConnectionString("AzureDB");
            string connection = Environment.GetEnvironmentVariable("ConnectionStrings", EnvironmentVariableTarget.Process);


            // Configure services
            builder.Services.AddOptions();
            //builder.Services.AddScoped<ShopmonkeyClient>(x => new ShopmonkeyClient("https://api.shopmonkey.io/v1"));
            builder.Services.AddDbContext<SNSTimeTrackerContext>(options => options.UseSqlServer(connection));
            builder.Services.AddScoped<DbContext, SNSTimeTrackerContext>();
            builder.Services.AddAutoMapper(typeof(AutoMapping));
        }
    }
}
