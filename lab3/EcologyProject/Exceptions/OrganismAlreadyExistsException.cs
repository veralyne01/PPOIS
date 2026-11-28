namespace EcologyProject;

public class OrganismAlreadyExistsException : Exception
{
    public OrganismAlreadyExistsException() : base("The organism already exists in this area.") { }
}
