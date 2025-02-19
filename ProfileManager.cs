using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle
{
    public static class ProfileManager
    {
        private static readonly Dictionary<Assembly, ModProfile> Profiles = [];

        public static ModProfile RegisterMod(string guid, string prefix, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            if(Profiles.TryGetValue(assembly, out var prof))
                return prof;

            return Profiles[assembly] = new()
            {
                Assembly = assembly,

                Guid = guid,
                Prefix = prefix,
            };
        }

        internal static bool TryGetProfile(Assembly assembly, out ModProfile prof)
        {
            return Profiles.TryGetValue(assembly, out prof);
        }

        internal static ModProfile GetProfile(Assembly assembly)
        {
            if(TryGetProfile(assembly, out var prof))
                return prof;

            return null;
        }
    }

    public class ModProfile
    {
        internal Assembly Assembly;
        internal AssetBundle Bundle;
        internal string Guid = "";
        internal string Prefix = "";

        public void SetAssetBundle(AssetBundle bundle)
        {
            if (bundle == null)
                Debug.LogError($"ModProfile.SetAssetBundle called with a null asset bundle.");

            Bundle = bundle;
        }

        public void LoadAssetBundle(string name)
        {
            if (!AdvancedResourceLoader.TryReadFromResource(name, out var ba, Assembly))
            {
                Debug.LogError($"Couldn't load an asset bundle with the name \"{name}\" from embedded resources.");

                return;
            }

            Bundle = AssetBundle.LoadFromMemory(ba);
        }

        public string GetID(string original)
        {
            return $"{Prefix}_{original}";
        }
    }
}
