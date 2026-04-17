using QuestForge.Engine.Models;
using Xunit;
namespace QuestForge.Tests;

public class PlayerTests
{
    private Player MakePlayer() => new("Aria", health: 85, attack: 20, defence: 10);
    private Item MakeItem(string name = "Potion") =>
        new(name, "test", 1f, 10, Category.Consumable, Rarity.Common);

    [Fact]
    public void ToString_ContainsName() =>
        Assert.Contains("Aria", MakePlayer().ToString());

    [Fact]
    public void AddItem_ReturnsTrueWhenSpace()
    {
        var player = MakePlayer();
        Assert.True(player.AddItemToInventory(MakeItem()));
    }

    [Fact]
    public void AddItem_ReturnsFalseWhenFull()
    {
        var player = MakePlayer();
        for (int i = 0; i < 20; i++) player.AddItemToInventory(MakeItem($"Item{i}"));
        Assert.False(player.AddItemToInventory(MakeItem("one too many")));
    }

    [Fact]
    public void AddItem_IncrementsCount()
    {
        var player = MakePlayer();
        player.AddItemToInventory(MakeItem());
        Assert.Single(player.Inventory);
    }

    [Fact]
    public void RemoveItem_ReturnsTrueWhenExists()
    {
        var player = MakePlayer();
        var item = MakeItem();
        player.AddItemToInventory(item);
        Assert.True(player.RemoveItemFromInventory(item));
    }

    [Fact]
    public void RemoveItem_ReturnsFalseWhenMissing()
    {
        Assert.False(MakePlayer().RemoveItemFromInventory(MakeItem()));
    }

    [Fact]
    public void RemoveItem_RemovesFirstNameMatch()
    {
        var player = MakePlayer();
        player.AddItemToInventory(MakeItem("Potion"));
        player.AddItemToInventory(MakeItem("Potion"));
        player.RemoveItemFromInventory(MakeItem("Potion"));
        Assert.Single(player.Inventory);
    }

    [Fact]
    public void FindByName_ReturnsMatches()
    {
        var player = MakePlayer();
        player.AddItemToInventory(MakeItem("Potion"));
        player.AddItemToInventory(MakeItem("Super Potion"));
        player.AddItemToInventory(MakeItem("Sword"));
        Assert.Equal(2, player.FindItemByName("Potion").Count);
    }

    [Fact]
    public void FindByName_ReturnsEmptyWhenNoMatch()
    {
        var player = MakePlayer();
        player.AddItemToInventory(MakeItem("Sword"));
        Assert.Empty(player.FindItemByName("Potion"));
    }

    [Fact]
    public void FindByName_IsCaseInsensitive()
    {
        var player = MakePlayer();
        player.AddItemToInventory(MakeItem("Potion"));
        Assert.Single(player.FindItemByName("potion"));
    }
}