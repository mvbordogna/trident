using Trident.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trident.Domain;

namespace Trident.Web
{
    /// <summary>
    /// Class ValidationExtensions.
    /// </summary>
    public static class ValidationExtensions
    {

        /// <summary>
        /// Used for MVC Model State errors
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="request">The request.</param>
        /// <param name="modelState">State of the model.</param>
        /// <returns>HttpResponseMessage.</returns>
        public static HttpResponseMessage CreateModelStateValidationErrorResponse<TErrorCodes, TEntity>(
            this ValidationRollupException<TErrorCodes, TEntity> exception,
            HttpRequestMessage request, ModelStateDictionary modelState)
            where TErrorCodes : struct
            where TEntity : Entity

        {
            exception.ValidationResults.ToList().ForEach(x =>
            {
                foreach (var member in x.MemberNames)
                {
                    modelState.AddModelError(member, x.Message);
                }
            });

            var response = request.CreateResponse(HttpStatusCode.BadRequest, modelState);
            return response;
        }

        /// <summary>
        /// Used with WebApi Responses
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="request">The request.</param>
        /// <returns>HttpResponseMessage.</returns>

        public static HttpResponseMessage CreateValidationErrorResponse(this ValidationRollupException exception,
          HttpRequestMessage request)
        {
            var response = request.CreateResponse(HttpStatusCode.BadRequest, exception.ValidationResults);
            return response;
        }

    }
}
