using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaBotV2.Util
{
    class WeightedRandom<T> where T : class
    {
        List<KeyValuePair<int, T>> values = new List<KeyValuePair<int,T>>();
        Random rnd = new Random();
        List<T> suppress = new List<T>();

        public void AddValue(int weight, T value) {
            values.Add(new KeyValuePair<int, T>(weight, value));
        }

        public T Choose() {
            int index = rnd.Next(GetSum());
            foreach (KeyValuePair<int, T> value in values) {
                if (!suppress.Contains(value.Value)) {
                    index -= value.Key;
                    if (index < 0) {
                        return value.Value;
                    }
                }
            }
            return null;
        }

        public void Suppress(T item) {
            suppress.Add(item);
        }

        private int GetSum() {
            int sum = 0;
            foreach (KeyValuePair<int, T> value in values) {
                if (!suppress.Contains(value.Value)) {
                    sum += value.Key;
                }
            }
            return sum;
        }
    }
}
