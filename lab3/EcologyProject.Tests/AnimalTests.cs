using EcologyProject;
using Xunit;
namespace EcologyProject.Tests;

public class AnimalTests
{
    [Fact]
    public void IncreaseHunger_IncreasesAndCapsAtOneHundred()
    {
        var deer = new Deer();
        deer.IncreaseHunger(40);
        Assert.Equal(40, deer.Hunger);
        deer.IncreaseHunger(100);
        Assert.Equal(100, deer.Hunger);
    }

    [Fact]
    public void IncreaseHunger_RejectsNegativeAmount() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Deer().IncreaseHunger(-1));

    [Fact]
    public void Eat_ReturnsFalseWhenAnimalIsNotHungryEnough()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        var grass = new Grass();
        Assert.False(deer.Eat(grass));
        Assert.True(grass.IsAlive);
    }

    [Fact]
    public void Eat_ConsumesAllowedPreyAndReducesHunger()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        var grass = new Grass();
        deer.IncreaseHunger(60);
        Assert.True(deer.Eat(grass));
        Assert.False(grass.IsAlive);
        Assert.Equal(0, deer.Hunger);
    }

    [Fact]
    public void Eat_RejectsWrongVictim()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        deer.IncreaseHunger(60);
        Assert.Throws<WrongVictimException>(() => deer.Eat(new Shrub()));
    }

    [Fact]
    public void Eat_RejectsSelf()
    {
        var animal = new Deer("Deer", new[] { "Deer" });
        animal.IncreaseHunger(60);
        Assert.Throws<WrongVictimException>(() => animal.Eat(animal));
    }

    [Fact]
    public void TryEat_ConsumesFirstMatchingFood()
    {
        var area = new Forest(10, "Forest");
        var deer = new Deer("Deer", new[] { "Grass" });
        var grass = new Grass();
        area.AddCreature(deer);
        area.AddCreature(grass);
        deer.IncreaseHunger(60);
        Assert.True(deer.TryEat(area));
        Assert.False(grass.IsAlive);
    }

    [Fact]
    public void TryEat_ThrowsWhenFoodIsUnavailable()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        deer.IncreaseHunger(60);
        Assert.Throws<InsufficientFoodException>(() => deer.TryEat(new Forest(10, "Forest")));
    }

    [Fact]
    public void LiveDay_IncreasesAgeAndHunger()
    {
        var deer = new Deer("Deer", Array.Empty<string>());
        deer.LiveDay(new Forest(10, "Forest"));
        Assert.Equal(1, deer.AgeDays);
        Assert.Equal(20, deer.Hunger);
    }

    [Fact]
    public void LiveDay_StarvationKillsAnimal()
    {
        var deer = new Deer("Deer", Array.Empty<string>());
        for (int i = 0; i < 5; i++) deer.LiveDay(new Forest(10, "Forest"));
        Assert.False(deer.IsAlive);
    }

    [Fact]
    public void LiveDay_DoesNothingForDeadAnimal()
    {
        var deer = new Deer();
        deer.MarkAsHunted();
        deer.LiveDay(new Forest(10, "Forest"));
        Assert.Equal(0, deer.AgeDays);
    }

    [Fact]
    public void MatureAnimal_CanReproduceAndGetsCooldown()
    {
        var deer = new Deer("Deer", new[] { "Grass" });
        var area = new Forest(100, "Forest");
        area.AddCreature(deer);
        for (int i = 0; i < 10; i++) area.AddCreature(new Grass());
        for (int i = 0; i < 30; i++) deer.LiveDay(area);
        Assert.True(deer.IsMature);
        Assert.True(deer.CanReproduce);
        deer.MarkReproduced();
        Assert.Equal(30, deer.ReproductionCooldown);
        Assert.False(deer.CanReproduce);
    }

    [Fact]
    public void Trout_UsesFreshwaterCategoryAndDefaultDiet()
    {
        var trout = new Trout();

        Assert.Equal("Trout", trout.Name);
        Assert.Equal("Fish", trout.Type);
        Assert.Equal(FishTypes.freshwater, trout.Category);
        Assert.Contains("Crayfish", trout.Food);
        Assert.Contains("Small fish", trout.Food);
        Assert.Contains("Insect", trout.Food);
    }

    [Fact]
    public void Trout_AcceptsCustomNameAndDiet()
    {
        var trout = new Trout("Rainbow trout", new[] { "Worm", "Algae" });

        Assert.Equal("Rainbow trout", trout.Name);
        Assert.Contains("Worm", trout.Food);
        Assert.Contains("Algae", trout.Food);
        Assert.DoesNotContain("Crayfish", trout.Food);
    }

    [Fact]
    public void Fox_UsesPredatorCategoryAndDefaultDiet()
    {
        var fox = new Fox();

        Assert.Equal("Fox", fox.Name);
        Assert.Equal("Mammal", fox.Type);
        Assert.Equal(MammalTypes.predator, fox.Category);
        Assert.Contains("Hare", fox.Food);
        Assert.Contains("Mouse", fox.Food);
        Assert.Contains("Insect", fox.Food);
    }

    [Fact]
    public void Fox_AcceptsCustomNameAndDiet()
    {
        var fox = new Fox("Arctic fox", new[] { "Fish", "Bird" });

        Assert.Equal("Arctic fox", fox.Name);
        Assert.Contains("Fish", fox.Food);
        Assert.Contains("Bird", fox.Food);
        Assert.DoesNotContain("Hare", fox.Food);
    }

    [Fact]
    public void MarkReproduced_RejectsImmatureAnimal() =>
        Assert.Throws<InvalidOperationException>(() => new Deer().MarkReproduced());
}