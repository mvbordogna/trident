using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trident.IoC;
using Trident.Logging;

namespace Trident.Azure.Functions
{
    public class ExceptionLoggingMiddleware : IFunctionsWorkerMiddleware
    {

        private readonly IIoCServiceLocator _locator;

        public ExceptionLoggingMiddleware(IIoCServiceLocator locator)
        {
            _locator = locator;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                var logger = (ILog)context.InstanceServices.GetService(typeof(ILog));
                logger.SetCallContext(context);
                logger =_locator.Get<ILog>();
                logger.SetCallContext(context);
                await next(context);
            }
            catch (Exception ex)
            {
                var log = (ILogger) context.GetLogger<ExceptionLoggingMiddleware>();
                log.LogError(ex, ex.Message); 
            }
        }

    }
}
