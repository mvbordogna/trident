using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trident.Domain
{
    public class SafeNullable<T>
        where T : struct
    {


        public static implicit operator SafeNullable<T>(T b) => new SafeNullable<T>(b);
        public static implicit operator SafeNullable<T>(T? b) => new SafeNullable<T>(b);
        public static implicit operator T(SafeNullable<T> d) => d.Value;
        public static implicit operator T?(SafeNullable<T> d) => d.Value;

        #region SafeNUllable<T> to SafeNullable<T> InEquality Overloads

        public static bool operator >(SafeNullable<T> obj1, SafeNullable<T> obj2)
        {
            return Compare(obj1, obj2) > 0;
        }

        public static bool operator <(SafeNullable<T> obj1, SafeNullable<T> obj2)
        {
            return Compare(obj1, obj2) < 0;
        }

        public static bool operator >=(SafeNullable<T> obj1, SafeNullable<T> obj2)
        {
            return Compare(obj1, obj2) >= 0;
        }

        public static bool operator <=(SafeNullable<T> obj1, SafeNullable<T> obj2)
        {
            return Compare(obj1, obj2) <= 0;
        }

        #endregion


        #region SafeNullable<T> to T Inequality Overloads

        public static bool operator >(SafeNullable<T> obj1, T obj2)
        {
            return Compare(obj1, obj2) > 0;
        }

        public static bool operator <(SafeNullable<T> obj1, T obj2)
        {
            return Compare(obj1, obj2) < 0;
        }

        public static bool operator >(T obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) < 0;
        }

        public static bool operator <(T obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) > 0;
        }

        public static bool operator >=(SafeNullable<T> obj1, T obj2)
        {
            return Compare(obj1, obj2) >= 0;
        }

        public static bool operator <=(SafeNullable<T> obj1, T obj2)
        {
            return Compare(obj1, obj2) <= 0;
        }

        public static bool operator >=(T obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) <= 0;
        }

        public static bool operator <=(T obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) >= 0;
        }

        #endregion


        #region Safenullable<T> to Nullable<T> Inequality Overloads

        public static bool operator >(SafeNullable<T> obj1, T? obj2)
        {
            return Compare(obj1, obj2) > 0;
        }

        public static bool operator <(SafeNullable<T> obj1, T? obj2)
        {
            return Compare(obj1, obj2) < 0;
        }

        public static bool operator >(T? obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) < 0;
        }

        public static bool operator <(T? obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) > 0;
        }


        public static bool operator >=(SafeNullable<T> obj1, T? obj2)
        {
            return Compare(obj1, obj2) >= 0;
        }

        public static bool operator <=(SafeNullable<T> obj1, T? obj2)
        {
            return Compare(obj1, obj2) <= 0;
        }

        public static bool operator >=(T? obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) <= 0;
        }

        public static bool operator <=(T? obj2, SafeNullable<T> obj1)
        {
            return Compare(obj1, obj2) >= 0;
        }

        #endregion



        private static int Compare<X>(SafeNullable<X> n1, SafeNullable<X> n2) where X : struct
        {
            if (n1.HasValue)
            {
                if (n2.HasValue) return Comparer<X>.Default.Compare(n1.Value, n2.Value);
                return 1;
            }
            if (n2.HasValue) return -1;
            return 0;
        }

        private static int Compare<X>(SafeNullable<X> n1, X n2) where X : struct
        {
            if (n1.HasValue)
            {
                return Comparer<X>.Default.Compare(n1.Value, n2);

            }
            return -1;
        }


        private static int Compare<X>(SafeNullable<X> n1, X? n2) where X : struct
        {
            if (n1.HasValue)
            {
                if (n2.HasValue) return Comparer<X>.Default.Compare(n1.Value, n2.Value);
                return 1;
            }
            if (n2.HasValue) return -1;
            return 0;
        }




        public static bool operator ==(SafeNullable<T> obj1, T obj2)
        {
            #pragma warning disable CA2013 // Do not use ReferenceEquals with value types
            if (ReferenceEquals(obj1, obj2))

            {
                return true;
            }
            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Equals(obj2);
            #pragma warning restore CA2013 // Do not use ReferenceEquals with value types
        }

        public static bool operator !=(SafeNullable<T> obj1, T obj2)
        {
            #pragma warning disable CA2013 // Do not use ReferenceEquals with value types
            if (ReferenceEquals(obj1, obj2))

            {
                return false;
            }
            if (ReferenceEquals(obj1, null))
            {
                return true;
            }
            if (ReferenceEquals(obj2, null))
            {
                return true;
            }

            return !obj1.Equals(obj2);  
            #pragma warning restore CA2013 // Do not use ReferenceEquals with value types
        }

        public override bool Equals(object obj)
        {
            if (obj is T tObj)
            {
                return EqualityComparer<T>.Default.Equals(this.Value, tObj);
            }
            else if (typeof(T?).IsAssignableFrom(obj.GetType()))
            {
                return EqualityComparer<T>.Default.Equals(this.Value, ((T?)obj).GetValueOrDefault());
            }

            return base.Equals(obj as SafeNullable<T>);
        }


        private T? _safeValue;

        public SafeNullable() { }

        public SafeNullable(Nullable<T> val)
        {
            this.Value = val.HasValue ? val.Value : default(T);
        }

        public SafeNullable(T val)
        {
            this.Value = val;
        }


        public bool HasValue
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public T Value
        {
            get => (Nullable.HasValue) ? Nullable.Value : default(T);
            set
            {
                if (!value.Equals(default(T)))
                {
                    Nullable = value;
                    this.HasValue = true;
                }
                else
                {
                    HasValue = false;
                }
            }
        }

        [NotMapped]
        public Nullable<T> Nullable
        {
            get => _safeValue;
            set
            {
                _safeValue = value;
                this.HasValue = _safeValue.HasValue;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
