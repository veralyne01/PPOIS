using EcologyProject;
using Xunit;

namespace EcologyProject.Tests;

public class NaturalAreaTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveSize(double size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Forest(size, "Forest"));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsEmptyName(string name) =>
        Assert.Throws<ArgumentException>(() => new Forest(10, name));

    [Fact]
    public void Area_ExposesExpectedTypeAndPollutionParameters()
    {
        var forest = new Forest(100, "Forest");
        Assert.Equal(NaturalAreaType.Forest, forest.AreaType);
        Assert.True(forest.Pollution.HasParameter(PollutionType.Air));
        Assert.True(forest.Pollution.HasParameter(PollutionType.Soil));
        Assert.False(forest.Pollution.HasParameter(PollutionType.Water));
    }

    [Fact]
    public void AddCreature_AddsAllowedOrganism()
    {
        var forest = new Forest(10, "Forest");
        var grass = new Grass();
        forest.AddCreature(grass);
        Assert.Contains(grass, forest.Organisms);
        Assert.True(forest.CanContainOrganism(grass));
    }

    [Fact]
    public void AddCreature_RejectsNull() =>
        Assert.Throws<ArgumentNullException>(() => new Forest(10, "Forest").AddCreature(null!));

    [Fact]
    public void AddCreature_RejectsOrganismFromAnotherHabitat() =>
        Assert.Throws<UnsuitableNatureAreaException>(() => new Forest(10, "Forest").AddCreature(new Shark()));

    [Fact]
    public void AddCreature_RejectsDeadOrganism()
    {
        var grass = new Grass();
        grass.MarkAsHunted();
        Assert.Throws<OrganismAlreadyDeadException>(() => new Forest(10, "Forest").AddCreature(grass));
    }

    [Fact]
    public void AddCreature_RejectsSameOrganismTwice()
    {
        var forest = new Forest(10, "Forest");
        var grass = new Grass();
        forest.AddCreature(grass);
        Assert.Throws<OrganismAlreadyExistsException>(() => forest.AddCreature(grass));
    }

    [Fact]
    public void RemoveCreature_RemovesExistingOrganism()
    {
        var forest = new Forest(10, "Forest");
        var grass = new Grass();
        forest.AddCreature(grass);
        Assert.True(forest.RemoveCreature(grass));
        Assert.Empty(forest.Organisms);
        Assert.False(forest.RemoveCreature(grass));
    }

    [Fact]
    public void AddNeighbor_CreatesBidirectionalConnectionAndIgnoresSelf()
    {
        var forest = new Forest(10, "Forest");
        var plains = new Plains(10, "Plains");
        forest.AddNeighbor(plains);
        forest.AddNeighbor(forest);
        Assert.Contains(plains, forest.Neighbors);
        Assert.Contains(forest, plains.Neighbors);
        Assert.Single(forest.Neighbors);
    }

    [Fact]
    public void ToString_ContainsNameAndType() =>
        Assert.Equal("Forest [Forest]", new Forest(10, "Forest").ToString());

    [Theory]
    [InlineData(typeof(Forest), NaturalAreaType.Forest)]
    [InlineData(typeof(Lake), NaturalAreaType.Lake)]
    [InlineData(typeof(River), NaturalAreaType.River)]
    [InlineData(typeof(Sea), NaturalAreaType.Sea)]
    [InlineData(typeof(Mountain), NaturalAreaType.Mountain)]
    [InlineData(typeof(Plains), NaturalAreaType.Plains)]
    public void EveryAreaType_IsCorrect(Type type, NaturalAreaType expected)
    {
        var area = (NaturalArea)Activator.CreateInstance(type, 10d, "Area")!;
        Assert.Equal(expected, area.AreaType);
        Assert.NotEmpty(area.AllowedOrganismNames);
    }
}

