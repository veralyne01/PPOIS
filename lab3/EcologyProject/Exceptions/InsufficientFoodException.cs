namespace EcologyProject;

public class InsufficientFoodException : Exception
{
    public InsufficientFoodException() : base("No suitable food is available.") { }
}
