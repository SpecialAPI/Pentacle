using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides component-related tools and extension methods.
    /// </summary>
    public static class ComponentTools
    {
        /// <summary>
        /// Gets a component from a GameObject, or adds it if the GameObject doesn't have it.
        /// </summary>
        /// <typeparam name="T">The type of component to get or add.</typeparam>
        /// <param name="go">The GameObject to get from or add the component to.</param>
        /// <returns>The existing or added component.</returns>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var exists = go.GetComponent<T>();

            if (exists != null)
                return exists;

            return go.AddComponent<T>();
        }

        /// <summary>
        /// Gets a component from a component's GameObject, or adds it if the GameObject doesn't have it.
        /// </summary>
        /// <typeparam name="T">The type of component to get or add.</typeparam>
        /// <param name="comp">The component to get from or add the component to.</param>
        /// <returns>The existing or added component.</returns>
        public static T GetOrAddComponent<T>(this Component comp) where T : Component
        {
            return comp.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// Adds a new component to a component's GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to add.</typeparam>
        /// <param name="comp">The component to add the component to.</param>
        /// <returns>The added component.</returns>
        public static T AddComponent<T>(this Component comp) where T : Component
        {
            return comp.gameObject.AddComponent<T>();
        }
    }
}
