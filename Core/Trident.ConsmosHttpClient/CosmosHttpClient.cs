using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Trident.Cosmos.Contracts;
using System.Linq;

namespace Trident.Cosmos
{
    public class CosmosHttpClient
    {
        private readonly string endpoint = "";
        private readonly string masterKey = "";
        private readonly Uri baseUri;

        /// <summary>
        /// The user identifier based. If true use the user created ids if false expects cosmos generated ids like _rId value types
        /// </summary>
        bool userIdBased = true;

        string utc_date { get => DateTime.UtcNow.ToString("r"); }

        public CosmosHttpClient(string connectionString, bool userIdBased = true)
        {
            var items = connectionString.Split(";");
            var dict = items.ToDictionary(x => x.Split("=")[0].Trim(), x =>
            {
                var pos = x.IndexOf("=") +1;
                return x.Substring(pos, x.Length - pos).Trim();

            });

            this.masterKey = dict["AccountKey"];
            this.endpoint = dict["AccountEndpoint"];
            this.baseUri = new Uri(endpoint);
            this.userIdBased = userIdBased;
        }


        public CosmosHttpClient(string masterKey, string endpoint, bool userIdBased = true)
        {
            this.masterKey = masterKey;
            this.endpoint = endpoint;
            this.baseUri = new Uri(endpoint);
            this.userIdBased = userIdBased;
        }

        public async Task<List<CosmosDatabase>> GetAllDatabases()
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");

            //LIST all databases
            var verb = "GET";
            var resourceType = "dbs";
            var resourceId = string.Empty;
            var resourceLink = string.Format("dbs");

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetStringAsync(new Uri(baseUri, resourceLink));
            var data = (JObject)JsonConvert.DeserializeObject(response);
            var results = data.Property("Databases").Value.ToObject<List<CosmosDatabase>>();
            return results;
        }

        public async Task<CosmosDatabase> GetDatabaseById(string databaseId)
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");

            //GET a database
            var verb = "GET";
            var resourceType = "dbs";
            var resourceLink = "dbs/" + databaseId;
            var resourceId = (userIdBased) ? resourceLink : databaseId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetStringAsync(new Uri(baseUri, resourceLink));

            return JsonConvert.DeserializeObject<CosmosDatabase>(response);
        }

        public async Task<List<CosmosCollection>> GetAllCollections(string databaseId)
        {

            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");

            //LIST all collections
            var verb = "GET";
            var resourceType = "colls";
            var resourceLink = string.Format("dbs/{0}/colls", databaseId);
            var resourceId = (userIdBased) ? string.Format("dbs/{0}", databaseId) : databaseId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetStringAsync(new Uri(baseUri, resourceLink));

            var data = (JObject)JsonConvert.DeserializeObject(response);
            var results = data.Property("DocumentCollections").Value.ToObject<List<CosmosCollection>>();
            return results;

        }

        public async Task<CosmosCollection> GetCollectionById(string databaseId, string collectionId)
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");

            //GET a collections
            var verb = "GET";
            var resourceType = "colls";
            var resourceLink = string.Format("dbs/{0}/colls/{1}", databaseId, collectionId);
            var resourceId = (userIdBased) ? resourceLink : collectionId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetStringAsync(new Uri(baseUri, resourceLink));
            return JsonConvert.DeserializeObject<CosmosCollection>(response);
        }

        public async Task<List<T>> GetAllDocuments<T>(string databaseId, string collectionId)
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");

            //LIST all documents in a collection
            var verb = "GET";
            var resourceType = "docs";
            var resourceLink = string.Format("dbs/{0}/colls/{1}/docs", databaseId, collectionId);
            var resourceId = (userIdBased) ? string.Format("dbs/{0}/colls/{1}", databaseId, collectionId) : collectionId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetStringAsync(new Uri(baseUri, resourceLink));
            var data = (JObject)JsonConvert.DeserializeObject(response);
            var results = data.Property("Documents").Value.ToObject<List<T>>();
            return results;
        }

        public async Task<T> GetDocumentById<T>(string databaseId, string collectionId, string partitionKey, string documentId)
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");
            client.DefaultRequestHeaders.Add("x-ms-documentdb-query-enablecrosspartition", "True");
            client.DefaultRequestHeaders.Add("x-ms-documentdb-partitionkey", " [ \"" + partitionKey + "\" ] ");
            //GET a document
            var verb = "GET";
            var resourceType = "docs";
            var resourceLink = string.Format("dbs/{0}/colls/{1}/docs/{2}", databaseId, collectionId, documentId);
            var resourceId = (userIdBased) ? resourceLink : documentId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            var response = await client.GetAsync(new Uri(baseUri, resourceLink));
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data);
        }


        public async Task<List<T>> ExecuteQuery<T>(string databaseId, string collectionId, string query)
        {
            var client = HttpClientFactory.Create();
            var timeStamp = utc_date;
            client.DefaultRequestHeaders.Add("x-ms-date", timeStamp);
            client.DefaultRequestHeaders.Add("x-ms-version", "2015-12-16");
            client.DefaultRequestHeaders.Add("x-ms-documentdb-isquery", "True");
            client.DefaultRequestHeaders.Add("x-ms-documentdb-query-enablecrosspartition", "True");
            //EXECUTE a query
            var verb = "POST";
            var resourceType = "docs";
            var resourceLink = string.Format("dbs/{0}/colls/{1}/docs", databaseId, collectionId);
            var resourceId = (userIdBased) ? string.Format("dbs/{0}/colls/{1}", databaseId, collectionId) : collectionId.ToLowerInvariant();

            var authHeader = GenerateMasterKeyAuthorizationSignature(timeStamp, verb, resourceId, resourceType, masterKey);

            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", authHeader);


            var qry = new SqlQuerySpec { query = query };
            var content = new StringContent(JsonConvert.SerializeObject(qry));
            content.Headers.ContentType.MediaType = "application/query+json";
            content.Headers.ContentType.CharSet = "";
            var msg = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, resourceLink))
            {
                Content = content
            };
            var response = await client.SendAsync(msg);
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ResultsWrapper<T>>(body);


            var results = data.Documents;
            return results;
        }

        private class ResultsWrapper<T>
        {
            public List<T> Documents { get; set; }
        }

        private static string GenerateMasterKeyAuthorizationSignature(string utcDate, string verb, string resourceId, string resourceType, string key, string keyType = "master", string tokenVersion = "1.0")
        {
            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };

            string payLoad = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n",
                    verb.ToLowerInvariant(),
                    resourceType.ToLowerInvariant(),
                    resourceId,
                    utcDate.ToLowerInvariant(),
                    ""
            );

            byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
            string signature = Convert.ToBase64String(hashPayLoad);

            return System.Web.HttpUtility.UrlEncode(String.Format(System.Globalization.CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}",
                keyType,
                tokenVersion,
                signature));
        }

        class SqlQuerySpec
        {
            public string query { get; set; }
            public SqlQueryParameter[] parameters { get; set; } = new SqlQueryParameter[0];
        }

        class SqlQueryParameter
        {
            public string name { get; set; }
            public string value { get; set; }
        }
    }
}