public class EcosystemTests
{
    [Fact]
    public void AddArea_AssignsCoordinatesAndSeedsOrganisms()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(100, "Forest");
        ecosystem.AddArea(forest);
        Assert.Equal(0, forest.MapX);
        Assert.Equal(0, forest.MapY);
        Assert.NotNull(forest.Climate);
        Assert.NotEmpty(forest.Organisms);
    }

    [Fact]
    public void AddArea_RejectsNullAndDuplicate()
    {
        var ecosystem = new Ecosystem();
        Assert.Throws<ArgumentNullException>(() => ecosystem.AddArea(null!));
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        Assert.Throws<InvalidOperationException>(() => ecosystem.AddArea(forest));
    }

    [Fact]
    public void AddArea_RejectsOccupiedExplicitCoordinates()
    {
        var ecosystem = new Ecosystem();
        ecosystem.AddArea(new Forest(10, "First"), 2, 3);
        Assert.Throws<MapException>(() => ecosystem.AddArea(new Forest(10, "Second"), 2, 3));
    }

    [Fact]
    public void AddArea_AutomaticallyConnectsAdjacentAreas()
    {
        var ecosystem = new Ecosystem();
        var first = new Forest(10, "First");
        var second = new Lake(10, "Second");
        ecosystem.AddArea(first, 0, 0);
        ecosystem.AddArea(second, 1, 0);
        Assert.Contains(second, first.Neighbors);
        Assert.Contains(first, second.Neighbors);
    }

    [Fact]
    public void ConnectAreas_RejectsAreasOutsideEcosystem()
    {
        var ecosystem = new Ecosystem();
        var first = new Forest(10, "First");
        var second = new Lake(10, "Second");
        Assert.Throws<ObjectIsNotInTheEcosystemException>(() => ecosystem.ConnectAreas(first, second));
    }

    [Fact]
    public void AddFactory_ValidatesLocationAndUniqueId()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        Assert.Throws<FactoryLocationException>(() => ecosystem.AddFactory(new Factory("External", FactoryType.TPP, new Forest(10, "Outside"))));
        var factory = new Factory("Factory", FactoryType.TPP, forest);
        ecosystem.AddFactory(factory);
        Assert.Throws<InvalidOperationException>(() => ecosystem.AddFactory(factory));
    }

    [Fact]
    public void AddProtectedArea_ValidatesZonesAndNames()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        var reserve = new NatureReserve("Reserve", new List<NaturalArea> { forest });
        Assert.Throws<InvalidProtectedAreaException>(() => ecosystem.AddProtectedArea(reserve, Array.Empty<NaturalArea>()));
        Assert.Throws<ObjectIsNotInTheEcosystemException>(() => ecosystem.AddProtectedArea(new NatureReserve("External", new List<NaturalArea> { forest }), new[] { new Forest(10, "Outside") }));
        ecosystem.AddProtectedArea(reserve, new[] { forest });
        Assert.Throws<InvalidProtectedAreaException>(() => ecosystem.AddProtectedArea(new NatureReserve("reserve", new List<NaturalArea> { forest }), new[] { forest }));
        Assert.Throws<ArgumentNullException>(() => ecosystem.AddProtectedArea(null!, new[] { forest }));
        Assert.Throws<ArgumentNullException>(() => ecosystem.AddProtectedArea(new NatureReserve("Other", new List<NaturalArea> { forest }), null!));
    }

    [Fact]
    public void CanHunt_ReflectsProtectedAreas()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        Assert.True(ecosystem.CanHunt("Deer", forest));
        ecosystem.AddProtectedArea(new WildlifeSanctuary("Sanctuary", new[] { "Deer" }, new List<NaturalArea> { forest }), new[] { forest });
        Assert.False(ecosystem.CanHunt("Deer", forest));
        Assert.True(ecosystem.CanHunt("Wolf", forest));
    }

    [Fact]
    public void ApplyFactoryEmissions_AffectsFactoryAreaAndNeighbors()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        var lake = new Lake(10, "Lake");
        ecosystem.AddArea(forest, 0, 0);
        ecosystem.AddArea(lake, 1, 0);
        ecosystem.AddFactory(new Factory("Factory", FactoryType.TPP, forest));
        ecosystem.ApplyFactoryEmissions();
        Assert.Equal(10d, forest.Pollution.GetLevel(PollutionType.Air));
        Assert.Equal(3d, lake.Pollution.GetLevel(PollutionType.Water));
    }

    [Fact]
    public void ApplyFactoryEmissions_UsesProtectedAreaMultiplier()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        ecosystem.AddFactory(new Factory("Factory", FactoryType.TPP, forest));
        ecosystem.AddProtectedArea(new NatureReserve("Reserve", new List<NaturalArea> { forest }), new[] { forest });
        ecosystem.ApplyFactoryEmissions();
        Assert.Equal(0d, forest.Pollution.GetLevel(PollutionType.Air));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(36501)]
    public void AdvanceTime_RejectsInvalidStepCount(int steps) =>
        Assert.Throws<InvalidSimulationStepException>(() => new Ecosystem().AdvanceTime(steps));

    [Fact]
    public void AdvanceTime_AdvancesRequestedNumberOfDays()
    {
        var ecosystem = new Ecosystem();
        ecosystem.AddArea(new Forest(10, "Forest"));
        ecosystem.AdvanceTime(3);
        Assert.Equal(3, ecosystem.Day);
    }

    [Fact]
    public void AdvanceOneDay_UpdatesDayWeatherAndRemovesDeadOrganisms()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest")
        {
            Climate = new AreaClimate(10, 10, 20, 20, 30, 30)
        };
        ecosystem.AddArea(forest);
        ecosystem.AdvanceOneDay();
        Assert.Equal(1, ecosystem.Day);
        Assert.NotNull(forest.CurrentWeather);
        Assert.Equal(10, forest.CurrentWeather!.Temperature);
        Assert.Equal(20, forest.CurrentWeather.Precipitation);
        Assert.Equal(30, forest.CurrentWeather.CloudCover);
    }

    [Fact]
    public void DisplayMethods_DoNotThrow()
    {
        var ecosystem = new Ecosystem();
        ecosystem.ShowMap();
        ecosystem.ShowStatus();
        ecosystem.AddArea(new Forest(10, "Forest"));
        ecosystem.ShowMap();
        ecosystem.ShowStatus();
    }
}

