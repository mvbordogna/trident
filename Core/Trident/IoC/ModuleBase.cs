using System.Reflection;

namespace Trident.IoC
{
    public abstract class ModuleBase : IModule
    {
         public virtual void Configure(IIoCProvider builder) {
            RegisterDefaultAssemblyScans(builder);
        }

        public virtual Assembly[] TargetAssemblies => new Assembly[] { this.ModuleHostAssembly };

        protected Assembly ModuleHostAssembly => this.GetType().Assembly;

        protected void RegisterDefaultAssemblyScans(IIoCProvider builder)
        {
            builder.UsingTridentFactories(this.TargetAssemblies);
            builder.UsingTridentManagers(this.TargetAssemblies);
            builder.UsingTridentMapperProfiles(this.TargetAssemblies);
            builder.UsingTridentProviders(this.TargetAssemblies);
            builder.UsingTridentRepositories(this.TargetAssemblies);

            builder.UsingTridentSearch(this.TargetAssemblies);
            builder.UsingTridentTransactions();
            builder.UsingTridentValidationManagers(this.TargetAssemblies);
            builder.UsingTridentValidationRules(this.TargetAssemblies);
            builder.UsingTridentWorkflowManagers(this.TargetAssemblies);
            builder.UsingTridentWorkflowTasks(this.TargetAssemblies);
            builder.UsingTridentResolvers(this.TargetAssemblies);           

        }
    }
}