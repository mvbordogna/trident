using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Trident.Api.Search
{
    public class SearchCriteriaModelConverter : JsonConverter<SearchCriteriaModel>
    {
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {

        };

        private readonly JsonSerializer _nonCirclularSerializer;

        public SearchCriteriaModelConverter()
        {

            _nonCirclularSerializer = new JsonSerializer();
            var thisConverters = _nonCirclularSerializer.Converters.Where(x => x.GetType() == typeof(SearchCriteriaModelConverter)).ToList();
            thisConverters.ForEach(x => _nonCirclularSerializer.Converters.Remove(x));

        }

        public override bool CanWrite => false;


        public override void WriteJson(JsonWriter writer, SearchCriteriaModel value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override SearchCriteriaModel ReadJson(JsonReader reader, Type objectType, SearchCriteriaModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader?.TokenType == JsonToken.Null) return null;
            SearchCriteriaModel model = Activator.CreateInstance(objectType) as SearchCriteriaModel;
            JObject item = JObject.Load(reader);
            serializer.Populate(item.CreateReader(), model);

            var objFiltersKeyValues = model?.Filters.ToList();

            if (objFiltersKeyValues != null)
            {
                foreach (var kp in model?.Filters)
                {
                    if (kp.Value != null)
                    {
                        if (kp.Value is JObject obj)
                        {
                            if (obj.ContainsKey(nameof(AxiomModel.Field)))
                            {
                                model.Filters[kp.Key] = JsonConvert.DeserializeObject<AxiomModel>(obj.ToString(), _jsonSettings);
                            }
                            else if (obj.ContainsKey(nameof(AxiomFilterModel.Axioms)))
                            {
                                model.Filters[kp.Key] = JsonConvert.DeserializeObject<AxiomFilterModel>(obj.ToString(), _jsonSettings);
                            }
                            else
                            {
                                model.Filters[kp.Key] = JsonConvert.DeserializeObject<CompareModel>(obj.ToString(), _jsonSettings);
                            }
                        }
                        else if (kp.Value is JArray ary)
                        {
                            var distinctJTokenType = ary.Select(x => x.Type).Distinct().ToList();

                            var targetType = (distinctJTokenType.Count() == 1 && JTokenTypeDotNetTypeLookupDict.ContainsKey(distinctJTokenType.First()))
                                ? JTokenTypeDotNetTypeLookupDict[distinctJTokenType.First()]
                                : typeof(object);

                            var listType = targetType.MakeArrayType();
                            model.Filters[kp.Key] = ary.ToObject(listType); 
                        }
                    }
                }
            }

            return model;
        }

        private Dictionary<JTokenType, Type> JTokenTypeDotNetTypeLookupDict = new Dictionary<JTokenType, Type>()
        {
            {JTokenType.String,typeof(string)},
            {JTokenType.Integer, typeof(int)},
            {JTokenType.Float, typeof(float)},
            {JTokenType.Boolean, typeof(bool)},
            {JTokenType.Date, typeof(DateTime)},
            {JTokenType.Bytes, typeof(byte)},
            {JTokenType.Guid, typeof(Guid)},
            {JTokenType.TimeSpan, typeof(TimeSpan)},

        };
    }
}
