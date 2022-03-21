using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Scholar.Framework.Core.Logging;
using System;
using System.Threading.Tasks;

namespace Scholar.Framework.Azure.Common
{
    public class ExceptionLoggingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IAppLogger _appLoger;

        public ExceptionLoggingMiddleware(IAppLogger appLoger)
        {
            _appLoger = appLoger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                // Code before function execution here
                _appLoger.Adapter.SetCallContext(context);
                await next(context);
                // Code after function execution here
            }
            catch (Exception ex)
            {
                var log = context.GetLogger<ExceptionLoggingMiddleware>();

            }
        }

    }
}
