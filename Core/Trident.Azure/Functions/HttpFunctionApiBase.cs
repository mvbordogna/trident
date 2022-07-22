
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Trident.Contracts;
using Trident.Contracts.Api;
using Trident.Domain;
using Trident.Logging;
using Trident.Validation;

namespace Trident.Azure.Functions
{
    public abstract class HttpFunctionApiBase<TModel, TEntity, TId> : ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>, IFunctionController
        where TEntity : EntityBase<TId>
        where TModel : ApiModelBase<TId>
    {
        protected HttpFunctionApiBase(ILog log, Mapper.IMapperRegistry mapper, IManager<TId, TEntity> manager) : base(log, mapper, manager)
        {
        }      

        protected async Task<HttpResponseData> Update(HttpRequestData req, TId id, string logName, Func<object, bool> isValidUrlParameters = null)
        {
            HttpResponseData response;
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<TModel>(json);
                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Data", json } });

                if (!id.Equals(request.Id))
                {
                    throw new ArgumentException("Invalid Id provided.");
                }
               
                if (isValidUrlParameters != null && !isValidUrlParameters(request))
                {
                    throw new ArgumentException("Invalid Url Parameters.");
                }

                var entity = Mapper.Map<TEntity>(request);
                var result = await Manager.Save(entity);
                var model = Mapper.Map<TModel>(result);

                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(model);

                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (ValidationRollupException validationRollupException)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{nameof(TModel)} - Validation Exception - {validationRollupException.Message}");
                response = req.CreateResponse();
                await response.WriteAsJsonAsync(validationRollupException.ValidationResults);
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (ArgumentException argEx)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(argEx, argEx.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
           


            return response;
        }

        protected async Task<HttpResponseData> Create(HttpRequestData req, string logName)
        {
            HttpResponseData response;

            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<TModel>(json);         

                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Data", json } });
                var entity = Mapper.Map<TEntity>(request);               
                var result = await Manager.Save(entity);
                var model = Mapper.Map<TModel>(result);
                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(model);
                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (ValidationRollupException validationRollupException)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{nameof(TModel)} - Validation Exception - {validationRollupException.Message}");
                response = req.CreateResponse();
                await response.WriteAsJsonAsync(validationRollupException.ValidationResults);
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (ArgumentException argEx)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(argEx, argEx.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        protected async Task<HttpResponseData> Delete(HttpRequestData req, TId id, string logName, Func<object, bool> isValidUrlParameters = null)
        {
            HttpResponseData response;
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<TModel>(json);
                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate:  $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Data", json } });

                if (!id.Equals(request.Id))
                {
                    throw new ArgumentException("Invalid Id provided.");
                }

                if (isValidUrlParameters != null && !isValidUrlParameters(request))
                {
                    throw new ArgumentException("Invalid Url Parameters.");
                }

                var entity = Mapper.Map<TEntity>(request);
                try
                {
                    var result = await Manager.Delete(entity); //This always returns true and will break if you pass a bad Id
                }
                catch
                {
                    throw new ArgumentOutOfRangeException("Record Not Found To Delete.");
                }

                var model = Mapper.Map<TModel>(entity);
                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(model);
                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (ValidationRollupException validationRollupException)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{nameof(TModel)} - Validation Exception - {validationRollupException.Message}");
                response = req.CreateResponse();
                await response.WriteAsJsonAsync(validationRollupException.ValidationResults);
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (ArgumentOutOfRangeException argOutEx)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(argOutEx, argOutEx.Message);
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (ArgumentException argEx)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(argEx, argEx.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        protected async Task<HttpResponseData> Patch(HttpRequestData req, TId id, string logName, Func<object, bool> isValidUrlParameters = null)
        {
            HttpResponseData response;
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                var patchDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Data", json } });
                          
                var result = await Manager.Patch(id, patchDictionary);
                var model = Mapper.Map<TModel>(result);

                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(model);

                AppLogger.Information<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (ValidationRollupException validationRollupException)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{nameof(TModel)} - Validation Exception - {validationRollupException.Message}");
                response = req.CreateResponse();
                await response.WriteAsJsonAsync(validationRollupException.ValidationResults);
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (ArgumentException argEx)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(argEx, argEx.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                AppLogger.Error<HttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }


    }
}
