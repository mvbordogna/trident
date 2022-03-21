using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trident.Samples.Web.Shared;

namespace Trident.Samples.AzureFunctions
{
    public class WeatherFunctions
    {

        public WeatherFunctions(ITestInject test) 
        {
            _test = test;
        }


        [Function("WeatherForcast")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");


            var rng = new Random();
            var resultsString = JsonConvert.SerializeObject(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray());

            response.WriteString(resultsString);

            return response;
        }




        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly ITestInject _test;
    }
}
