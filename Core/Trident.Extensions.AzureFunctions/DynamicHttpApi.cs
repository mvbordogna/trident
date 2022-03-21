//using AutoMapper;
//using Microsoft.Azure.Functions.Worker.Http;
//using Newtonsoft.Json;
//using Scholar.Framework.Core.Api;
//using Scholar.Framework.Core.Api.Search;
//using Scholar.Framework.Core.Configuration;
//using Scholar.Framework.Core.Domain;
//using Scholar.Framework.Core.Logging;
//using Scholar.Framework.Core.Search;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//namespace Scholar.Framework.Azure.Common
//{
//    public class DynamicHttpApi<TModel, TEntity, T> : IDynamicHttpApi
//        where TEntity : DynamicEntity<T>
//        where TModel : BaseApiModel<Guid>
//    {
//        private readonly IDynamicConfiguration _dynamicConfig;
//        private readonly DynamicStackConfiguration _currentStackConfig;

//        protected IManager<Guid, TEntity> Manager { get; }
//        protected IAppLogger AppLogger { get; }
//        protected IMapper Mapper { get; }

//        public DynamicHttpApi(
//            IAppLogger appLogger,
//            IMapper mapper,
//            IManager<Guid, TEntity> manager,
//            IDynamicConfiguration dynamicConfig)
//        {
//            AppLogger = appLogger;
//            Mapper = mapper;
//            Manager = manager;
//            _dynamicConfig = dynamicConfig;
//            _currentStackConfig = _dynamicConfig.Stacks.First(x => x.DynamicType == typeof(T));
//        }

//        public async Task<HttpResponseData> Search(HttpRequestData req, string logName, Dictionary<string, object> validate = null)
//        {
//            HttpResponseData response = null;

//            try
//            {
//                string json = await new StreamReader(req.Body).ReadToEndAsync();
//                var model = JsonConvert.DeserializeObject<SearchCriteriaModel>(json);
//                if (validate != null)
//                {
//                    foreach (var entry in validate)
//                    {
//                        model.AddFilter(entry.Key, entry.Value);
//                    }
//                }
//                AppLogger.LogInformation($"{logName} - Received Request", new Dictionary<string, object> { { "Details", json } });

//                if (model != null)
//                {
//                    var criteriaEntity = Mapper.Map<SearchCriteria<DynamicEntity<T>>>(model);
//                    var results = await Manager.Search(criteriaEntity);
//                    var searchResult = Mapper.Map<SearchResultsModel<TModel, SearchCriteriaModel>>(results);

//                    response = req.CreateResponse(HttpStatusCode.OK);
//                    await response.WriteAsJsonAsync(searchResult);
//                }
//                else
//                {
//                    response = req.CreateResponse(HttpStatusCode.BadRequest);
//                }

//                AppLogger.LogInformation($"{logName} - Completed");

//            }
//            catch (Exception ex)
//            {
//                AppLogger.LogError(ex, ex.Message);
//                response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            }

//            return response;
//        }

//        public async Task<HttpResponseData> GetById(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null)
//        {
//            HttpResponseData response;

//            try
//            {
//                AppLogger.LogInformation($"{logName} - Received Request", new Dictionary<string, object> { { "Details", new { id } } });

//                var entity = await Manager.GetById(id);
//                if (entity != null)
//                {
//                    var model = Mapper.Map<TModel>(entity);
//                    if (isValidUrlParameters != null && !isValidUrlParameters(model))
//                    {
//                        throw new ArgumentException("Invalid Url Parameters.");
//                    }
//                    response = req.CreateResponse(HttpStatusCode.OK);
//                    await response.WriteAsJsonAsync(model);
//                }
//                else
//                {
//                    response = req.CreateResponse(HttpStatusCode.NotFound);
//                }

//                AppLogger.LogInformation($"{logName} - Completed");

//            }
//            catch (ArgumentException argEx)
//            {
//                AppLogger.LogError(argEx, argEx.Message);
//                response = req.CreateResponse(HttpStatusCode.BadRequest);
//            }
//            catch (Exception ex)
//            {
//                AppLogger.LogError(ex, ex.Message);
//                response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            }

//            return response;
//        }

//        public async Task<HttpResponseData> Update(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null)
//        {
//            HttpResponseData response;
//            try
//            {
//                string json = await new StreamReader(req.Body).ReadToEndAsync();
//                var request = JsonConvert.DeserializeObject<TModel>(json);
//                AppLogger.LogInformation($"{logName} - Received Request", new Dictionary<string, object> { { "Details", json } });

