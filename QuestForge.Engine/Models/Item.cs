namespace QuestForge.Engine.Models;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public enum Category
{
    Weapon,
    Armour,
    Consumable,
    QuestItem
}

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Weight { get; set; }
    public int Value { get; set; }
    public Category Category { get; set; }
    public Rarity Rarity { get; set; }

    public Item(string name, string description, float weight, int value, Category category, Rarity rarity)
    {
        Name = name;
        Description = description;
        Weight = weight;
        Value = value;
        Category = category;
        Rarity = rarity;
    }

public static Item MakeLoot(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common    => CommonPool[Random.Shared.Next(CommonPool.Length)],
            Rarity.Uncommon  => UncommonPool[Random.Shared.Next(UncommonPool.Length)],
            Rarity.Rare      => RarePool[Random.Shared.Next(RarePool.Length)],
            Rarity.Legendary => LegendaryPool[Random.Shared.Next(LegendaryPool.Length)],
            _ => throw new ArgumentOutOfRangeException(nameof(rarity))
        };
    }
    private static readonly Item[] CommonPool =
    [
        new("Potion", "Recovers 20 HP", 1f, 10, Category.Consumable, Rarity.Common),
        new("Sword", "Increases Attack by 10", 5f, 25, Category.Weapon, Rarity.Common),
        new("Dagger", "Light, quick weapon", 2f, 15, Category.Weapon, Rarity.Common),
    ];

    private static readonly Item[] UncommonPool =
    [
        new("Shield", "Increases Defence by 5", 6f, 50, Category.Armour, Rarity.Uncommon),
        new("Chain Mail","Solid body armour", 8f, 80, Category.Armour, Rarity.Uncommon),
    ];

    private static readonly Item[] RarePool =
    [
        new("Elixir", "Recovers all HP", 1f, 200, Category.Consumable, Rarity.Rare),
        new("Flame Sword", "Burns enemies", 5f, 300, Category.Weapon, Rarity.Rare),
    ];

    private static readonly Item[] LegendaryPool =
    [
        new("Excalibur", "The legendary blade", 4f,  999, Category.Weapon, Rarity.Legendary),
        new("Dragon Scale Armour", "Near invincible", 12f, 999, Category.Armour, Rarity.Legendary),
    ];

    public override string ToString()
    {
        return $"{Name} [{Category}/{Rarity}] - {Description} | Wt:{Weight} Val:{Value}";
    }
}