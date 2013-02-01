using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Util
{
    static class ListShuffle
    {
        public static readonly Random random = new Random();
        public static List<T> Shuffle<T>(this List<T> toShuffle) {
            List<T> deck = new List<T>(toShuffle);
            int N = deck.Count;

            for (int i = 0; i < N; ++i) {
                int r = i + (int)(random.Next(N - i));
                T t = deck[r];
                deck[r] = deck[i];
                deck[i] = t;
            }

            return deck;
        }  
    }
}
