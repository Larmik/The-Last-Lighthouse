using System;

namespace Game.Core.Save
{
    public sealed class SaveDataException : Exception
    {
        public SaveDataException(string message) : base(message)
        {
        }

        public SaveDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
