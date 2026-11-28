using EcologyProject;
using Xunit;
namespace EcologyProject.Tests;

public class PollutionAndFactoryTests
{
    [Fact]
    public void Pollution_AddIncreaseDecreaseAndGetLevel()
    {
        var pollution = new PollutionLevel();
        pollution.Add(PollutionType.Air, 10);
        Assert.Equal(10d, pollution.GetLevel(PollutionType.Air));
        Assert.True(pollution.Increase(PollutionType.Air, 5));
        Assert.Equal(15d, pollution.GetLevel(PollutionType.Air));
        Assert.True(pollution.Decrease(PollutionType.Air, 7));
        Assert.Equal(8d, pollution.GetLevel(PollutionType.Air));
        pollution.Decrease(PollutionType.Air, 100);
        Assert.Equal(0d, pollution.GetLevel(PollutionType.Air));
    }

    [Fact]
    public void Pollution_ReturnsFalseForMissingParameter()
    {
        var pollution = new PollutionLevel();
        Assert.False(pollution.Increase(PollutionType.Water, 1));
        Assert.False(pollution.Decrease(PollutionType.Water, 1));
        Assert.Null(pollution.GetLevel(PollutionType.Water));
        Assert.False(pollution.HasParameter(PollutionType.Water));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Pollution_RejectsInvalidValues(double value)
    {
        var pollution = new PollutionLevel();
        Assert.Throws<InvalidPollutionValueException>(() => pollution.Add(PollutionType.Air, value));
        Assert.Throws<InvalidPollutionValueException>(() => pollution.Increase(PollutionType.Air, value));
        Assert.Throws<InvalidPollutionValueException>(() => pollution.Decrease(PollutionType.Air, value));
    }

    [Theory]
    [InlineData(FactoryType.TPP, PollutionType.Air, 10)]
    [InlineData(FactoryType.TPP, PollutionType.Water, 3)]
    [InlineData(FactoryType.TPP, PollutionType.Soil, 5)]
    [InlineData(FactoryType.HPS, PollutionType.Water, 5)]
    [InlineData(FactoryType.NPP, PollutionType.Water, 4)]
    [InlineData(FactoryType.NPP, PollutionType.Soil, 2)]
    public void Factory_ProvidesExpectedPollutionImpact(FactoryType type, PollutionType pollutionType, double expected)
    {
        var factory = new Factory("Factory", type, new Forest(10, "Forest"));
        Assert.Equal(expected, factory.GetPollutionImpact()[pollutionType]);
    }

    [Fact]
    public void Factory_RejectsEmptyName() =>
        Assert.Throws<ArgumentException>(() => new Factory(" ", FactoryType.TPP, new Forest(10, "Forest")));

    [Fact]
    public void Factory_RejectsNullLocation() =>
        Assert.Throws<FactoryLocationException>(() => new Factory("Factory", FactoryType.TPP, null!));

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Factory_RejectsInvalidMultiplier(double multiplier) =>
        Assert.Throws<InvalidPollutionValueException>(() => new Factory("Factory", FactoryType.TPP, new Forest(10, "Forest")).Pollute(new Forest(10, "Other"), multiplier));

    [Fact]
    public void Factory_ToStringContainsNameAndType() =>
        Assert.Equal("Factory (TPP)", new Factory("Factory", FactoryType.TPP, new Forest(10, "Forest")).ToString());
}
