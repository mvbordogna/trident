using Trident.Mapper;
using System;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using System.Collections.Generic;

namespace Trident.Mapper
{

    /// <summary>
    /// Class ServiceMapperRegistry.
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Mapper.AutoMapperRegistry" />
    /// <seealso cref="TridentOptionsBuilder.Mapper.IServiceMapperRegistry" />
    /// <remarks>This is the main entry point for all AutoMapping type maps.
    /// This class is registered as a singleton in DI.  DI finds all AutoMapper Profiles
    /// in various assemblies and injects them here, they are then registered with AutoMapper.</remarks>
    public class ServiceMapperRegistry : AutoMapperRegistry, IServiceMapperRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMapperRegistry" /> class.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        public ServiceMapperRegistry(IEnumerable<Profile> profiles)
            : this(profiles?.ToArray())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMapperRegistry" /> class.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        protected ServiceMapperRegistry(params Profile[] profiles)
            : base(new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();

                if (profiles != null)
                {
                    profiles.ToList().ForEach(x => cfg.AddProfile(x));
                }

              
            }))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMapperRegistry" /> class.
        /// </summary>
        /// <param name="profileTypes">The profile types.</param>
        protected ServiceMapperRegistry(params Type[] profileTypes)
            : base(new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();
                if (profileTypes != null)
                {
                    profileTypes.ToList().ForEach(x => cfg.AddProfile(x));
                }
            }))
        { }

    }
}

