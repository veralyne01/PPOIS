namespace Minimarket
{
    public class PhoneNumber
    {
        private const int CountryCode = 375;
        private static readonly int[] operatorCodes = { 25, 29, 33, 44 };

        public int OperatorCode { get; }
        public int Number { get; }

        internal PhoneNumber(int opCode, int number)
        {
            if (!operatorCodes.Contains(opCode))
                throw new InvalidPhoneNumberException();
            if (number < 0 || number > 9_999_999 || number.ToString().Length != 7)
                throw new InvalidPhoneNumberException();

            OperatorCode = opCode;
            Number = number;
        }

        public static PhoneNumber Create(int operatorCode, int number)
        {
            return new PhoneNumber(operatorCode, number);
        }

        public override string ToString()
        {
            return $"+{CountryCode} ({OperatorCode}) {Number:0000000}";
        }
    }
}
