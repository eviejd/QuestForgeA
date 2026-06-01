using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

namespace QuestForge.Tests;

public class Phase3Tests
{
    private (Player player, ZoneManager zoneManager) Setup()
    {
        var zoneManager = new ZoneManager();

        var townZone = new Zone("Town", "start", 1);
        var outskirtsZone = new Zone("Outskirts", "mid", 2);

        zoneManager.AddZone(townZone, null, null);
        zoneManager.AddZone(outskirtsZone, townZone, null);
        zoneManager.SetCurrentZone(townZone);

        var player = new Player("Aria", 100, 20, 5);
        player.CurrentZone = "Town";

        return (player, zoneManager);
    }

    [Fact]
    public void CanMove_TrueForAdjacent()
    {
        var (player, zoneManager) = Setup();
        Assert.True(player.CanMove(zoneManager, "Outskirts"));
    }

    [Fact]
    public void CanMove_FalseForNonAdjacent()
    {
        var zoneManager = new ZoneManager();

        var zoneA = new Zone("A", "", 1);
        var zoneB = new Zone("B", "", 1);
        var zoneC = new Zone("C", "", 1);

        zoneManager.AddZone(zoneA, null, null);
        zoneManager.AddZone(zoneB, zoneA, null);
        zoneManager.AddZone(zoneC, zoneB, null);

        zoneManager.SetCurrentZone(zoneA);

        var player = new Player("Aria");
        player.CurrentZone = "A";

        Assert.False(player.CanMove(zoneManager, "C"));
    }

    [Fact]
    public void CanMove_FalseForUnknownZone()
    {
        var (player, zoneManager) = Setup();
        Assert.False(player.CanMove(zoneManager, "Narnia"));
    }

    [Fact]
    public void CanMove_FalseWhenRequiredItemMissing()
    {
        var (player, zoneManager) = Setup();

        var requiredKeyItem = new Item("Key", "", 0.5f, 0, Category.QuestItem, Rarity.Uncommon);

        zoneManager.AddRequirement(
            zoneManager.GetZone("Outskirts")!,
            new List<GameEvent>(),
            new List<Item> { requiredKeyItem }
        );

        Assert.False(player.CanMove(zoneManager, "Outskirts"));
    }

    [Fact]
    public void CanMove_TrueWhenRequiredItemPresent()
    {
        var (player, zoneManager) = Setup();

        var requiredKeyItem = new Item("Key", "", 0.5f, 0, Category.QuestItem, Rarity.Uncommon);

        zoneManager.AddRequirement(
            zoneManager.GetZone("Outskirts")!,
            new List<GameEvent>(),
            new List<Item> { requiredKeyItem }
        );

        player.AddItemToInventory(requiredKeyItem);

        Assert.True(player.CanMove(zoneManager, "Outskirts"));
    }

    [Fact]
    public void CanMove_FalseWhenEventNotCleared()
    {
        var (player, zoneManager) = Setup();

        var combatEvent = new GameEvent(EventType.Combat, "guard");

        zoneManager.AddRequirement(
            zoneManager.GetZone("Outskirts")!,
            new List<GameEvent> { combatEvent },
            new List<Item>()
        );

        Assert.False(player.CanMove(zoneManager, "Outskirts"));
    }

    [Fact]
    public void CanMove_TrueWhenEventCleared()
    {
        var (player, zoneManager) = Setup();

        var combatEvent = new GameEvent(EventType.Combat, "guard");

        zoneManager.AddRequirement(
            zoneManager.GetZone("Outskirts")!,
            new List<GameEvent> { combatEvent },
            new List<Item>()
        );

        player.ClearEvent(combatEvent);

        Assert.True(player.CanMove(zoneManager, "Outskirts"));
    }

    [Fact]
    public void Interrupt_PotionHealsPlayer()
    {
        var (player, zoneManager) = Setup();

        player.Health = 50;

        var healingPotion = new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common);
        player.AddItemToInventory(healingPotion);

