namespace EcologyProject;

public static class OrganismCatalog
{
    private static readonly HashSet<string> plantNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Birch", "Oak", "Pine", "Spruce", "Maple", "Aspen", "Lily", "Fern", "Blueberry",
        "Lingonberry", "Kovyl", "Tipchak", "Wormwood", "Clover", "Chamomile", "Cornflower",
        "Poppy", "Reed", "Cattail", "Water lily", "Cup", "Duckweed", "Spirogyra", "Chlorella",
        "Laminaria", "Fucus", "Ulva", "Zostera", "Edelweiss", "Gentian", "Alpine poppy",
        "Mountain Pine", "Juniper", "Mosses", "Lichens", "Cladonia", "Parmelia", "Grass", "Shrub", "AquaticPlant", "Cattail", "Cup"
    };

    private static readonly HashSet<string> mushroomNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "White mushroom", "Chanterelle", "Fly Agaric"
    };

    private static readonly HashSet<string> birdNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Woodpecker", "Owl", "Cuckoo", "Titmouse", "Capercaillie", "Grey Heron", "Mallard Duck",
        "Hissing Swan", "Kingfisher", "Golden Eagle", "Albatross", "Seagull", "Cormorant",
        "Penguin", "Petrel", "Steppe Eagle", "Lark", "Quail", "Grebe", "Reed Warbler", "Beauty Crane", "Grey partridge", "Bustard", "Ular", "Alpine Jackdaw", "Bearded"
    };

    private static readonly HashSet<string> fishNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pike", "Crucian carp", "Perch", "Roach", "Bream", "Tench", "Carp", "Walleye", "Trout",
        "Catfish", "Chub", "Ide", "Asp", "Shark", "Tuna", "Cod", "Flounder", "Mackerel"
    };

    private static readonly HashSet<string> insectNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Ant", "Stag beetle", "Dragonfly", "Vodomerka", "Floating beetle", "Mosquito", "Locust",
        "Grasshopper", "Mantis", "Bumblebee", "Mayfly", "Brook", "Mountain bumblebee", "Alpine butterfly",
        "Cross spider", "Tarantula", "Karakurt", "Water Strider"
    };

    public static Organism Create(string name, IEnumerable<string>? availableSpecies = null)
    {
        string[] available = (availableSpecies ?? Array.Empty<string>()).Where(item =>
            !item.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
        string[] plants = available.Where(IsPlant).ToArray();
        string[] animals = available.Where(item => !IsPlant(item) && !IsMushroom(item)).ToArray();
        string[] insects = available.Where(IsInsect).ToArray();
        string[] aquaticFood = available.Where(item => IsPlant(item) || IsFish(item) || IsCrustacean(item)).ToArray();
        string[] food;

        if (IsPlant(name)) return CreatePlant(name);
        if (IsMushroom(name)) return name.Contains("Fly Agaric", StringComparison.OrdinalIgnoreCase) ? new Mushroom(name) : new EdibleMushroom(name);

        bool herbivore = IsHerbivore(name);
        bool predator = IsPredator(name);
        bool amphibian = name.Contains("frog", StringComparison.OrdinalIgnoreCase) || name.Contains("toad", StringComparison.OrdinalIgnoreCase) || name.Contains("newt", StringComparison.OrdinalIgnoreCase) || name.Contains("salamander", StringComparison.OrdinalIgnoreCase);
        bool reptile = name.Contains("snake", StringComparison.OrdinalIgnoreCase) || name.Contains("viper", StringComparison.OrdinalIgnoreCase) || name.Contains("lizard", StringComparison.OrdinalIgnoreCase) || name.Contains("turtle", StringComparison.OrdinalIgnoreCase);
        bool mollusk = name.Contains("octopus", StringComparison.OrdinalIgnoreCase) || name.Contains("squid", StringComparison.OrdinalIgnoreCase) || name.Contains("mussel", StringComparison.OrdinalIgnoreCase) || name.Contains("oyster", StringComparison.OrdinalIgnoreCase) || name.Contains("toothless", StringComparison.OrdinalIgnoreCase) || name.Contains("pearl barley", StringComparison.OrdinalIgnoreCase) || name.Contains("coil", StringComparison.OrdinalIgnoreCase) || name.Contains("scallop", StringComparison.OrdinalIgnoreCase);
        if (herbivore || (IsInsect(name) && !predator)) food = plants;
        else if (amphibian || reptile) food = animals.Concat(insects).ToArray();
        else if (mollusk || IsCrustacean(name)) food = aquaticFood;
        else if (predator) food = animals.Concat(insects).ToArray();
        else food = plants.Concat(animals).Concat(insects).ToArray();

        if (food.Length == 0)
            food = plants.Concat(animals).Concat(insects).ToArray();

        if (name.Equals("Bear", StringComparison.OrdinalIgnoreCase) || name.Equals("Brown bear", StringComparison.OrdinalIgnoreCase))
            return new Bear(name, food);
        if (name.Equals("Wolf", StringComparison.OrdinalIgnoreCase)) return new Wolf(name, food);
        if (name.Equals("Fox", StringComparison.OrdinalIgnoreCase) || name.Equals("Common fox", StringComparison.OrdinalIgnoreCase)) return new Fox(name, food);
        if (name.Equals("Deer", StringComparison.OrdinalIgnoreCase) || name.Equals("Red deer", StringComparison.OrdinalIgnoreCase)) return new Deer(name, food);
        if (name.Equals("Beaver", StringComparison.OrdinalIgnoreCase)) return new Beaver(name, food);
        if (name.Equals("Eagle", StringComparison.OrdinalIgnoreCase) || name.Equals("Golden Eagle", StringComparison.OrdinalIgnoreCase)) return new Eagle(name, food);
        if (name.Equals("Owl", StringComparison.OrdinalIgnoreCase)) return new Owl(name, food);
        if (name.Equals("Duck", StringComparison.OrdinalIgnoreCase) || name.Equals("Mallard Duck", StringComparison.OrdinalIgnoreCase)) return new Duck(name, food);
        if (name.Equals("Woodpecker", StringComparison.OrdinalIgnoreCase)) return new Woodpecker(name, food);
        if (name.Equals("Pike", StringComparison.OrdinalIgnoreCase)) return new Pike(name, food);
        if (name.Equals("Trout", StringComparison.OrdinalIgnoreCase)) return new Trout(name, food);
        if (name.Equals("Shark", StringComparison.OrdinalIgnoreCase)) return new Shark(name, food);
        if (name.Contains("frog", StringComparison.OrdinalIgnoreCase) || name.Contains("toad", StringComparison.OrdinalIgnoreCase) || name.Contains("newt", StringComparison.OrdinalIgnoreCase) || name.Contains("salamander", StringComparison.OrdinalIgnoreCase))
            return new Frog(name, food);
        if (name.Contains("snake", StringComparison.OrdinalIgnoreCase) || name.Contains("viper", StringComparison.OrdinalIgnoreCase) || name.Contains("lizard", StringComparison.OrdinalIgnoreCase) || name.Contains("turtle", StringComparison.OrdinalIgnoreCase))
            return new Snake(name, food);
        if (IsCrustacean(name)) return new Crayfish(name, food);
        if (mollusk)
            return new Octopus(name, food);
        if (IsBird(name)) return new Bird(name, predator ? BirdTypes.predator : BirdTypes.passeriformes, food);
        if (IsFish(name)) return new Fish(name, name.Equals("Shark", StringComparison.OrdinalIgnoreCase) ? FishTypes.marine : FishTypes.freshwater, food.Length == 0 ? aquaticFood : food);
        if (IsInsect(name)) return new Insect(name, plants);
        if (name.Contains("crab", StringComparison.OrdinalIgnoreCase) || name.Contains("lobster", StringComparison.OrdinalIgnoreCase) || name.Contains("shrimp", StringComparison.OrdinalIgnoreCase))
            return new Crustacean(name, aquaticFood);
        if (name.Contains("mussel", StringComparison.OrdinalIgnoreCase) || name.Contains("oyster", StringComparison.OrdinalIgnoreCase) || name.Contains("octopus", StringComparison.OrdinalIgnoreCase) || name.Contains("squid", StringComparison.OrdinalIgnoreCase))
            return new Mollusk(name, aquaticFood);
        if (name.Contains("Dolphin", StringComparison.OrdinalIgnoreCase) || name.Contains("Whale", StringComparison.OrdinalIgnoreCase) || name.Contains("Killer whale", StringComparison.OrdinalIgnoreCase) || name.Contains("Seal", StringComparison.OrdinalIgnoreCase) || name.Contains("Walrus", StringComparison.OrdinalIgnoreCase))
            return new Mammal(name, MammalTypes.predator, food);
        return new Mammal(name, herbivore ? MammalTypes.artiodactyl : MammalTypes.predator, food);
    }

    private static Organism CreatePlant(string name)
    {
        if (name.Equals("Grass", StringComparison.OrdinalIgnoreCase)) return new Grass(name);
        if (name.Equals("Shrub", StringComparison.OrdinalIgnoreCase)) return new Shrub(name);
        if (name.Equals("AquaticPlant", StringComparison.OrdinalIgnoreCase)) return new AquaticPlant(name);
        if (new[] { "Birch", "Oak", "Pine", "Spruce", "Maple", "Aspen", "Mountain Pine", "Juniper" }.Contains(name.Trim(), StringComparer.OrdinalIgnoreCase)) return new Tree(name);
        if (new[] { "Lily", "Chamomile", "Cornflower", "Poppy", "Edelweiss", "Gentian", "Alpine poppy", "Blueberry", "Lingonberry" }.Contains(name.Trim(), StringComparer.OrdinalIgnoreCase)) return new Flower(name);
        if (new[] { "Spirogyra", "Chlorella", "Laminaria", "Fucus", "Ulva", "Zostera", "Duckweed" }.Contains(name.Trim(), StringComparer.OrdinalIgnoreCase)) return new Algae(name);
        return new Plant(name);
    }

    public static bool IsPlant(string name) => plantNames.Contains(name.Trim()) ||
        name.Contains("grass", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("reed", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("clover", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("moss", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("lichen", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("algae", StringComparison.OrdinalIgnoreCase);

    public static bool IsMushroom(string name) => mushroomNames.Contains(name) || name.Contains("mushroom", StringComparison.OrdinalIgnoreCase) || name.Contains("chanterelle", StringComparison.OrdinalIgnoreCase);
    private static bool IsBird(string name) => birdNames.Contains(name) || name.Contains("eagle", StringComparison.OrdinalIgnoreCase) || name.Contains("heron", StringComparison.OrdinalIgnoreCase) || name.Contains("swan", StringComparison.OrdinalIgnoreCase) || name.Contains("duck", StringComparison.OrdinalIgnoreCase) || name.Contains("seagull", StringComparison.OrdinalIgnoreCase);
    private static bool IsFish(string name) => fishNames.Contains(name) || name.Contains("carp", StringComparison.OrdinalIgnoreCase) || name.Contains("trout", StringComparison.OrdinalIgnoreCase) || name.Contains("pike", StringComparison.OrdinalIgnoreCase);
    private static bool IsInsect(string name) => insectNames.Contains(name) || name.Contains("butterfly", StringComparison.OrdinalIgnoreCase) || name.Contains("bee", StringComparison.OrdinalIgnoreCase) || name.Contains("beetle", StringComparison.OrdinalIgnoreCase);
    private static bool IsCrustacean(string name) => name.Contains("crab", StringComparison.OrdinalIgnoreCase) || name.Contains("crayfish", StringComparison.OrdinalIgnoreCase) || name.Contains("lobster", StringComparison.OrdinalIgnoreCase) || name.Contains("shrimp", StringComparison.OrdinalIgnoreCase);
    public static bool IsHerbivore(string name) => new[] { "deer", "hare", "beaver", "squirrel", "moose", "vole", "marmot", "goat", "bison", "saiga", "yak", "groundhog", "muskrat", "water vole", "swan" }.Any(value => name.Contains(value, StringComparison.OrdinalIgnoreCase));
    public static bool IsPredator(string name) => new[] { "wolf", "fox", "lynx", "bear", "eagle", "owl", "pike", "shark", "snake", "viper", "otter", "catfish", "kingfisher", "heron", "dolphin", "whale", "killer whale", "seal", "walrus", "dragonfly", "mantis", "tarantula", "karakurt", "spider", "leopard", "korsak", "ferret" }.Any(value => name.Contains(value, StringComparison.OrdinalIgnoreCase));
}
