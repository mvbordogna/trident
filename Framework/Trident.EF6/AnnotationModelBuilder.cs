using Trident.EF6.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;

namespace Trident.EF6
{
    /// <summary>
    /// Provides an abstract implemenation of an assembly scanning IDbModelBuilder
    /// Implements the <see cref="Trident.EF6.Contracts.IDbModelBuilder" />
    /// </summary>
    /// <typeparam name="TEntity">The base Type that is the primary filter for entities to be registered with the EFDataContext</typeparam>
    /// <seealso cref="Trident.EF6.Contracts.IDbModelBuilder" />
    public abstract class AnnotationDirectiveModelBuilder<TEntity> : IDbModelBuilder
    {
        /// <summary>
        /// The pad lock
        /// </summary>
        private static object _padLock = new object();
        /// <summary>
        /// The compiled model
        /// </summary>
        private DbCompiledModel _compiledModel;
        /// <summary>
        /// The model assemblies
        /// </summary>
        private Assembly[] _modelAssemblies = null;

        /// <summary>
        /// Construct a new instance of the AnnotationDirectiveModelBuilder class assuming it will scan its Host assembly for Models to register with EF
        /// </summary>
        protected AnnotationDirectiveModelBuilder()
            : this(true) { }

        /// <summary>
        /// Construct a new instance of the AnnotationDirectiveModelBuilder class that will scan its Host assembly for Models to register with EF if specified
        /// and any of the model Assemblies specified. Assumes that the Entities to be register inherit from Trident.Domain.Entity
        /// </summary>
        /// <param name="scanSelfAssembly">Scans the Assmily in which the AnnotationDirectiveModelBuilder lives for models to register with EF</param>
        /// <param name="modelAssemblies">The model assemblies.</param>
        protected AnnotationDirectiveModelBuilder(bool scanSelfAssembly,  params Assembly[] modelAssemblies)
        {
            List<Assembly> assemblies = new List<Assembly>();

            if(modelAssemblies == null || scanSelfAssembly) {
                assemblies.Add(this.GetType().Assembly);
            }
            
            if(modelAssemblies != null)
            {
                assemblies.AddRange(modelAssemblies);
            }

            this._modelAssemblies = assemblies.ToArray();
        }

        /// <summary>
        /// Builds the model mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public void BuildModelMappings(DbModelBuilder modelBuilder)
        {
            var types = this._modelAssemblies.SelectMany(x => x.GetTypes());
            var mapTypes = from t in types
                           where
                               t.BaseType != null
                               && t.BaseType != typeof(object)
                               && typeof(TEntity).IsAssignableFrom(t)
                               && !t.IsAbstract
                           select t;

            foreach (var mapType in mapTypes)
            {
                var genericType = typeof(EntityTypeConfiguration<>).MakeGenericType(mapType);
                dynamic mapInstance = Activator.CreateInstance(genericType);
                modelBuilder.Configurations.Add(mapInstance);
            }

            OnModelBinding(modelBuilder);
        }


        /// <summary>
        /// Called when [model binding].
        /// </summary>
        /// <param name="modelbuilder">The modelbuilder.</param>
        public abstract void OnModelBinding(DbModelBuilder modelbuilder);

        /// <summary>
        /// Gets the compiled.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>DbCompiledModel.</returns>
        public DbCompiledModel GetCompiled(IDbConnection connection)
        {
            if (_compiledModel == null)
            {
                lock (_padLock)
                {
                    if (_compiledModel == null)
                    {
                        var builder = new DbModelBuilder();
                        this.BuildModelMappings(builder);
                        var dbModel = builder.Build(connection as DbConnection);
                        _compiledModel = dbModel.Compile();

                    }
                }

            }
            return _compiledModel;
        }

    }

}
