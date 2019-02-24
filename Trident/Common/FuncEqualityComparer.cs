using System;
using System.Collections.Generic;

namespace Trident.Common
{
    /// <summary>
    /// Class FuncEqualityComparer provides the ability to use a function to do the compare between two object of the same type given a lambda expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{T}" />
    public class FuncEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// The comparer
        /// </summary>
        readonly Func<T, T, bool> _comparer;
        /// <summary>
        /// The hash
        /// </summary>
        readonly Func<T, int> _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncEqualityComparer{T}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public FuncEqualityComparer(Func<T, T, bool> comparer)
            : this(comparer, t => 0) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncEqualityComparer{T}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="hash">The hash.</param>
        public FuncEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
        {
            _comparer = comparer;
            _hash = hash;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(T obj)
        {
            return _hash(obj);
        }
    }
}
