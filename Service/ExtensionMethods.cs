using System;
using System.Collections.Generic;
using System.Linq;

namespace Un1ver5e.Service
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Shuffles the IEnumerable, making it random order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The enumerable to shuffle.</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection) => collection.OrderBy(e => Generals.Random.Next());

        /// <summary>
        /// Shuffles the IEnumerable, making it random order using specific Random object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The enumerable to shuffle.</param>
        /// <param name="randomizer">Random object.</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, Random randomizer) => collection.OrderBy(e => randomizer.Next());

        /// <summary>
        /// Fills IEnumerable according to a specific T builder.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="source"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEnumerable<T> FillWith<T>(this IEnumerable<T> source, Func<T> builder)
        {
            T[] mdfSource = source.ToArray();
            for (int i = 0; i < mdfSource.Length; i++)
            {
                mdfSource[i] = builder();
            }
            return mdfSource;
        }

        /// <summary>
        /// Formats the string according to Discord's formatting pattern.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <returns></returns>
        public static string FormatAs(this string input, params TextFormat[] format)
        {
            input = format.Contains(TextFormat.Cursive)       ? "*"   + input + "*"   : input;
            input = format.Contains(TextFormat.Bold)          ? "**"  + input + "**"  : input;
            input = format.Contains(TextFormat.Underlined)    ? "_"   + input + "_"   : input;
            input = format.Contains(TextFormat.Strikethrough) ? "~~"  + input + "~~"  : input;
            input = format.Contains(TextFormat.Spoiler)       ? "||"  + input + "||"  : input;
            input = format.Contains(TextFormat.CodeBlock)     ? "```" + input + "```" : input;
            return input;
        }

        /// <summary>
        /// Formats the string according to Discord's formatting pattern.
        /// </summary>
        /// <param name="input">The original string.</param>
        /// <param name="formatCode">The byte code for formatting. Must lie int 0..63. Defaults for bold codeblock.</param>
        /// <returns></returns>
        public static string FormatAs(this string input, byte formatCode = 34)
        {
            if (formatCode >= 64) throw new Exception("Invalid formatCode!");
            if (formatCode >= 32)
            {
                formatCode -= 32;
                input = "```" + input + "```";
            }
            if (formatCode >= 16)
            {
                formatCode -= 16;
                input = "||" + input + "||";
            }
            if (formatCode >= 8)
            {
                formatCode -= 8;
                input = "~~" + input + "~~";
            }
            if (formatCode >= 4)
            {
                formatCode -= 4;
                input = "_" + input + "_";
            }
            if (formatCode >= 2)
            {
                formatCode -= 2;
                input = "**" + input + "**";
            }
            if (formatCode >= 1)
            {
                formatCode -= 1;
                input = "*" + input + "*";
            }
            return input;
        }

        public enum TextFormat
        {
            Cursive = 1,
            Bold = 2,
            Underlined = 4,
            Strikethrough = 8,
            Spoiler = 16,
            CodeBlock = 32
        }
    }
}