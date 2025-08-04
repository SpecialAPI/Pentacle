using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides ScriptableObject-related tools.
    /// </summary>
    public static class ScriptableObjectTools
    {
        /// <summary>
        /// Creates a scriptable object of a certain type. This method can be given an action that will be immediately invoked on the created scriptable object, which can be used to immediately set the scriptable object's values.
        /// </summary>
        /// <typeparam name="T">The type for the created object. Must be a subclass of ScriptableObject.</typeparam>
        /// <param name="configure">An action that immediately gets invoked on the created scriptable object. This can be used to immediately set the scriptable object's values, similarly to how constructors work for regular objects./param>
        /// <returns>The created scriptable object.</returns>
        public static T CreateScriptable<T>(Action<T> configure = null) where T : ScriptableObject
        {
            var s = ScriptableObject.CreateInstance<T>();
            configure?.Invoke(s);

            return s;
        }
    }
}
