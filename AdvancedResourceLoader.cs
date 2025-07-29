using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle
{
    public static class AdvancedResourceLoader
    {
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

        public static void LoadFMODBankFromResource(string resname, bool loadSamples = false, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            if (!TryReadFromResource(resname.TryAddExtension("bank"), out var ba, assembly))
                return;

            LoadFMODBankFromBytes(ba, resname, loadSamples);
        }

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

        public static string TryAddExtension(this string n, string e)
        {
            if (n.EndsWith($".{e}"))
                return n;

            return n + $".{e}";
        }
    }
}
