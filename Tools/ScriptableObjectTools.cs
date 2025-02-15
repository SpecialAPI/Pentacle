using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// ScriptableObject-related extensions.
    /// </summary>
    public static class ScriptableObjectTools
    {
        /// <summary>
        /// Creates a scriptable object of the given type. Can be given an action to immediately configure the created object.
        /// </summary>
        /// <typeparam name="T">The scriptable object's type.</typeparam>
        /// <param name="configure">An action that immediately gets called on the created object to configure it.</param>
        /// <returns>The created object.</returns>
        public static T CreateScriptable<T>(Action<T> configure = null) where T : ScriptableObject
        {
            var s = ScriptableObject.CreateInstance<T>();
            configure?.Invoke(s);

            return s;
        }
    }
}
