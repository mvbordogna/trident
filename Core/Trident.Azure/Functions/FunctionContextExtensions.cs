using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Trident.Azure.Functions
{
    public static class FunctionContextExtensions
    {
        public static HttpRequestData GetHttpRequestData(this FunctionContext functionContext)
        {
            KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            object functionBindingsFeature = keyValuePair.Value;
            Type type = functionBindingsFeature.GetType();
            var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
            return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
        }



        public static object GetHttpResponseObject(this FunctionContext functionContext)
        {
            KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            object functionBindingsFeature = keyValuePair.Value;
            Type type = functionBindingsFeature.GetType();
            var streamPropertyInfo = type.GetProperties().Single(p => p.Name == "InvocationResult");
            var existing = streamPropertyInfo.GetValue(functionBindingsFeature);

            if (existing == null)
            {
                var req = functionContext.GetHttpRequestData();
                existing = req.CreateResponse();
                streamPropertyInfo.SetValue(functionBindingsFeature, existing);
            }

            return existing;
        }



        public static void OverwriteResponseStream(this FunctionContext functionContext, string output, HttpStatusCode httpStatusCode)
        {
            var grpcRespponse = functionContext.GetHttpResponseObject();

            if (grpcRespponse != null)
            {
                //set status code
                grpcRespponse.GetType().GetProperty("StatusCode").SetValue(grpcRespponse, httpStatusCode);

                //overwrite stream
                var stream = grpcRespponse.GetType().GetProperty("Body").GetValue(grpcRespponse) as Stream;
                stream.Position = 0;
                stream.SetLength(0);
                var sw = new StreamWriter(stream);
                sw.Write(output);
                sw.Flush();
            }
        }



        public static string ReadResponseStream(this FunctionContext functionContext)
        {
            var grpcRespponse = functionContext.GetHttpResponseObject();
            if (grpcRespponse != null)
            {
                //overwrite stream
                var stream = grpcRespponse.GetType().GetProperty("Body").GetValue(grpcRespponse) as Stream;
                stream.Position = 0;
                stream.SetLength(0);
                var sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }

            return null;
        }



    }
}
