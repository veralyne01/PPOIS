namespace EcologyProject;

public class InvalidSimulationStepException : Exception
{
    public InvalidSimulationStepException() : base("Simulation steps must be positive.") { }
}
