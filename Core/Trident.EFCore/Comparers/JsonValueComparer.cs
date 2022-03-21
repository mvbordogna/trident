using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Trident.EFCore.Json
{
    internal class JsonValueComparer<T> : ValueComparer<T>
    {
        public JsonValueComparer() : base(
            (t1, t2) => DoEquals(t1, t2),
            t => DoGetHashCode(t),
            t => DoGetSnapshot(t))
        { }

        private static string Json(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        private static T DoGetSnapshot(T instance)
        {

            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            var result = (T)JsonConvert.DeserializeObject(Json(instance), typeof(T));
            return result;

        }

        private static int DoGetHashCode(T instance)
        {

            if (instance is IEquatable<T>)
                return instance.GetHashCode();

            return Json(instance).GetHashCode();

        }

        private static bool DoEquals(T left, T right)
        {

            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            var result = Json(left).Equals(Json(right));
            return result;

        }
    }
}
