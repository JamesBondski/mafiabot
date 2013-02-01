using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MafiaBotV2
{
    [global::System.Serializable]
    public class MafiaException : Exception
    {
        public MafiaException() { }
        public MafiaException(string message) : base(message) { }
        public MafiaException(string message, Exception inner) : base(message, inner) { }
        protected MafiaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