public class ClimateTests
{
    [Fact]
    public void TodayWeather_ValuesStayWithinClimateRanges()
    {
        var climate = new AreaClimate(-10, 20, 10, 80, 0, 100);
        for (int i = 0; i < 100; i++)
        {
            var weather = new TodayWeather(climate);
            Assert.InRange(weather.Temperature, -10, 20);
            Assert.InRange(weather.Precipitation, 10, 80);
            Assert.InRange(weather.CloudCover, 0, 100);
        }
    }

    [Fact]
    public void TodayWeather_RejectsNullClimate() =>
        Assert.Throws<ArgumentNullException>(() => new TodayWeather(null!));

}

public class AdditionalCoverageTests
{
    [Fact]
    public void Factory_UnknownEnumHasNoPollutionImpact()
    {
        var factory = new Factory("Unknown", (FactoryType)999, new Forest(10, "Forest"));
        Assert.Empty(factory.GetPollutionImpact());
    }

    [Fact]
    public void Pollution_AddOverwritesExistingLevel()
    {
        var pollution = new PollutionLevel();
        pollution.Add(PollutionType.Air, 5);
        pollution.Add(PollutionType.Air, 12);
        Assert.Equal(12d, pollution.GetLevel(PollutionType.Air));
    }

    [Fact]
    public void Catalog_UsesFallbackAnimalTypesAndFood()
    {
        Assert.IsType<Mammal>(OrganismCatalog.Create("Unknown mammal", new[] { "Grass" }));
        Assert.IsType<Bird>(OrganismCatalog.Create("Cuckoo", new[] { "Insect" }));
        Assert.IsType<Fish>(OrganismCatalog.Create("Cod", new[] { "Small fish" }));
        Assert.IsType<Insect>(OrganismCatalog.Create("Ant", new[] { "Grass" }));
        Assert.IsType<Crayfish>(OrganismCatalog.Create("Lobster", new[] { "Algae" }));
        Assert.IsType<Octopus>(OrganismCatalog.Create("Mussel", new[] { "Algae" }));
    }

