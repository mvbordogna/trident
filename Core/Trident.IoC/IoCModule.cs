using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Trident.IoC
{
    public abstract class IoCModule: IIoCModule
    {

        public virtual void Configure(IIoCProvider builder) {
            RegisterDefaultAssemblyScans(builder);
        }

        public virtual Assembly[] TargetAssemblies => new Assembly[] { this.ModuleHostAssembly };

        protected Assembly ModuleHostAssembly => this.GetType().Assembly;

        protected virtual void RegisterDefaultAssemblyScans(IIoCProvider builder)
        {
            builder.UsingTridentFactories(this.TargetAssemblies);
            builder.UsingTridentRepositories(this.TargetAssemblies);
            builder.UsingTridentManagers(this.TargetAssemblies);
            builder.UsingTridentProviders(this.TargetAssemblies);
            builder.UsingTridentResolvers(this.TargetAssemblies);
        }
    }                 
    
}
