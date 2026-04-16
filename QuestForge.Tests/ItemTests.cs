using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class ItemTests
{
    [Fact]
    public void ToString_ContainsNameAndCategory()
    {
        var item = new Item("Sword", "Sharp", 5f, 25, Category.Weapon, Rarity.Common);
        var str = item.ToString();
        Assert.Contains("Sword", str);
        Assert.Contains("Weapon", str);
    }

    [Theory]
    [InlineData(Rarity.Common)]
    [InlineData(Rarity.Uncommon)]
    [InlineData(Rarity.Rare)]
    [InlineData(Rarity.Legendary)]
    public void MakeLoot_ReturnsItemOfCorrectRarity(Rarity rarity)
    {
        var item = Item.MakeLoot(rarity);
        Assert.Equal(rarity, item.Rarity);
    }

    [Fact]
    public void MakeLoot_ReturnsNonNullItem()
    {
        Assert.NotNull(Item.MakeLoot(Rarity.Common));
    }
}