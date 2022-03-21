using AutoMapper;
using System;
using System.Linq;

namespace Trident.Mapper
{
    /// <summary>
    /// Class DynamicMapperRegistry. This class cannot be inherited.
    /// Implements the <see cref="TridentOptionsBuilder.Mapper.AutoMapperRegistry" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Mapper.AutoMapperRegistry" />
    public sealed class DynamicMapperRegistry : AutoMapperRegistry
    {
        /// <summary>
        /// The initialize mapper
        /// </summary>
        private static readonly Func<Profile[], MapperConfiguration> InitializeMapperProfileInstances = (Profile[] profiles) =>
        {
            var configuration = new MapperConfiguration(cfg => {               
                if (profiles != null)
                {
                    profiles.ToList().ForEach(x => cfg.AddProfile(x));
                }
            });     
               
            return configuration;
        };

        /// <summary>
        /// The initialize mapper
        /// </summary>
        private static readonly Func<Type[], MapperConfiguration> InitializeMapperProfileTypes = (Type[] profileTypes) =>
        {
            var configuration = new MapperConfiguration(cfg => {               
                if (profileTypes != null)
                {
                    profileTypes.ToList().ForEach(x => cfg.AddProfile(x));
                }
            });

            return configuration;
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMapperRegistry" /> class.
        /// NOTE:
        /// </summary>
        /// <remarks>This class cannot be used as a singleton due to mapping of the same object combination with
        /// ordinal based constructs such as IDataReader and DataRow, could vary from Repository to Repository.</remarks>
        /// <param name="profiles">The profiles.</param>
        public DynamicMapperRegistry(params Profile[] profiles)
            : base(InitializeMapperProfileInstances(profiles))
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMapperRegistry" /> class.     
        /// </summary>
        /// <remarks>This class cannot be used as a singleton due to mapping of the same object combination with
        /// ordinal based constructs such as IDataReader and DataRow, could vary from Repository to Repository.</remarks>       
        /// <param name="profileTypes">The profile types.</param>
        public DynamicMapperRegistry(params Type[] profileTypes)
            : base(InitializeMapperProfileTypes(profileTypes))
        {
        }
    }
}