using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class PlayerTests
{
    private Player MakePlayer() => new("Aria", health: 85, attack: 20, defence: 10);
    private Item MakeItem(string name = "Potion") =>
        new(name, "test item", 1f, 10, Category.Consumable, Rarity.Common);

    [Fact]
    public void ToString_ContainsName()
    {
        Assert.Contains("Aria", MakePlayer().ToString());
    }

    [Fact]
    public void AddItem_ReturnsTrueWhenSpace()
    {
        Assert.True(MakePlayer().AddItemToInventory(MakeItem()));
    }

    [Fact]
    public void AddItem_ReturnsFalseWhenFull()
    {
        var p = MakePlayer();
        for (int i = 0; i < 20; i++) p.AddItemToInventory(MakeItem($"Item{i}"));
        Assert.False(p.AddItemToInventory(MakeItem("overflow")));
    }

    [Fact]
    public void AddItem_IncrementsInventoryCount()
    {
        var p = MakePlayer();
        p.AddItemToInventory(MakeItem());
        Assert.Single(p.Inventory);
    }

    [Fact]
    public void RemoveItem_ReturnsTrueWhenFound()
    {
        var p = MakePlayer();
        var item = MakeItem();
        p.AddItemToInventory(item);
        Assert.True(p.RemoveItemFromInventory(item));
    }

    [Fact]
    public void RemoveItem_ReturnsFalseWhenNotFound()
    {
        Assert.False(MakePlayer().RemoveItemFromInventory(MakeItem()));
    }

    [Fact]
    public void RemoveItem_MatchesByNameWhenNoReference()
    {
        var p = MakePlayer();
        p.AddItemToInventory(MakeItem("Potion"));
        p.AddItemToInventory(MakeItem("Potion"));
        p.RemoveItemFromInventory(MakeItem("Potion"));
        Assert.Single(p.Inventory);
    }

    [Fact]
    public void FindByName_ReturnsAllMatches()
    {
        var p = MakePlayer();
        p.AddItemToInventory(MakeItem("Potion"));
        p.AddItemToInventory(MakeItem("Super Potion"));
        p.AddItemToInventory(MakeItem("Sword"));
        Assert.Equal(2, p.FindItemByName("Potion").Count);
    }

    [Fact]
    public void FindByName_ReturnsEmptyWhenNoMatch()
    {
        var p = MakePlayer();
        p.AddItemToInventory(MakeItem("Sword"));
        Assert.Empty(p.FindItemByName("Potion"));
    }

    [Fact]
    public void FindByName_CaseInsensitive()
    {
        var p = MakePlayer();
        p.AddItemToInventory(MakeItem("Potion"));
        Assert.Single(p.FindItemByName("POTION"));
    }
}