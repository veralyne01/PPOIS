using EcologyProject;
using Xunit;
namespace EcologyProject.Tests;

public class ProtectedAreaAndHuntingTests
{
    [Fact]
    public void NatureReserve_ProtectsEverySpeciesAndBlocksEmissions()
    {
        var forest = new Forest(10, "Forest");
        var reserve = new NatureReserve("Reserve", new List<NaturalArea> { forest });
        Assert.Equal(0, reserve.EmissionMultiplier);
        Assert.True(reserve.ProtectsAllSpeciesFromHunting);
        Assert.False(reserve.IsHuntingAllowed("Deer"));
        Assert.True(reserve.Contains(forest));
        Assert.False(reserve.Contains(new Forest(10, "Other")));
    }

    [Fact]
    public void WildlifeSanctuary_ProtectsOnlyListedSpecies()
    {
        var forest = new Forest(10, "Forest");
        var sanctuary = new WildlifeSanctuary("Sanctuary", new[] { "Deer", "Wolf" }, new List<NaturalArea> { forest });
        Assert.Equal(0.5, sanctuary.EmissionMultiplier);
        Assert.False(sanctuary.ProtectsAllSpeciesFromHunting);
        Assert.False(sanctuary.IsHuntingAllowed("deer"));
        Assert.True(sanctuary.IsHuntingAllowed("Fox"));
        Assert.Contains("Wolf", sanctuary.ProtectedSpecies);
    }

    [Fact]
    public void ProtectedArea_RejectsInvalidConstruction()
    {
        var forest = new Forest(10, "Forest");
        Assert.Throws<InvalidProtectedAreaException>(() => new NatureReserve(" ", new List<NaturalArea> { forest }));
        Assert.Throws<InvalidProtectedAreaException>(() => new NatureReserve("Reserve", new List<NaturalArea>()));
        Assert.Throws<EmptySanctuaryException>(() => new WildlifeSanctuary("Sanctuary", Array.Empty<string>(), new List<NaturalArea> { forest }));
        Assert.Throws<ArgumentNullException>(() => new WildlifeSanctuary("Sanctuary", null!, new List<NaturalArea> { forest }));
    }

    [Fact]
    public void ProtectedArea_AddAreaDoesNotDuplicate()
    {
        var forest = new Forest(10, "Forest");
        var reserve = new NatureReserve("Reserve", new List<NaturalArea> { forest });
        reserve.AddArea(forest);
        Assert.Single(reserve.Areas);
        Assert.Throws<ArgumentNullException>(() => reserve.AddArea(null!));
    }

    [Fact]
    public void Hunter_RejectsNonAnimalDeadUnlistedAndOverLimitVictims()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        var hunter = new Hunter(new Dictionary<string, int> { ["Deer"] = 1 });
        Assert.Throws<WrongVictimException>(() => hunter.Kill(new Grass(), forest, ecosystem));
        var deadDeer = new Deer();
        deadDeer.MarkAsHunted();
        Assert.Throws<WrongVictimException>(() => hunter.Kill(deadDeer, forest, ecosystem));
        Assert.Throws<WrongVictimException>(() => hunter.Kill(new Wolf(), forest, ecosystem));
        var deer = new Deer();
        forest.AddCreature(deer);
        hunter.Kill(deer, forest, ecosystem);
        var second = new Deer();
        forest.AddCreature(second);
        Assert.Throws<WrongVictimException>(() => hunter.Kill(second, forest, ecosystem));
    }

    [Fact]
    public void Hunter_CannotHuntInNatureReserve()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        ecosystem.AddProtectedArea(new NatureReserve("Reserve", new List<NaturalArea> { forest }), new[] { forest });
        var deer = new Deer();
        forest.AddCreature(deer);
        Assert.Throws<WrongVictimException>(() => new Hunter(new Dictionary<string, int> { ["Deer"] = 1 }).Kill(deer, forest, ecosystem));
    }
}