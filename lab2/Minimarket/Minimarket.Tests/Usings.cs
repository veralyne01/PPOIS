global using Xunit;
global using Xunit.Sdk;
global using Xunit.v3;

// Tests share Console and use reflection to inspect private state.
// Keep this assembly sequential to avoid cross-test interference.
[assembly: Parallelization(Mode = ParallelMode.None)]
