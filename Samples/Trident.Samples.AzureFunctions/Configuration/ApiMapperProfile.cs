using AutoMapper;
using System;
using System.Linq;
using Trident.Mapper;
using Trident.Samples.AzureFunctions.Models.Organization;
using Trident.Samples.Domain.Entities;

namespace Trident.Samples.AzureFunctions.Configuration
{
    public class ApiMapperProfile : Profile
    {
        public ApiMapperProfile()
        {

            //base search mappings
            this.ConfigureSearchMapping();
            this.ConfigureAllSupportedPrimitiveCollectionTypes();


            CreateMap<Organization, OrganizationEntity>().ReverseMap();
            CreateMap<Department, DepartmentEntity>().ReverseMap();


            // When mapping a collection property, if the source value is null AutoMapper will map the destination field to an empty collection rather than setting the
            // destination value to null. This aligns with the behavior of Entity Framework and Framework Design Guidelines that believe C# references, arrays, lists,
            // collections, dictionaries and IEnumerables should NEVER be null, ever.
            // This behavior can be changed by setting the AllowNullCollections property to true when configuring the mapper.
            // https://docs.automapper.org/en/stable/Lists-and-arrays.html


        }


    }
}
