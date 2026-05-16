using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownOfUsMiraJK.Utilities
{
    public static class JKMiscUtils
    {
        public static void Shuffle<T>(this List<T> list, ref Random random)
        {
            for (var i = list.Count - 1; i > 0; --i)
            {
                var j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
