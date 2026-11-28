namespace EcologyProject;

public class InvalidProtectedAreaException : Exception
{
    public InvalidProtectedAreaException() : base("Protected area configuration is invalid.") { }
}
