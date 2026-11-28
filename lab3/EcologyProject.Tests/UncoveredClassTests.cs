using System.Reflection;
using EcologyProject;
using Xunit;

namespace EcologyProject.Tests;

public class AdditionalOrganismTests
{
    [Fact]
    public void Bear_UsesPredatorCategoryAndDefaultDiet()
    {
        var bear = new Bear();
        Assert.Equal("Bear", bear.Name);
        Assert.Equal("Mammal", bear.Type);
        Assert.Equal(MammalTypes.predator, bear.Category);
        Assert.Contains("Deer", bear.Food);
        Assert.Contains("Grass", bear.Food);
    }

    [Fact]
    public void Bear_AcceptsCustomNameAndDiet()
    {
        var bear = new Bear("Brown bear", new[] { "Salmon", "Berry" });
        Assert.Equal("Brown bear", bear.Name);
        Assert.Contains("Salmon", bear.Food);
        Assert.DoesNotContain("Deer", bear.Food);
    }

    [Fact]
    public void Beaver_UsesRodentCategoryAndDefaultDiet()
    {
        var beaver = new Beaver();
        Assert.Equal("Beaver", beaver.Name);
        Assert.Equal(MammalTypes.rodent, beaver.Category);
        Assert.Contains("Reed", beaver.Food);
        Assert.Contains("Aspen", beaver.Food);
    }

    [Fact]
    public void Beaver_AcceptsCustomNameAndDiet()
    {
        var beaver = new Beaver("River engineer", new[] { "Willow" });
        Assert.Equal("River engineer", beaver.Name);
        Assert.Contains("Willow", beaver.Food);
        Assert.DoesNotContain("Birch", beaver.Food);
    }

    [Fact]
    public void Duck_UsesWaterfowlCategoryAndDefaultDiet()
    {
        var duck = new Duck();
        Assert.Equal("Duck", duck.Name);
        Assert.Equal(BirdTypes.waterfaul, duck.Category);
        Assert.Contains("Algae", duck.Food);
        Assert.Contains("Small fish", duck.Food);
    }

    [Fact]
    public void Duck_AcceptsCustomNameAndDiet()
    {
        var duck = new Duck("Mallard", new[] { "Seeds" });
        Assert.Equal("Mallard", duck.Name);
        Assert.Contains("Seeds", duck.Food);
        Assert.DoesNotContain("Algae", duck.Food);
    }

    [Fact]
    public void Eagle_UsesPredatorCategoryAndDefaultDiet()
    {
        var eagle = new Eagle();
        Assert.Equal("Eagle", eagle.Name);
        Assert.Equal(BirdTypes.predator, eagle.Category);
        Assert.Contains("Mouse", eagle.Food);
        Assert.Contains("Hare", eagle.Food);
    }

    [Fact]
    public void Eagle_AcceptsCustomNameAndDiet()
    {
        var eagle = new Eagle("Golden eagle", new[] { "Trout" });
        Assert.Equal("Golden eagle", eagle.Name);
        Assert.Contains("Trout", eagle.Food);
        Assert.DoesNotContain("Mouse", eagle.Food);
    }

    [Fact]
    public void Owl_UsesPredatorCategoryAndDefaultDiet()
    {
        var owl = new Owl();
        Assert.Equal("Owl", owl.Name);
        Assert.Equal(BirdTypes.predator, owl.Category);
        Assert.Contains("Mouse", owl.Food);
        Assert.Contains("Vole", owl.Food);
    }

    [Fact]
    public void Owl_AcceptsCustomNameAndDiet()
    {
        var owl = new Owl("Snowy owl", new[] { "Lemming" });
        Assert.Equal("Snowy owl", owl.Name);
        Assert.Contains("Lemming", owl.Food);
        Assert.DoesNotContain("Vole", owl.Food);
    }

    [Fact]
    public void Pike_UsesFreshwaterCategoryAndDefaultDiet()
    {
        var pike = new Pike();
        Assert.Equal("Pike", pike.Name);
        Assert.Equal(FishTypes.freshwater, pike.Category);
        Assert.Contains("Crayfish", pike.Food);
        Assert.Contains("Algae", pike.Food);
    }

    [Fact]
    public void Pike_AcceptsCustomNameAndDiet()
    {
        var pike = new Pike("Northern pike", new[] { "Perch" });
        Assert.Equal("Northern pike", pike.Name);
        Assert.Contains("Perch", pike.Food);
        Assert.DoesNotContain("Crayfish", pike.Food);
    }

    [Fact]
    public void Mushroom_UsesMushroomTypeAndProvidedName()
    {
        var mushroom = new Mushroom("Fly agaric");
        Assert.Equal("Fly agaric", mushroom.Name);
        Assert.Equal("Mushroom", mushroom.Type);
        Assert.True(mushroom.IsAlive);
    }

