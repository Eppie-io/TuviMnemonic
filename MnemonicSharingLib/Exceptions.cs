using System;

namespace MnemonicSharingLib
{
    public class PartialMnemonicsCreationException : Exception
    {
        public PartialMnemonicsCreationException()
        {
        }

        public PartialMnemonicsCreationException(string message) : base(message)
        {
        }

        public PartialMnemonicsCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
