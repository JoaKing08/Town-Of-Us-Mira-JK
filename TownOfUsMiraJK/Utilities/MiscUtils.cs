namespace TownOfUsMiraJK.Utilities
{
    public static class JKMiscUtils
    {
        public static void Shuffle<T>(this List<T> list, int seed)
        {
            var random = new Random(seed);
            for (var i = list.Count - 1; i > 0; --i)
            {
                var j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

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