    [Fact]
    public void EdibleMushroom_UsesDefaultAndCustomNames()
    {
        var defaultMushroom = new EdibleMushroom();
        var customMushroom = new EdibleMushroom("Chanterelle");

        Assert.Equal("EdibleMushroom", defaultMushroom.Name);
        Assert.Equal("Mushroom", defaultMushroom.Type);
        Assert.Equal("Chanterelle", customMushroom.Name);
        Assert.IsAssignableFrom<Mushroom>(customMushroom);
    }

    [Fact]
    public void Mushroom_RejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() => new Mushroom(" "));
        Assert.Throws<ArgumentException>(() => new EdibleMushroom(""));
    }
}

public class MenuUITests
{
    private static string RunWithInput(Action action, string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var writer = new StringWriter();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void AddAreaToEcosystem_CreatesSelectedArea()
    {
        var ecosystem = new Ecosystem();
        string output = RunWithInput(() => MenuUI.AddAreaToEcosystem(ecosystem), "1\nPine forest\n25\n");
        Assert.Single(ecosystem.Areas);
        Assert.IsType<Forest>(ecosystem.Areas[0]);
        Assert.Equal("Pine forest", ecosystem.Areas[0].Name);
        Assert.Contains("added", output);
    }

    [Fact]
    public void AddAreaToEcosystem_HandlesBackAndInvalidInput()
    {
        var ecosystem = new Ecosystem();
        string backOutput = RunWithInput(() => MenuUI.AddAreaToEcosystem(ecosystem), "7\n");
        Assert.Empty(ecosystem.Areas);
        Assert.Contains("Choose area type", backOutput);

        string invalidTypeOutput = RunWithInput(() => MenuUI.AddAreaToEcosystem(ecosystem), "99\nName\n10\n");
        Assert.Contains("Invalid area type", invalidTypeOutput);
        Assert.Empty(ecosystem.Areas);

        string invalidSizeOutput = RunWithInput(() => MenuUI.AddAreaToEcosystem(ecosystem), "2\nLake\nnot-a-number\n");
        Assert.Contains("Invalid area size", invalidSizeOutput);
    }

    [Fact]
    public void AddAreaToEcosystem_RejectsEmptyNameAndNonPositiveSize()
    {
        var ecosystem = new Ecosystem();
        string output = RunWithInput(() => MenuUI.AddAreaToEcosystem(ecosystem), "3\n   \n0\n");
        Assert.Contains("must not be empty", output);
        Assert.Empty(ecosystem.Areas);
    }

    [Fact]
    public void AddProtectedArea_RequiresExistingAreas()
    {
        string output = RunWithInput(() => MenuUI.AddProtectedArea(new Ecosystem(), false), "");
        Assert.Contains("Add natural areas first", output);
    }

    [Fact]
    public void AddProtectedArea_CreatesSanctuaryAndReserve()
    {
        var ecosystem = new Ecosystem();
        ecosystem.AddArea(new Forest(20, "Forest"));

        RunWithInput(() => MenuUI.AddProtectedArea(ecosystem, false), "Sanctuary\n1\nBear, Wolf\n");
        Assert.Single(ecosystem.ProtectedAreas);
        Assert.IsType<WildlifeSanctuary>(ecosystem.ProtectedAreas[0]);

        RunWithInput(() => MenuUI.AddProtectedArea(ecosystem, true), "Reserve\n1\n");
        Assert.Equal(2, ecosystem.ProtectedAreas.Count);
        Assert.IsType<NatureReserve>(ecosystem.ProtectedAreas[1]);
    }

    [Fact]
    public void GetAreasAsList_ParsesMultipleSelectionsAndRejectsInvalidIndex()
    {
        var ecosystem = new Ecosystem();
        ecosystem.AddArea(new Forest(10, "Forest"));
        ecosystem.AddArea(new Lake(10, "Lake"));

        List<NaturalArea> selected = null!;
        RunWithInput(() => selected = MenuUI.GetAreasAsList(ecosystem), "1, 2; 1\n");
        Assert.Equal(2, selected.Count);
        Assert.Throws<FormatException>(() => RunWithInput(
            () => MenuUI.GetAreasAsList(ecosystem), "0\n"));
    }

    [Fact]
    public void AdvanceSimulation_RejectsInvalidAndAcceptsValidDayCount()
    {
        var ecosystem = new Ecosystem();
        string invalid = RunWithInput(() => MenuUI.AdvanceSimulation(ecosystem), "abc\n");
        Assert.Contains("whole number", invalid);
        Assert.Equal(0, ecosystem.Day);

        string valid = RunWithInput(() => MenuUI.AdvanceSimulation(ecosystem), "2\n\n");
        Assert.Equal(2, ecosystem.Day);
        Assert.Contains("2 day(s) passed", valid);
    }

    
    [Fact]
    public void OrganizeHunt_ReportsMissingSpeciesAndInvalidRequest()
    {
        var ecosystem = new Ecosystem();
        ecosystem.AddArea(new Forest(20, "Forest"));

        string missing = RunWithInput(() => MenuUI.OrganizeHunt(ecosystem), "1\nUnknown species\n2\n");
        Assert.Contains("No animals of this species were found", missing);

        string invalid = RunWithInput(() => MenuUI.OrganizeHunt(ecosystem), "1\n0\n");
        Assert.Contains("Invalid hunting request", invalid);
    }

    [Fact]
    public void OrganizeHunt_HuntsRequestedAnimals()
    {
        var ecosystem = new Ecosystem();
        var forest = new Forest(20, "Forest");
        ecosystem.AddArea(forest);
        var deer = new Deer();
        forest.AddCreature(deer);

        string output = RunWithInput(() => MenuUI.OrganizeHunt(ecosystem), "1\nDeer\n1\n");

        Assert.False(deer.IsAlive);
        Assert.Contains("Deer was hunted", output);
    }

    [Fact]
    public void SuggestCreation_ReturnsOnBack()
    {
        string output = RunWithInput(() => MenuUI.SuggestCreation(new Ecosystem()), "4\n");
        Assert.Contains("Back", output);
    }

    [Fact]
    public void Menu_ExitsOnOptionNine()
    {
        var ecosystem = new Ecosystem();
        string output = RunWithInput(() => MenuUI.Menu(ecosystem), "7\n9\n");
        Assert.Contains("ECOSYSTEM SIMULATION", output);
        Assert.Empty(ecosystem.Areas);
    }

    [Fact]
    public void ProgramMain_StartsMenuAndCanExit()
    {
        string output = RunWithInput(() =>
        {
            Type programType = typeof(Ecosystem).Assembly.GetType("EcologyProject.Program", throwOnError: true)!;
            MethodInfo main = programType.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic)!;
            main.Invoke(null, new object[] { Array.Empty<string>() });
        }, "7\n9\n");

        Assert.Contains("ECOSYSTEM SIMULATION", output);
    }


