using System;

namespace Boxxy
{
    public class InputProcessingError : Exception
    {
        public InputProcessingError(string message): base(message) {
        }
    }
}