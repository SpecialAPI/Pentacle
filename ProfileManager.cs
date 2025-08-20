using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle
{
    /// <summary>
    /// Static class that provides tools for managing Pentacle mod profiles.
    /// </summary>
    public static class ProfileManager
    {
        private static readonly Dictionary<Assembly, ModProfile> Profiles = [];

        /// <summary>
        /// Creates a new mod profile and connects it to an assembly.
        /// </summary>
        /// <param name="guid">The GUID of the mod registering the profile.</param>
        /// <param name="prefix">The mod prefix that will be used by the profile.</param>
        /// <param name="assembly">The assembly that will be used by the profile, and also the assembly that the profile will be connected to. Defaults to the calling assembly if null.</param>
        /// <returns>The created mod profile.</returns>
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

        internal static bool EnsureProfileExists(ModProfile profile)
        {
            if (profile != null)
                return true;

            PentacleLogger.LogError($"Profile doesn't exist! Use ProfileManager.RegisterMod to add a Pentacle profile for your mod.\n{Environment.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// A class that stores information about a mod using Pentacle. Mod profiles can be created and managed by ProfileManager.
    /// </summary>
    public class ModProfile
    {
        internal Assembly Assembly;
        internal AssetBundle Bundle;
        internal string Guid = "";
        internal string Prefix = "";
        internal Dictionary<Type, Dictionary<string, AbilitySO>> abilityReferences = [];

        /// <summary>
        /// Assigns an asset bundle to this profile.
        /// </summary>
        /// <param name="bundle">The asset bundle to assign to this profile.</param>
        public void SetAssetBundle(AssetBundle bundle)
        {
            if (bundle == null)
                PentacleLogger.LogError($"{Guid}: ModProfile.SetAssetBundle called with a null asset bundle.");

            Bundle = bundle;
        }

        /// <summary>
        /// Loads an asset bundle stored in the embedded resources of this profile's assembly and assigns it to this profile.
        /// </summary>
        /// <param name="name">The file name for the asset bundle.</param>
        public void LoadAssetBundle(string name)
        {
            if (!AdvancedResourceLoader.TryReadFromResource(name, out var ba, Assembly))
            {
                PentacleLogger.LogError($"{Guid}: Couldn't load an asset bundle with the name \"{name}\" from embedded resources.");

                return;
            }

            Bundle = AssetBundle.LoadFromMemory(ba);
        }

        /// <summary>
        /// Converts a basic ID to a modded ID by adding this profile's prefix to it. The format for modded IDs is ProfilePrefix_BasicID.
        /// </summary>
        /// <param name="original">The basic ID to convert.</param>
        /// <returns>The modded ID for the input basic ID.</returns>
        public string GetID(string original)
        {
            return $"{Prefix}_{original}";
        }

        /// <summary>
        /// Loads a Unity Sprite object from a PNG image stored in the embedded resources of this profile's assembly. Returns null if the image couldn't be loaded.
        /// </summary>
        /// <param name="name">The file name for the image. The .png extension is optional.</param>
        /// <param name="pivot">The pivot point for the created sprite. Defaults to the center (0.5, 0.5) if null.</param>
        /// <param name="pixelsPerUnit">The pixels per unit value for the created sprite.</param>
        /// <returns>The loaded Sprite object.</returns>
        public Sprite LoadSprite(string name, Vector2? pivot = null, int pixelsPerUnit = 32)
        {
            return AdvancedResourceLoader.LoadSprite(name, pivot, pixelsPerUnit, Assembly);
        }
    }
}