        Assert.True(player.Interrupt(zoneManager, healingPotion));
        Assert.Equal(70, player.Health);
    }

    [Fact]
    public void Interrupt_ElixirFullyHealsPlayer()
    {
        var (player, zoneManager) = Setup();

        player.Health = 1;

        var elixirItem = new Item("Elixir", "", 1f, 100, Category.Consumable, Rarity.Rare);
        player.AddItemToInventory(elixirItem);

        Assert.True(player.Interrupt(zoneManager, elixirItem));
        Assert.Equal(100, player.Health);
    }

    [Fact]
    public void Interrupt_ArmourIncreasesDefence()
    {
        var (player, zoneManager) = Setup();

        int initialDefence = player.Defence;

        var shieldItem = new Item("Shield", "", 6f, 50, Category.Armour, Rarity.Uncommon);
        player.AddItemToInventory(shieldItem);

        Assert.True(player.Interrupt(zoneManager, shieldItem));
        Assert.True(player.Defence > initialDefence);
    }

    [Fact]
    public void Interrupt_LimitedToOncePerZone()
    {
        var (player, zoneManager) = Setup();

        var potionOne = new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common);
        var potionTwo = new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common);

        player.AddItemToInventory(potionOne);
        player.AddItemToInventory(potionTwo);

        player.Interrupt(zoneManager, potionOne);

        Assert.False(player.Interrupt(zoneManager, potionTwo));
    }

    [Fact]
    public void Interrupt_ResetsOnZoneChange()
    {
        var (player, zoneManager) = Setup();

        var potionItem = new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common);

        player.AddItemToInventory(potionItem);
        player.Interrupt(zoneManager, potionItem);

        player.MovePlayer(zoneManager, "Outskirts");

        Assert.False(player.HasUsedInterrupt);
    }

    [Fact]
    public void Interrupt_FailsForNonConsumableItem()
    {
        var (player, zoneManager) = Setup();

        var swordItem = new Item("Sword", "", 5f, 25, Category.Weapon, Rarity.Common);
        player.AddItemToInventory(swordItem);

        Assert.False(player.Interrupt(zoneManager, swordItem));
    }

    [Fact]
    public void Interrupt_FailsWhenItemNotInInventory()
    {
        var (player, zoneManager) = Setup();

        var missingPotion = new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common);

        Assert.False(player.Interrupt(zoneManager, missingPotion));
    }

    [Fact]
    public void AddScore_AccumulatesCorrectly()
    {
        var player = new Player("Aria");

        player.AddScore(10);
        player.AddScore(25);

        Assert.Equal(35, player.Score);
    }

    [Fact]
    public void AddRequirement_ThrowsWhenBothListsEmpty()
    {
        var zoneManager = new ZoneManager();
        var zone = new Zone("Town", "", 1);

        zoneManager.AddZone(zone, null, null);

        Assert.Throws<ArgumentException>(() =>
            zoneManager.AddRequirement(zone, new List<GameEvent>(), new List<Item>())
        );
    }

    [Fact]
    public void BeginCombat_PlayerWins_ReturnsLoot()
    {
        var combatManager = new CombatManager();

        var player = new Player("Aria", 100, 50, 5);
        var enemy = new Enemy("Goblin", 10, 1, 0, Difficulty.Easy);

        Assert.Equal(EventType.Loot, combatManager.BeginCombat(player, enemy)?.Type);
    }

    [Fact]
    public void BeginCombat_PlayerLoses_ReturnsGameOver()
    {
        var combatManager = new CombatManager();

        var player = new Player("Aria", 1, 1, 0);
        var enemy = new Enemy("Dragon", 1000, 999, 0, Difficulty.Boss);

        Assert.Equal("Game Over", combatManager.BeginCombat(player, enemy)?.Description);
    }

    [Fact]
    public void BeginCombat_ThrowsWhenTwoEnemiesProvided()
    {
        var combatManager = new CombatManager();

        var enemyOne = new Enemy("A", 10, 5, 0, Difficulty.Easy);
        var enemyTwo = new Enemy("B", 10, 5, 0, Difficulty.Easy);

        Assert.Throws<ArgumentException>(() => combatManager.BeginCombat(enemyOne, enemyTwo));
    }

    [Fact]
    public void BeginCombat_BothFleeResultsInDraw()
    {
        var combatManager = new CombatManager();

        var player = new Player("Aria", 100, 20, 5);
        var enemy = new Enemy("Goblin", 100, 8, 3, Difficulty.Easy);

        combatManager.QueueCombatAction(new CombatAction("Flee", 0, player), player);
        combatManager.QueueCombatAction(new CombatAction("Flee", 0, enemy), enemy);
        combatManager.QueueRoundOver(player);

        var result = combatManager.PlayCombatRound(player, enemy);

        Assert.Contains("fled", result?.Description);
    }

    [Fact]
    public void GetLog_ContainsCombatDetails()
    {
        var combatManager = new CombatManager();

        var player = new Player("Aria", 100, 50, 5);
        var enemy = new Enemy("Goblin", 10, 1, 0, Difficulty.Easy);

        combatManager.BeginCombat(player, enemy);

        var log = combatManager.GetLog();

        Assert.Contains("Goblin", log);
        Assert.Contains("Combat", log);
    }

    [Fact]
    public void ApplyCombatScore_EasyDifficultyGivesOnePoint()
    {
        var gameManager = new GameManager();
        var player = new Player("Aria");

        gameManager.Register(player);
        gameManager.ApplyCombatScore(Difficulty.Easy);

        Assert.Equal(GameManager.ScoreEasy, player.Score);
    }

    [Fact]
    public void ApplyCombatScore_HardDifficultyGivesFivePoints()
    {
        var gameManager = new GameManager();
        var player = new Player("Aria");

        gameManager.Register(player);
        gameManager.ApplyCombatScore(Difficulty.Hard);

        Assert.Equal(GameManager.ScoreHard, player.Score);
    }

    [Fact]
    public void ApplyCombatScore_BossDifficultyGivesTenPoints()
    {
        var gameManager = new GameManager();
        var player = new Player("Aria");

        gameManager.Register(player);
        gameManager.ApplyCombatScore(Difficulty.Boss);

        Assert.Equal(GameManager.ScoreBoss, player.Score);
    }
}