namespace EcologyProject;

public class FactoryLocationException : Exception
{
    public FactoryLocationException() : base("The factory location is not valid.") { }
}
