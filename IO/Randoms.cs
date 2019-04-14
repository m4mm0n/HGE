using System;

namespace HGE.IO
{
    internal static class Randoms
    {
        private static Random rnd = new Random();
        private static int seed;

        static Randoms()
        {
            Seed = Environment.TickCount;
        }

        public static int Seed
        {
            get => seed;
            set
            {
                seed = value;
                rnd = new Random(value);
            }
        }

        public static byte RandomByte(int count)
        {
            return RandomBytes(1)[0];
        }

        public static byte[] RandomBytes(int count)
        {
            var b = new byte[count];
            rnd.NextBytes(b);
            return b;
        }

        public static int RandomInt(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static float RandomFloat(float min = 0, float max = 1)
        {
            return (float) rnd.NextDouble() * (max - min) + min;
        }
    }
}