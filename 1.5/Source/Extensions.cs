using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Rimball
{
    public static class Extensions
    {
        private static System.Random rng = new System.Random();

        public static class ThreadSafeRandom
        {
            [ThreadStatic] private static System.Random Local;

            public static System.Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string ToStringDecimal(this float num)
        {
            string result;
            if (Math.Abs(num) < 1f)
            {
                result = Math.Round(num, 2).ToString("0.##");
            }
            else
            {
                result = Math.Round(num, 1).ToString("0.#");
            }
            return result;
        }


        public static int RoundToAsInt(this float num, int factor)
        {
            return (int)(Math.Round(num / (double)factor, 0) * factor);
        }


        public static Rect LeftThird(this Rect rect)
        {
            return new Rect(rect.x, rect.y, rect.width / 3f, rect.height);
        }


        public static Rect MiddleThird(this Rect rect)
        {
            return new Rect(rect.x + rect.width / 3f, rect.y, rect.width / 3f, rect.height);
        }


        public static Rect RightThird(this Rect rect)
        {
            return new Rect(rect.x + rect.width / 1.5f, rect.y, rect.width / 3f, rect.height);
        }
    }
}
