using AutoMapper;
using RestSharp;
using Trident.Rest;
using RestRequest = Trident.Rest.RestRequest;
using RestSharpMethod = RestSharp.Method;
using RestSharpParameterType = RestSharp.ParameterType;
using RestSharpRestRequest = RestSharp.RestRequest;
using RestSharpRestParameter = RestSharp.Parameter;

namespace Trident.Data.RestSharp
{
    /// <summary>
    /// Class RestSharpMapperProfile.
    /// Implements the <see cref="AutoMapper.Profile" />
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class RestSharpMapperProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpMapperProfile" /> class.
        /// </summary>
        public RestSharpMapperProfile()
        {
            ConfigureRestMappings();
        }

        /// <summary>
        /// Configures the rest mappings.
        /// </summary>
        private void ConfigureRestMappings()
        {
            //
            // RestSharp mappings
            //
            CreateMap<RestMethod, RestSharpMethod>();
            CreateMap<RestSharpMethod, RestMethod>();
            CreateMap<RestParameterType, RestSharpParameterType>();
            CreateMap<RestSharpParameterType, RestParameterType>();
            CreateMap<RestParameter, RestSharpRestParameter>();
            CreateMap<RestRequest, RestSharpRestRequest>()
                .ForMember(x => x.Method, opts => opts.MapFrom(x => x.Method))
                .ForMember(x => x.Resource, opts => opts.MapFrom(x => x.Path))
                .ForMember(x => x.RequestFormat, opts => opts.MapFrom(x=> DataFormat.Json))
                ;
            CreateMap<IRestRequest, RestRequest>()
                .ForMember(x => x.Method, opts => opts.MapFrom(x => x.Method))
                .ForMember(x => x.Path, ops => ops.MapFrom(x => x.Resource))
                ;

            CreateMap<IRestResponse, Rest.RestResponse>();
            CreateMap(typeof(IRestResponse<>), typeof(Rest.RestResponse<>));
        }
    }
}
