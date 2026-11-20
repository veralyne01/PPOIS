using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class PhoneNumber
    {
        private const int belCode = 375;
        private int opCode;
        private int number;
        private bool ValidateNumber()
        {
            ValidateCode();
            int digits = number.ToString().Length;
            if (digits > 7 || number < 0) throw new InvalidPhoneNumberException();
            return true;
        }
        private bool ValidateCode()
        {
            int[] codes = { 25, 29, 33, 44 };
            if (codes.Contains(opCode)) return true;
            throw new InvalidPhoneNumberException();
        }

        PhoneNumber(int opCode, int number)
        {
            this.opCode = opCode;
            this.number = number;
            ValidateNumber();
        }
    }
}
