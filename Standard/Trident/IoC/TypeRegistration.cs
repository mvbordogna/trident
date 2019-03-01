using System;
using System.Collections.Generic;

namespace Trident.IoC
{

    public interface ITypeRegistration
    {
         Type InstanceType { get; }
         Type InferanceType { get; }
         LifeSpan InstanceLifeSpan { get; set; } 
    }

    public class TypeRegistration<T, I> : ITypeRegistration
    {

        public TypeRegistration(LifeSpan lifeSpan = LifeSpan.InstancePerLifetimeScope)
        {
            this.InstanceLifeSpan = lifeSpan;
        }

        public Type InstanceType { get { return typeof(T); } }
        public Type InferanceType { get { return typeof(I);  } }
        public LifeSpan InstanceLifeSpan { get; set; } = LifeSpan.InstancePerLifetimeScope;
    }
    
    public enum LifeSpan
    {
        InstancePerLifetimeScope = 0,
        SingleInstance = 1,
        NewInstancePerRequest = 2
    }
    
    public abstract class TypeRegistrationModule : IRegistrationModule
    {
        public abstract List<ITypeRegistration> GetRegistrations();

        public abstract List<ITypeRegistration> GetScanRegistrations();
    }
}
