using Trident.Contracts.Api.Validation;
using System.Collections.Generic;
using System.Net;

namespace Trident.Contracts.Api.Client
{
    public class Response<TModel, TErrorCodes>
        where TModel : class
        where TErrorCodes : struct
    {
        public TModel Model { get; set; }
        public IEnumerable<ValidationResult<TErrorCodes, TModel>> ValidationErrors { get; set; }

        public string ValidationSummary { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode
        {
            get
            {
                var val = (int)StatusCode;
                return val is not (< 200 or >= 400);
            }
        }
        public string ResponseContent { get; set; }
        public System.Exception Exception { get; set; }
    }

    public class Response<TModel> : Response<TModel, ErrorCodes>
      where TModel : class
    { }

}
