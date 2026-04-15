using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class ItemTests
{
    [Fact]
    public void ToString_ContainsNameAndCategory()
    {
        var item = new Item("Sword", "Sharp", 5f, 25, Category.Weapon, Rarity.Common);
        Assert.Contains("Sword", item.ToString());
        Assert.Contains("Weapon", item.ToString());
    }

    [Theory]
    [InlineData(Rarity.Common)]
    [InlineData(Rarity.Uncommon)]
    [InlineData(Rarity.Rare)]
    [InlineData(Rarity.Legendary)]
    public void MakeLoot_ReturnsCorrectRarity(Rarity rarity)
    {
        var item = Item.MakeLoot(rarity);
        Assert.Equal(rarity, item.Rarity);
    }

    [Fact]
    public void MakeLoot_ReturnsNonNull()
    {
        Assert.NotNull(Item.MakeLoot(Rarity.Common));
    }
}