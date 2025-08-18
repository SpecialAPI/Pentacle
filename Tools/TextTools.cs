using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides text-related tools and extensions.
    /// </summary>
    public static class TextTools
    {
        private static FieldInfo spriteAssetVersion = AccessTools.Field(typeof(TMP_SpriteAsset), "m_Version");

        // TODO: remove/disable for the release
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

        /// <summary>
        /// Replaces all instances of a string (case-insensitive) in the input string with another string, attempting to retain the capitalization of the original string.
        /// <para>More specifically, it will try to mach these capitalization styles in this order:</para>
        /// <list type="bullet">
        /// <item>Lowercase</item>
        /// <item>Uppercase</item>
        /// <item>First-word title case (see <see cref="ToFirstWordTitleInvariant(string)"/>)</item>
        /// </list>
        /// If one of these capitalization styles is detected, the same capitalization style will be applied to the replacement. If an instance of the original string doesn't match any of these capitalization styles, the capitalization of the replacement string will not be modified.
        /// </summary>
        /// <param name="input">The string to do the replacements on.</param>
        /// <param name="orig">The string that should be replaced.</param>
        /// <param name="replacement">The replacement for the original string.</param>
        /// <returns>The input string with all instances of the original string replaced with the replacement string.</returns>
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

        // TODO: make private
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

        /// <summary>
        /// Converts the first word of a string to title case and all other words to lowercase. This uses the text rules of the invariant culture.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>The invariant first word title case equivalent of the input string.</returns>
        public static string ToFirstWordTitleInvariant(this string input)
        {
            var sp = input.Split(' ');

            for (int i = 0; i < sp.Length; i++)
            {
                sp[i] = i == 0 ? sp[i].ToTitleInvariant() : sp[i].ToLowerInvariant();
            }

            return string.Join(" ", sp);
        }

        /// <summary>
        /// Converts a string to title case using the text rules of the invariant culture.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>The invariant title case equivalent of the input string.</returns>
        public static string ToTitleInvariant(this string input)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        /// <summary>
        /// Applies the <![CDATA[<color>]]> TextMeshPro tag to a string, changing its displayed color.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <param name="c">The color that the string should be displayed as.</param>
        /// <returns>The input string with the <![CDATA[<color>]]> TextMeshPro tag applied to it.</returns>
        public static string Colorize(this string txt, Color c)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>{txt}</color>";
        }

        /// <summary>
        /// Applies the <![CDATA[<i>]]> TextMeshPro tag to a string, making it be displayed as italic.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<i>]]> TextMeshPro tag applied to it.</returns>
        public static string Italic(this string txt)
        {
            return $"<i>{txt}</i>";
        }

        /// <summary>
        /// Applies the <![CDATA[<size>]]> TextMeshPro tag to a string, changing its displayed size.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <param name="scalePercent">The size percentage for the string. For example, 200 would double the text's size and 50 would halve it.</param>
        /// <returns>The input string with the <![CDATA[<size>]]> TextMeshPro tag applied to it.</returns>
        public static string Scale(this string txt, int scalePercent)
        {
            return $"<size={scalePercent}%>{txt}</size>";
        }

        /// <summary>
        /// Applies the <![CDATA[<sub>]]> TextMeshPro tag to a string, making it be displayed as subscript.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<sub>]]> TextMeshPro tag applied to it.</returns>
        public static string Subscript(this string txt)
        {
            return $"<sub>{txt}</sub>";
        }

        /// <summary>
        /// Applies the <![CDATA[<sup>]]> TextMeshPro tag to a string, making it be displayed as superscript.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<sup>]]> TextMeshPro tag applied to it.</returns>
        public static string Superscript(this string txt)
        {
            return $"<sup>{txt}</sup>";
        }

        /// <summary>
        /// Applies the <![CDATA[<s>]]> TextMeshPro tag to a string, making it be displayed with a strikethrough line.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<s>]]> TextMeshPro tag applied to it.</returns>
        public static string Strikethrough(this string txt)
        {
            return $"<s>{txt}</s>";
        }

        /// <summary>
        /// Applies the <![CDATA[<u>]]> TextMeshPro tag to a string, making it be displayed with an underline.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<u>]]> TextMeshPro tag applied to it.</returns>
        public static string Underline(this string txt)
        {
            return $"<u>{txt}</u>";
        }

        /// <summary>
        /// Applies the <![CDATA[<nobr>]]> TextMeshPro tag to a string, making words in that string not be separated by word wrapping.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <returns>The input string with the <![CDATA[<nobr>]]> TextMeshPro tag applied to it.</returns>
        public static string NoBreak(this string txt)
        {
            return $"<nobr>{txt}</nobr>";
        }

        /// <summary>
        /// Applies the <![CDATA[<voffset>]]> TextMeshPro tag to a string, making it be displayed vertically offset.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <param name="offsetUnits">The vertical offsets for the string, measured in font units.</param>
        /// <returns>The input string with the <![CDATA[<voffset>]]> TextMeshPro tag applied to it.</returns>
        public static string VerticalOffset(this string txt, float offsetUnits)
        {
            return $"<voffset={offsetUnits}em>{txt}</voffset>";
        }

        /// <summary>
        /// Applies the <![CDATA[<align>]]> TextMeshPro tag to a string, making it be displayed with a certain text alignment.
        /// </summary>
        /// <param name="txt">The string to apply the tag to.</param>
        /// <param name="alignment">The text alignment type for the string.</param>
        /// <returns>The input string with the <![CDATA[<align>]]> TextMeshPro tag applied to it.</returns>
        public static string Align(this string txt, TextAlignment alignment)
        {
            return $"<align=\"{alignment.ToString().ToLowerInvariant()}\">{txt}</align>";
        }
    }
}
