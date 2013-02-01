using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2.Util
{
    class ListFormatter<T>
    {
        IEnumerable<T> list;
        string format;

        public ListFormatter(IEnumerable<T> list) : this("{0}", list) {
        }

        public ListFormatter(string format, IEnumerable<T> list) {
            this.list = list;
            this.format = format + ", ";
        }

        public override string ToString() {
            return Format();
        }

        public string Format() {
            StringBuilder result = new StringBuilder();
            foreach(T item in list) {
                if (item != null) {
                    result.Append(String.Format(format, item.ToString()));
                }
                else {
                    result.Append(String.Format(format, "(null)"));
                }
            }
            result.Remove(result.Length - 2, 2);
            return result.ToString();
        }
    }
}
