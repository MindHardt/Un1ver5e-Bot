using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Un1ver5e.ExtensionsMethods
{
    public static class ExtensionMethods
    {
        public static void Shuffle<T>(this T[] array)
        {
            array.OrderBy(e => Bot.Program.Randomizer.Next());
        }
    }
}
