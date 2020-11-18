using System;
using System.Collections.Generic;
using System.Text;

namespace GPing
{
    public class PingerException : Exception
    {
        public PingerException()
        {
        }

        public PingerException(string message)
            : base(message)
        {
        }

        public PingerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
