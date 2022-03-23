namespace Trident.IoC
{
    public abstract class TridentModule : IoCModule
    {
        protected override void RegisterDefaultAssemblyScans(IIoCProvider builder)
        {
            builder.UsingTridentFactories(this.TargetAssemblies);
            builder.UsingTridentManagers(this.TargetAssemblies);
            //builder.UsingTridentMapperProfiles(this.TargetAssemblies);
            builder.UsingTridentProviders(this.TargetAssemblies);
            builder.UsingTridentRepositories(this.TargetAssemblies);
            builder.UsingTridentSearch(this.TargetAssemblies);
            builder.UsingTridentValidationManagers(this.TargetAssemblies);
            builder.UsingTridentValidationRules(this.TargetAssemblies);
            builder.UsingTridentWorkflowManagers(this.TargetAssemblies);
            builder.UsingTridentWorkflowTasks(this.TargetAssemblies);
            builder.UsingTridentResolvers(this.TargetAssemblies);
            //builder.UsingTridentCoreConfiguration(this.TargetAssemblies);
        }

    }
}
