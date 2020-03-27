using System;
using System.Collections.Generic;
using System.Text;

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
