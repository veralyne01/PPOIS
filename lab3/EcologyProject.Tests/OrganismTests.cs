using EcologyProject;
using Xunit;
namespace EcologyProject.Tests;

public class OrganismTests
{
    [Fact]
    public void Organism_TrimsNameAndFormatsToString()
    {
        var grass = new Grass(" Grass ");
        Assert.Equal("Grass", grass.Name);
        Assert.Equal("Grass (Plant)", grass.ToString());
        Assert.True(grass.IsAlive);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Organism_RejectsEmptyName(string name) =>
        Assert.Throws<ArgumentException>(() => new Grass(name));

    [Fact]
    public void MarkAsHunted_KillsOrganismAndSecondKillThrows()
    {
        var grass = new Grass();
        grass.MarkAsHunted();
        Assert.False(grass.IsAlive);
        Assert.Throws<OrganismAlreadyDeadException>(() => grass.MarkAsHunted());
    }

    public class OrganismCatalogTests
    {
        [Theory]
        [InlineData("Grass", typeof(Grass))]
        [InlineData("Shrub", typeof(Shrub))]
        [InlineData("Oak", typeof(Tree))]
        [InlineData("Lily", typeof(Flower))]
        [InlineData("Spirogyra", typeof(Algae))]
        [InlineData("AquaticPlant", typeof(AquaticPlant))]
        [InlineData("Fern", typeof(Plant))]
        public void Create_ReturnsCorrectPlantType(string name, Type expectedType)
        {
            var organism = OrganismCatalog.Create(name);

            Assert.IsType(expectedType, organism);
            Assert.Equal(name, organism.Name);
            Assert.True(organism.IsAlive);
        }

        [Theory]
        [InlineData("Fly Agaric", typeof(Mushroom))]
        [InlineData("Chanterelle", typeof(EdibleMushroom))]
        [InlineData("White mushroom", typeof(EdibleMushroom))]
        public void Create_ReturnsCorrectMushroomType(string name, Type expectedType)
        {
            var organism = OrganismCatalog.Create(name);

            Assert.IsType(expectedType, organism);
            Assert.Equal("Mushroom", organism.Type);
        }

        [Theory]
        [InlineData("Grass")]
        [InlineData("Oak")]
        [InlineData("Blueberry")]
        [InlineData("Spirogyra")]
        [InlineData("Reed")]
        public void IsPlant_ReturnsTrueForPlantNames(string name)
        {
            Assert.True(OrganismCatalog.IsPlant(name));
        }

        [Theory]
        [InlineData("Fox")]
        [InlineData("Trout")]
        [InlineData("Owl")]
        [InlineData("Wolf")]
        public void IsPlant_ReturnsFalseForAnimalNames(string name)
        {
            Assert.False(OrganismCatalog.IsPlant(name));
        }

        [Theory]
        [InlineData("Fly Agaric")]
        [InlineData("Chanterelle")]
        [InlineData("White mushroom")]
        public void IsMushroom_ReturnsTrueForMushroomNames(string name)
        {
            Assert.True(OrganismCatalog.IsMushroom(name));
        }

        [Theory]
        [InlineData("Fox")]
        [InlineData("Grass")]
        [InlineData("Trout")]
        public void IsMushroom_ReturnsFalseForOtherOrganisms(string name)
        {
            Assert.False(OrganismCatalog.IsMushroom(name));
        }
    }

    public class AnimalEdgeCaseTests
    {
        [Fact]
        public void Eat_ThrowsWhenPredatorIsDead()
        {
            var fox = new Fox();
            fox.MarkAsHunted();

            Assert.Throws<OrganismAlreadyDeadException>(
                () => fox.Eat(new Mammal("Hare", MammalTypes.artiodactyl)));
        }

        [Fact]
        public void Eat_RejectsAlreadyDeadPrey()
        {
            var fox = new Fox();
            var hare = new Mammal("Hare", MammalTypes.artiodactyl);

            hare.MarkAsHunted();
            fox.IncreaseHunger(60);

            Assert.Throws<WrongVictimException>(() => fox.Eat(hare));
        }

        [Fact]
        public void TryEat_ReturnsFalseWhenAnimalIsNotHungry()
        {
            var forest = new Forest(10, "Forest");
            var fox = new Fox();

            forest.AddCreature(fox);
            forest.AddCreature(new Mammal("Hare", MammalTypes.artiodactyl));

            Assert.False(fox.TryEat(forest));
            Assert.True(forest.Organisms[1].IsAlive);
        }

        [Fact]
        public void TryEat_ThrowsWhenAnimalIsDead()
        {
            var fox = new Fox();
            fox.MarkAsHunted();

            Assert.Throws<OrganismAlreadyDeadException>(
                () => fox.TryEat(new Forest(10, "Forest")));
        }

        [Fact]
        public void MarkReproduced_RejectsAnimalWithHighHunger()
        {
            var fox = new Fox();
            var forest = new Forest(100, "Forest");

            forest.AddCreature(fox);

            for (int i = 0; i < 10; i++)
                forest.AddCreature(new Mammal("Hare", MammalTypes.artiodactyl));

            for (int i = 0; i < 30; i++)
                fox.LiveDay(forest);

            fox.IncreaseHunger(60);

            Assert.True(fox.IsMature);
            Assert.False(fox.CanReproduce);
            Assert.Throws<InvalidOperationException>(() => fox.MarkReproduced());
        }

        [Fact]
        public void LiveDay_IncreasesAgeAndHunger()
        {
            var fox = new Fox();
            fox.LiveDay(new Forest(10, "Forest"));

            Assert.Equal(1, fox.AgeDays);
            Assert.Equal(20, fox.Hunger);
            Assert.True(fox.IsAlive);
        }
    }

    [Theory]
    [InlineData("Oak", true)]
    [InlineData("Grass", true)]
    [InlineData("Reed", true)]
    [InlineData("Wolf", false)]
    public void IsPlant_ClassifiesNames(string name, bool expected) =>
        Assert.Equal(expected, OrganismCatalog.IsPlant(name));

    [Theory]
    [InlineData("White mushroom", true)]
    [InlineData("Chanterelle", true)]
    [InlineData("Wolf", false)]
    public void IsMushroom_ClassifiesNames(string name, bool expected) =>
        Assert.Equal(expected, OrganismCatalog.IsMushroom(name));

    [Theory]
    [InlineData("Deer", true)]
    [InlineData("Beaver", true)]
    [InlineData("Wolf", false)]
    public void IsHerbivore_ClassifiesNames(string name, bool expected) =>
        Assert.Equal(expected, OrganismCatalog.IsHerbivore(name));

    [Theory]
    [InlineData("Wolf", true)]
    [InlineData("Eagle", true)]
    [InlineData("Deer", false)]
    public void IsPredator_ClassifiesNames(string name, bool expected) =>
        Assert.Equal(expected, OrganismCatalog.IsPredator(name));
}