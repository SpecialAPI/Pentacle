using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;

namespace Pentacle.Tools
{
    public static class TextTools
    {
        private static FieldInfo spriteAssetVersion = AccessTools.Field(typeof(TMP_SpriteAsset), "m_Version");

        public static void AddNewTextSprite(string name, Texture2D tex)
        {
            var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            var hashCode = TMP_TextUtilities.GetSimpleHashCode(name);

            spriteAsset.name = name;
            spriteAsset.hashCode = hashCode;
            spriteAssetVersion.SetValue(spriteAsset, "1.1.0");

            spriteAsset.spriteInfoList = [];

            spriteAsset.material = new(Shader.Find("TextMeshPro/Sprite"))
            {
                mainTexture = tex
            };
            spriteAsset.spriteSheet = tex;

            var glyph = new TMP_SpriteGlyph
            {
                index = 0,
                atlasIndex = 0,
                scale = 1f,
                glyphRect = new(0, 0, tex.width, tex.height),
                metrics = new(tex.width, tex.height, 0f, tex.height * 0.9f, tex.width)
            };

            var spriteChar = new TMP_SpriteCharacter
            {
                name = name,
                scale = 1f,
                textAsset = spriteAsset,
                glyph = glyph,
                glyphIndex = 0
            };

            spriteAsset.spriteCharacterTable.Add(spriteChar);
            spriteAsset.spriteGlyphTable.Add(glyph);

            MaterialReferenceManager.AddSpriteAsset(spriteAsset);
        }

        public static string ReplaceAndKeepCase(this string input, string orig, string replacement)
        {
            var pattern = $@"\b{orig}\b";
            var match = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
            var idxOffs = 0;

            var idxDiff = replacement.Length - orig.Length;

            foreach (Match m in match)
            {
                var rep = GetReplacementWithSameCase(m.Value, replacement);
                var part2Start = m.Index + m.Length + idxOffs;

                var before = input.Substring(0, m.Index + idxOffs);
                var after = part2Start >= input.Length ? "" : input.Substring(part2Start);

                input = $"{before}{rep}{after}";
                idxOffs += idxDiff;
            }

            return input;
        }

        public static string GetReplacementWithSameCase(string match, string replacement)
        {
            if (match == match.ToLowerInvariant())
                return replacement.ToLowerInvariant();

            if (match == match.ToUpperInvariant())
                return replacement.ToUpperInvariant();

            if (match == match.ToFirstWordTitleInvariant())
                return replacement.ToFirstWordTitleInvariant();

            return replacement;
        }

        public static string ToFirstWordTitleInvariant(this string input)
        {
            var sp = input.Split(' ');

            for (int i = 0; i < sp.Length; i++)
            {
                sp[i] = i == 0 ? sp[i].ToTitleInvariant() : sp[i].ToLowerInvariant();
            }

            return string.Join(" ", sp);
        }

        public static string ToTitleInvariant(this string input)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        public static string Colorize(this string txt, Color c)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>{txt}</color>";
        }

        public static string Italic(this string txt)
        {
            return $"<i>{txt}</i>";
        }

        public static string Scale(this string txt, int scalePercent)
        {
            return $"<size={scalePercent}%>{txt}</size>";
        }

        public static string Subscript(this string txt)
        {
            return $"<sub>{txt}</sub>";
        }

        public static string Superscript(this string txt)
        {
            return $"<sup>{txt}</sup>";
        }

        public static string Strikethrough(this string txt)
        {
            return $"<s>{txt}</s>";
        }

        public static string Underline(this string txt)
        {
            return $"<u>{txt}</u>";
        }

        public static string NoBreak(this string txt)
        {
            return $"<nobr>{txt}</nobr>";
        }

        public static string VerticalOffset(this string txt, float offsetUnits)
        {
            return $"<voffset={offsetUnits}em>{txt}</voffset>";
        }

        public static string Align(this string txt, TextAlignment alignment)
        {
            return $"<align=\"{alignment.ToString().ToLowerInvariant()}\">{txt}</align>";
        }
    }
}
