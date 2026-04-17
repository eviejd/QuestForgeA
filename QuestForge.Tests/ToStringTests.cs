using QuestForge.Engine.Models;
using QuestForge.Engine.World;

namespace QuestForge.Tests;

public class ToStringTests
{
    [Fact]
    public void GameEntity_ToString_ContainsAllFields()
    {
        var player = new Player("Aria", 85, 20, 10);
        var str = player.ToString();
        Assert.Contains("Aria", str);
        Assert.Contains("85", str);
        Assert.Contains("20", str);
        Assert.Contains("10", str);
    }

    [Fact]
    public void Enemy_ToString_ContainsDifficultyAndActions()
    {
        var e = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        var str = e.ToString();
        Assert.Contains("Easy", str);
        Assert.Contains("Strike", str);
    }

    [Fact]
    public void Item_ToString_ContainsNameCategoryRarity()
    {
        var item = new Item("Sword", "Sharp", 5f, 25, Category.Weapon, Rarity.Common);
        var str = item.ToString();
        Assert.Contains("Sword", str);
        Assert.Contains("Weapon", str);
        Assert.Contains("Common", str);
    }

    [Fact]
    public void CombatAction_ToString_ContainsSourceAndName()
    {
        var player = new Player("Aria");
        var action = new CombatAction("Strike", 20, player);
        var str = action.ToString();
        Assert.Contains("Aria", str);
        Assert.Contains("Strike", str);
    }

    [Fact]
    public void Zone_ToString_ContainsNameAndDifficulty()
    {
        var zone = new Zone("Town", "A town", 1);
        var str = zone.ToString();
        Assert.Contains("Town", str);
        Assert.Contains("1", str);
    }

    [Fact]
    public void GameEvent_ToString_ContainsTypeAndDescription()
    {
        var e = new GameEvent(EventType.Combat, "A goblin attacks");
        var str = e.ToString();
        Assert.Contains("Combat", str);
        Assert.Contains("goblin", str);
    }
}