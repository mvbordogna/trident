using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Trident.Api.Search;
using Trident.Contracts;
using Trident.Contracts.Api;
using Trident.Domain;
using Trident.Logging;
using Trident.Search;

namespace Trident.Azure.Functions
{
    public abstract class ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId> : IFunctionController
        where TEntity : EntityBase<TId>
        where TModel : ApiModelBase<TId>
    {
        protected IManager<TId, TEntity> Manager { get; }
        protected ILog AppLogger { get; }
        protected Mapper.IMapperRegistry Mapper { get; }

        protected ReadOnlyHttpFunctionApiBase(ILog appLogger, Mapper.IMapperRegistry mapper, IManager<TId, TEntity> manager)
        {
            AppLogger = appLogger;
            Mapper = mapper;
            Manager = manager;
        }

        protected async Task<HttpResponseData> Search(HttpRequestData req, string logName, Dictionary<string, object> validate = null)
        {
            HttpResponseData response = null;

            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<SearchCriteriaModel>(json);
                if (validate != null)
                {
                    foreach (var entry in validate)
                    {
                        model.AddFilter(entry.Key, entry.Value);
                    }
                }
                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Data", json } });

                if (model != null)
                {
                    var criteriaEntity = Mapper.Map<SearchCriteria<TEntity>>(model);
                    var results = await Manager.Search(criteriaEntity);
                    var searchResult = Mapper.Map<SearchResultsModel<TModel, SearchCriteriaModel>>(results);

                    response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(searchResult);
                }
                else
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }

                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (Exception ex)
            {
                AppLogger.Error<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }
        protected async Task<HttpResponseData> GetAll(HttpRequestData req, string logName)
        {
            HttpResponseData response;

            try
            {
                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request");

                var entities = await Manager.Get();
                if (entities != null && entities.Any())
                {
                    var models = Mapper.Map<IEnumerable<TModel>>(entities);
                    response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(models);
                }
                else
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                }

                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (Exception ex)
            {
                AppLogger.Error<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }
        protected async Task<HttpResponseData> GetById(HttpRequestData req, TId id, string logName, Func<object, bool> isValidUrlParameters = null)
        {
            HttpResponseData response;

            try
            {
                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Details", new { id } } });

                var entity = await Manager.GetById(id);
                if (entity != null)
                {
                    var model = Mapper.Map<TModel>(entity);
                    if (isValidUrlParameters != null && !isValidUrlParameters(model))
                    {
                        throw new ArgumentException("Invalid Url Parameters.");
                    }
                    response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(model);
                }
                else
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                }

                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (ArgumentException argEx)
            {
                AppLogger.Error<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(argEx, argEx.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                AppLogger.Error<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        protected async Task<HttpResponseData> Get(HttpRequestData req, Expression<Func<TEntity, bool>> exp, string logName)
        {
            HttpResponseData response;

            try
            {
                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Received Request", propertyValues: new Dictionary<string, object> { { "Details", new { exp } } });

                var entities = await Manager.Get(exp);
                if (entities != null)
                {
                    var models = Mapper.Map<IEnumerable<TModel>>(entities);
                    response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(models);
                }
                else
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                }

                AppLogger.Information<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(messageTemplate: $"{logName} - Completed");

            }
            catch (Exception ex)
            {
                AppLogger.Error<ReadOnlyHttpFunctionApiBase<TModel, TEntity, TId>>(ex, ex.Message);
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

    }
}
