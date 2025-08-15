using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle
{
    /// <summary>
    /// Static class that provides tools for loading various assets from embedded resources.
    /// </summary>
    public static class AdvancedResourceLoader
    {
        /// <summary>
        /// Loads a Unity Sprite object from a PNG image stored in embedded resources. Returns null if the image couldn't be loaded.
        /// </summary>
        /// <param name="name">The file name for the image. The .png extension is optional.</param>
        /// <param name="pivot">The pivot point for the created sprite. Defaults to the center (0.5, 0.5) if null.</param>
        /// <param name="pixelsperunit">The pixels per unit value for the created sprite.</param>
        /// <param name="assembly">The assembly to load the image from. Defaults to the calling assembly if null.</param>
        /// <returns>The loaded Sprite object.</returns>
        public static Sprite LoadSprite(string name, Vector2? pivot = null, int pixelsperunit = 32, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var tex = LoadTexture(name, assembly);

            if (tex == null)
                return null;

            var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot ?? new Vector2(0.5f, 0.5f), pixelsperunit);
            sprite.name = tex.name;
            return sprite;
        }

        /// <summary>
        /// Loads a Unity Texture2D from a PNG image stored in embedded resources. Returns null if the image couldn't be loaded.
        /// </summary>
        /// <param name="name">The file name for the image. The .png extension is optional.</param>
        /// <param name="assembly">The assembly to load the image from. Defaults to the calling assembly if null.</param>
        /// <returns>The loaded Texture2D object.</returns>
        public static Texture2D LoadTexture(string name, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            if (!TryReadFromResource(name.TryAddExtension("png"), out var ba, assembly))
                return null;

            var tex = new Texture2D(1, 1);

            tex.LoadImage(ba);
            tex.filterMode = FilterMode.Point;
            tex.name = name;

            return tex;
        }

        /// <summary>
        /// Tries to load an array of bytes from a file stored in embedded resources.
        /// </summary>
        /// <param name="resname">The name of the file with the extension.</param>
        /// <param name="ba">If the file was successfully loaded, outputs the bytes stored in the file. Otherwise, outputs an empty array.</param>
        /// <param name="assembly">The assembly to load the file from. Defaults to the calling assembly if null.</param>
        /// <returns>True if the file was successfully loaded, false otherwise.</returns>
        public static bool TryReadFromResource(string resname, out byte[] ba, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var name = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith($".{resname}") || x == resname);

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"Couldn't load from resource name {resname}, returning an empty byte array.");
                ba = [];

                return false;
            }

            using var strem = assembly.GetManifestResourceStream(name);

            ba = new byte[strem.Length];
            strem.Read(ba, 0, ba.Length);

            return true;
        }

        /// <summary>
        /// Loads an FMOD soundbank from a .bank file stored in embedded resources.
        /// </summary>
        /// <param name="resname">The file name for the soundbank file. The .bank extension is optional.</param>
        /// <param name="loadSamples">If true, this will also load the soundbank's sample data.</param>
        /// <param name="assembly">The assembly to load the soundbank from. Defaults to the calling assembly if null.</param>
        public static void LoadFMODBankFromResource(string resname, bool loadSamples = false, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            if (!TryReadFromResource(resname.TryAddExtension("bank"), out var ba, assembly))
                return;

            LoadFMODBankFromBytes(ba, resname, loadSamples);
        }

        /// <summary>
        /// Loads an FMOD soundbank from the bytes stored in a .bank file.
        /// </summary>
        /// <param name="ba">The array of bytes that stores the soudbank's information.</param>
        /// <param name="bankName">The name of the soundbank.</param>
        /// <param name="loadSamples">If true, this will also load the soundbank's sample data.</param>
        /// <exception cref="BankLoadException">Thrown if the soundbank couldn't be loaded successfully.</exception>
        public static void LoadFMODBankFromBytes(byte[] ba, string bankName, bool loadSamples = false)
        {
            if (RuntimeManager.Instance.loadedBanks.TryGetValue(bankName, out var bnk))
            {
                bnk.RefCount++;

                if (loadSamples)
                    bnk.Bank.loadSampleData();

                return;
            }

            var bn = default(RuntimeManager.LoadedBank);
            var res = RuntimeManager.Instance.studioSystem.loadBankMemory(ba, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out bn.Bank);

            if (res == FMOD.RESULT.OK)
            {
                bn.RefCount = 1;
                RuntimeManager.Instance.loadedBanks.Add(bankName, bn);

                if (loadSamples)
                    bn.Bank.loadSampleData();

                return;
            }

            else if (res == FMOD.RESULT.ERR_ALREADY_LOCKED)
            {
                bn.RefCount = 2;
                RuntimeManager.Instance.loadedBanks.Add(bankName, bn);

                return;
            }

            throw new BankLoadException(bankName, res);
        }

        private static string TryAddExtension(this string n, string e)
        {
            if (n.EndsWith($".{e}"))
                return n;

            return n + $".{e}";
        }
    }
}