//                if (!id.Equals(request.Id))
//                {
//                    throw new ArgumentException("Invalid Id provided.");
//                }

//                if (isValidUrlParameters != null && !isValidUrlParameters(request))
//                {
//                    throw new ArgumentException("Invalid Url Parameters.");
//                }

//                var entity = Mapper.Map<TEntity>(request);
//                SetStorageProfile(entity);
//                var result = await Manager.Save(entity);
//                var model = Mapper.Map<TModel>(result);

//                response = req.CreateResponse(HttpStatusCode.OK);
//                await response.WriteAsJsonAsync(model);

//                AppLogger.LogInformation($"{logName} - Completed");

//            }
//            catch (ArgumentException argEx)
//            {
//                AppLogger.LogError(argEx, argEx.Message);
//                response = req.CreateResponse(HttpStatusCode.BadRequest);
//            }
//            catch (Exception ex)
//            {
//                AppLogger.LogError(ex, ex.Message);
//                response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            }

//            return response;
//        }

//        public async Task<HttpResponseData> Create(HttpRequestData req, string logName, Func<object, bool> isValidUrlParameters = null)
//        {
//            HttpResponseData response;

//            try
//            {
//                string json = await new StreamReader(req.Body).ReadToEndAsync();
//                var request = JsonConvert.DeserializeObject<TModel>(json);

//                if (isValidUrlParameters != null && !isValidUrlParameters(request))
//                {
//                    throw new ArgumentException("Invalid Url Parameters.");
//                }

//                AppLogger.LogInformation($"{logName} - Received Request", new Dictionary<string, object> { { "Details", json } });
//                var entity = Mapper.Map<TEntity>(request);
//                SetStorageProfile(entity);
//                var result = await Manager.Save(entity);
//                var model = Mapper.Map<TModel>(result);
//                response = req.CreateResponse(HttpStatusCode.OK);
//                await response.WriteAsJsonAsync(model);
//                AppLogger.LogInformation($"{logName} - Completed");

//            }
//            catch (ArgumentException argEx)
//            {
//                AppLogger.LogError(argEx, argEx.Message);
//                response = req.CreateResponse(HttpStatusCode.BadRequest);
//            }
//            catch (Exception ex)
//            {
//                AppLogger.LogError(ex, ex.Message);
//                response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            }

//            return response;
//        }


//        private void SetStorageProfile(TEntity entity)
//        {
//            entity.SetDiscriminatorValue(_currentStackConfig.DiscriminatorValue);
//            entity.SetDocTypeValue(_currentStackConfig.PartitionKeyValue);
//        }
        
//        public async Task<HttpResponseData> Delete(HttpRequestData req, Guid id, string logName, Func<object, bool> isValidUrlParameters = null)
//        {
//            HttpResponseData response;
//            try
//            {
//                string json = await new StreamReader(req.Body).ReadToEndAsync();
//                var request = JsonConvert.DeserializeObject<TModel>(json);
//                AppLogger.LogInformation($"{logName} - Received Request", new Dictionary<string, object> { { "Details", json } });

//                if (!id.Equals(request.Id))
//                {
//                    throw new ArgumentException("Invalid Id provided.");
//                }

//                if (isValidUrlParameters != null && !isValidUrlParameters(request))
//                {
//                    throw new ArgumentException("Invalid Url Parameters.");
//                }
//                var entity = Mapper.Map<TEntity>(request);
//                try
//                {
//                    SetStorageProfile(entity);
//                    var result = await Manager.Delete(entity); //This always returns true and will break if you pass a bad Id
//                }
//                catch
//                {
//                    throw new ArgumentOutOfRangeException("Record Not Found To Delete.");
//                }

//                var model = Mapper.Map<TModel>(entity);
//                response = req.CreateResponse(HttpStatusCode.OK);
//                await response.WriteAsJsonAsync(model);
//                AppLogger.LogInformation($"{logName} - Completed");

//            }
//            catch (ArgumentOutOfRangeException argOutEx)
//            {
//                AppLogger.LogError(argOutEx, argOutEx.Message);
//                response = req.CreateResponse(HttpStatusCode.NotFound);
//            }
//            catch (ArgumentException argEx)
//            {
//                AppLogger.LogError(argEx, argEx.Message);
//                response = req.CreateResponse(HttpStatusCode.BadRequest);
//            }
//            catch (Exception ex)
//            {
//                AppLogger.LogError(ex, ex.Message);
//                response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            }

//            return response;
//        }       
//    }
//}
