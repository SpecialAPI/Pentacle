using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class CollectionExtensions
    {
        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}
