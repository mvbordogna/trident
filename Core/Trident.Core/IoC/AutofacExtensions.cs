using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Trident.Common;

namespace Trident.IoC
{
    public static class AutofacExtensions
    {
        public static void UsingTridentEntityComparer(this ContainerBuilder builder)
        {
            builder.RegisterType<EntityComparer>().As<IEntityComparer>().SingleInstance();
        }


    }
}