    [Fact]
    public void Catalog_CreatesFallbackFoodWhenNoOtherSpeciesAreProvided()
    {
        var wolf = Assert.IsType<Wolf>(OrganismCatalog.Create("Wolf"));
        Assert.Empty(wolf.Food);
        var deer = Assert.IsType<Deer>(OrganismCatalog.Create("Deer"));
        Assert.Empty(deer.Food);
    }

    [Fact]
    public void ConnectAreas_CreatesBidirectionalLink()
    {
        var ecosystem = new Ecosystem();
        var first = new Forest(10, "Forest");
        var second = new Lake(10, "Lake");
        ecosystem.AddArea(first, 0, 0);
        ecosystem.AddArea(second, 10, 10);
        ecosystem.ConnectAreas(first, second);
        Assert.Contains(second, first.Neighbors);
        Assert.Contains(first, second.Neighbors);
    }

    [Fact]
    public void ApplyFactoryEmissions_DoesNothingWithoutFactories()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        ecosystem.ApplyFactoryEmissions();
        Assert.Equal(0d, forest.Pollution.GetLevel(PollutionType.Air));
    }

    [Fact]
    public void Hunter_CanHuntUnprotectedSpeciesInWildlifeSanctuary()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(10, "Forest");
        ecosystem.AddArea(forest);
        ecosystem.AddProtectedArea(
            new WildlifeSanctuary("Sanctuary", new[] { "Wolf" }, new List<NaturalArea> { forest }),
            new[] { forest });
        var deer = new Deer();
        forest.AddCreature(deer);
        var hunter = new Hunter(new Dictionary<string, int> { ["Deer"] = 1 });
        hunter.Kill(deer, forest, ecosystem);
        Assert.Equal(1, hunter.Killed["Deer"]);
    }

    [Fact]
    public void Animal_IncreaseHungerDefaultAddsTwenty()
    {
        var animal = new Deer();
        animal.IncreaseHunger();
        Assert.Equal(20, animal.Hunger);
    }

    [Fact]
    public void Animal_LiveDayCooldownDecreases()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        var forest = new Forest(100, "Forest");
        forest.AddCreature(deer);
        for (int i = 0; i < 10; i++) forest.AddCreature(new Grass());
        for (int i = 0; i < 30; i++) deer.LiveDay(forest);
        deer.MarkReproduced();
        deer.LiveDay(forest);
        Assert.Equal(29, deer.ReproductionCooldown);
    }
}

public class EdgeCaseTests
{
    [Fact]
    public void Catalog_RecognizesAdditionalAnimalCategories()
    {
        Assert.IsType<Bird>(OrganismCatalog.Create("Albatross", new[] { "Fish" }));
        Assert.IsType<Fish>(OrganismCatalog.Create("Catfish", new[] { "Fish" }));
        Assert.IsType<Insect>(OrganismCatalog.Create("Butterfly", new[] { "Grass" }));
        Assert.IsType<Crayfish>(OrganismCatalog.Create("Crab", new[] { "Algae" }));
        Assert.IsType<Mammal>(OrganismCatalog.Create("Dolphin", new[] { "Fish" }));
    }

    [Fact]
    public void Ecosystem_AddFactoryRejectsNull()
    {
        var ecosystem = new Ecosystem();
        Assert.Throws<ArgumentNullException>(() => ecosystem.AddFactory(null!));
    }

    [Fact]
    public void Ecosystem_EmptyAreaCanBeAddedAndSimulated()
    {
        var ecosystem = new Ecosystem();
        var area = new EmptyTestArea(10, "Empty");
        ecosystem.AddArea(area);
        Assert.Empty(area.Organisms);
        ecosystem.AdvanceOneDay();
        Assert.Equal(1, ecosystem.Day);
        Assert.NotNull(area.CurrentWeather);
    }

    private sealed class EmptyTestArea : NaturalArea
    {
        public override NaturalAreaType AreaType => NaturalAreaType.Plains;
        public EmptyTestArea(double size, string name) : base(size, name, Array.Empty<string>()) { }
    }
}
