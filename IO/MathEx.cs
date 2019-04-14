using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace HGE.IO
{
    public static class MathEx
    {
        public static readonly float PI = (float) Math.PI;

        public static Vector2 mix(Vector2 x, Vector2 y, float a)
        {
            return x * (1f - a) + y * a;
        }

        public static float distance(Vector2 p0, Vector2 p1)
        {
            var vector = p0 - p1;
            var x = Vector2.Dot(vector, vector);
            return (float) Math.Sqrt(x);
        }

        public static Vector2 fract(Vector2 x)
        {
            return x - floor(x);
        }

        public static Vector2 round(Vector2 a)
        {
            return new Vector2((float) Math.Round(a.X), (float) Math.Round(a.Y));
        }

        public static Vector2 floor(Vector2 a)
        {
            return new Vector2((float) Math.Floor(a.X), (float) Math.Floor(a.Y));
        }

        public static float smoothstep(float edge0, float edge1, float x)
        {
            x = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }

        public static float clamp(float n, float min, float max)
        {
            //return Math.Max(Math.Min(n, max), min);
            n = n > max ? max : n;
            n = n < min ? min : n;

            return n;
        }

        public static double clampAngle(double degrees)
        {
            if (degrees >= 360)
                do
                {
                    degrees -= 360;
                } while (degrees >= 360);
            else if (degrees < 0)
                do
                {
                    degrees += 360;
                } while (degrees < 0);

            if (degrees == 360) degrees = 0;

            return degrees;
        }

        public static double clamp(double n, double min, double max)
        {
            return Math.Max(Math.Min(n, max), min);
        }

        public static float sin(float a)
        {
            return (float) Math.Sin(a);
        }

        public static float smoothstep(float x)
        {
            return x * x * (3 - 2 * x);
        }

        public static float smootherstep(float x)
        {
            return x * x * x * (x * (x * 6 - 15) + 10);
        }

        public static float catmullrom(float t, float p0, float p1, float p2, float p3)
        {
            return 0.5f * (2 * p1 + (-p0 + p2) * t + (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
                           (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t);
        }

        public static float step(float edge, float x)
        {
            return x < edge ? 0.0f : 1.0f;
        }

        public static float Sin(float val)
        {
            return (float) Math.Sin(val);
        }

        public static float Cos(float val)
        {
            return (float) Math.Cos(val);
        }

        public static float Tan(float val)
        {
            return (float) Math.Tan(val);
        }

        public static float Power(float val, float pow)
        {
            return (float) Math.Pow(val, pow);
        }

        public static float Round(float val, int digits = 0)
        {
            return (float) Math.Round(val, digits);
        }

        public static float Map(float val, float oMin, float oMax, float nMin, float nMax)
        {
            return (val - oMin) / (oMax - oMin) * (nMax - nMin) + nMin;
        }

        public static float Constrain(float val, float min, float max)
        {
            return Math.Max(Math.Min(max, val), min);
        }

        public static float Lerp(float start, float end, float amt)
        {
            return Map(amt, 0, 1, start, end);
        }

        public static float Wrap(float val, float min, float max)
        {
            if (val > max)
                return val - min;
            if (val < min)
                return val - max;
            return val;
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return Power(Power(x2 - x1, 2) + Power(y2 - y1, 2), 1 / 2);
        }

        public static float Magnitude(float x, float y)
        {
            return Power(Power(x, 2) + Power(y, 2), 1 / 2);
        }

        public static bool Between(float val, float min, float max)
        {
            return val > min && val < max;
        }

        public static void Seed()
        {
            Randoms.Seed = Environment.TickCount % int.MaxValue;
        }

        public static void Seed(int s)
        {
            Randoms.Seed = s;
        }

        public static int Random(int max)
        {
            return Random(0, max);
        }

        public static int Random(int min, int max)
        {
            return Randoms.RandomInt(min, max);
        }

        public static float Random()
        {
            return Random(0f, 1f);
        }

        public static float Random(float max)
        {
            return Random(0, max);
        }

        public static float Random(float min, float max)
        {
            return Randoms.RandomFloat(min, max);
        }

        public static T Random<T>(params T[] list)
        {
            return list[Random(list.Length)];
        }

        public static T Random<T>(List<T> list)
        {
            return list[Random(list.Count)];
        }

        public static T Random<T>(IEnumerable<T> list)
        {
            return Random(list.ToArray());
        }

        public static float Degrees(float radians)
        {
            return (float) (radians * 180 / Math.PI);
        }

        public static float Radians(float degrees)
        {
            return (float) (degrees * Math.PI / 180);
        }

        public static double Radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static T[] MakeArray<T>(params T[] items)
        {
            return items;
        }

        public static T[] MakeArray<T>(int count, Func<int, T> selector)
        {
            var arr = new T[count];
            for (var i = 0; i < count; i++)
                arr[i] = selector(i);
            return arr;
        }

        public static List<T> MakeList<T>(params T[] items)
        {
            return items.ToList();
        }

        public static List<T> MakeList<T>(int count, Func<int, T> selector)
        {
            var list = new List<T>(count);
            for (var i = 0; i < count; i++)
                list.Add(selector(i));
            return list;
        }
    }
}