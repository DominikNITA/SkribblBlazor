using System;

namespace Skribbl_Website.Shared.Exceptions
{
    public class UserNotInLocalStorageException : Exception
    {
        public UserNotInLocalStorageException()
        {
        }

        public UserNotInLocalStorageException(string message) : base(message)
        {
        }
    }
}