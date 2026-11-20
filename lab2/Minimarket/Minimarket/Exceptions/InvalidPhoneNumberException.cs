using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException()
            : base("Wrong phone number!")
        {

        }
    }
}
