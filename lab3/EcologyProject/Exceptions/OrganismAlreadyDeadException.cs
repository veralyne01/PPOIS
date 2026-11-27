namespace EcologyProject;

public class OrganismAlreadyDeadException : Exception
{
    public OrganismAlreadyDeadException() : base("The organism is already dead.") { }
}
