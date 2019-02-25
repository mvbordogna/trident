using Autofac.Core;
using Autofac.Extras.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Testing.TestScopes
{
    public static class AutoMockHelpers
    {
        public static T CreateAutoMockedTestScope<T>(IDictionary<Type, object> instances = null) where T : IAutoMockTestScope
        {
            var mocker = AutoMock.GetLoose();
            var mockerMethod = mocker.GetType().GetMethod("Mock");
            var mockType = typeof(Mock);
            var type = typeof(T);
            var properties = type.GetProperties();
            var publicProperties = properties.Where(t => mockType.IsAssignableFrom(t.PropertyType) && t.CanWrite);
            var instance = mocker.Create<T>();
            

            foreach (var i in instances)
            {
                var mockerType = mocker.GetType();
                
                var provideMethod = mockerType.GetMethods()
                    .First(t => t.Name == "Provide" && t.IsGenericMethod && t.GetGenericArguments().Count() == 1
                    );
                var methodInstance = provideMethod.MakeGenericMethod(i.Key);
                var propInstance = methodInstance.Invoke(mocker, new[] { i.Value });
            }

            var arg = new object[] { new Parameter[] { } };
            foreach (var property in publicProperties)
            {
                var args = property.PropertyType.GetGenericArguments();
                var methodInstance = mockerMethod.MakeGenericMethod(args);
                var propInstance = methodInstance.Invoke(mocker, arg);
                property.SetValue(instance, propInstance);
            }
            var instanceUnderTestProperty = type.GetProperty("InstanceUnderTest");
            var creatorMethod = mocker.GetType().GetMethod("Create");
            var testMethodInstance = creatorMethod.MakeGenericMethod(instanceUnderTestProperty.PropertyType);
            var testPropInstance = testMethodInstance.Invoke(mocker, arg);
            instanceUnderTestProperty.SetValue(instance, testPropInstance);
            instance.Initialize();
            instance.ResolverInstance = mocker;
            return instance;
        }
            
    }
}
