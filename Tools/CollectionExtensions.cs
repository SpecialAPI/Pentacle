using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides extension methods for IEnumerable, ICollection and IList.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a random element from the given IList.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IList.</typeparam>
        /// <param name="list">The IList (list or array) to get the random element from.</param>
        /// <returns>A random element from the IList.</returns>
        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}
