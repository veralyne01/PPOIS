namespace EcologyProject;

public class InvalidPollutionValueException : Exception
{
    public InvalidPollutionValueException() : base("Pollution value cannot be negative.") { }
}
