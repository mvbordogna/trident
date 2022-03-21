using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scholar.Framework.Azure.Common
{
    public interface IDynamicHttpApi
    {
        Task<HttpResponseData> Create(HttpRequestData req, string logName, Func<object, bool> isValidUrlParameters = null);
        Task<HttpResponseData> Delete(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null);
        Task<HttpResponseData> GetById(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null);
        Task<HttpResponseData> Search(HttpRequestData req, string logName, Dictionary<string, object> validate = null);
        Task<HttpResponseData> Update(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null);
    }
}
