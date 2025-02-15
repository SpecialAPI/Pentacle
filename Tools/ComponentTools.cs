using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class ComponentTools
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var exists = go.GetComponent<T>();

            if (exists != null)
                return exists;

            return go.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component comp) where T : Component
        {
            return comp.gameObject.GetOrAddComponent<T>();
        }

        public static T AddComponent<T>(this Component comp) where T : Component
        {
            return comp.gameObject.AddComponent<T>();
        }
    }
}