    [Fact]
    public void WildlifeSanctuary_MatchesProtectedSpeciesIgnoringCase()
    {
        var forest = new Forest(10, "Forest");
        var sanctuary = new WildlifeSanctuary("Sanctuary", new[] { "FOX" }, new List<NaturalArea> { forest });
        Assert.False(sanctuary.IsHuntingAllowed("Fox"));
        Assert.True(sanctuary.IsHuntingAllowed("Deer"));
    }

    [Fact]
    public void ProtectedArea_AddAreaRejectsNull()
    {
        var forest = new Forest(10, "Forest");
        var reserve = new NatureReserve("Reserve", new List<NaturalArea> { forest });
        Assert.Throws<ArgumentNullException>(() => reserve.AddArea(null!));
    }

    [Fact]
    public void ProtectedArea_AddAreaDoesNotAddSameReferenceTwice()
    {
        var forest = new Forest(10, "Forest");
        var reserve = new NatureReserve("Reserve", new List<NaturalArea> { forest });
        reserve.AddArea(forest);
        Assert.Single(reserve.Areas);
    }
}

public class NaturalAreaAdditionalTests
{
    [Fact]
    public void CanContainOrganism_IsCaseInsensitiveAndTrimsName()
    {
        var forest = new Forest(10, "Forest");
        Assert.True(forest.CanContainOrganism(new Grass(" grass ")));
    }

    [Fact]
    public void RemoveCreature_ReturnsFalseForUnknownOrganism()
    {
        var forest = new Forest(10, "Forest");
        Assert.False(forest.RemoveCreature(new Grass()));
    }

    [Fact]
    public void AddNeighbor_DoesNotDuplicateExistingNeighbor()
    {
        var forest = new Forest(10, "Forest");
        var lake = new Lake(10, "Lake");
        forest.AddNeighbor(lake);
        forest.AddNeighbor(lake);
        Assert.Single(forest.Neighbors);
        Assert.Single(lake.Neighbors);
    }

    [Theory]
    [InlineData(typeof(Forest), PollutionType.Air, PollutionType.Soil)]
    [InlineData(typeof(Lake), PollutionType.Water, PollutionType.Soil)]
    [InlineData(typeof(River), PollutionType.Water, PollutionType.Soil)]
    [InlineData(typeof(Sea), PollutionType.Water, null)]
    [InlineData(typeof(Mountain), PollutionType.Air, PollutionType.Soil)]
    [InlineData(typeof(Plains), PollutionType.Air, PollutionType.Soil)]
    public void AreaConstructor_InitializesExpectedPollutionParameters(Type areaType, PollutionType first, PollutionType? second)
    {
        var area = (NaturalArea)Activator.CreateInstance(areaType, 10d, "Area")!;
        Assert.True(area.Pollution.HasParameter(first));
        if (second.HasValue) Assert.True(area.Pollution.HasParameter(second.Value));
    }
}

public class ClimateAdditionalTests
{
    [Fact]
    public void AreaClimate_StoresAllProvidedBounds()
    {
        var climate = new AreaClimate(-12, 27, 15, 85, 5, 95);
        Assert.Equal(-12, climate.minTemperature);
        Assert.Equal(27, climate.maxTemperature);
        Assert.Equal(15, climate.minPrecipitation);
        Assert.Equal(85, climate.maxPrecipitation);
        Assert.Equal(5, climate.minCloudCover);
        Assert.Equal(95, climate.maxCloudCover);
    }
}
