using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;
using Xunit;

namespace QuestForge.Tests;

public class CombatManagerTests
{
    private Player MakePlayer() => new("Aria", health: 100, attack: 20, defence: 5);
    private Enemy MakeEnemy(Difficulty d = Difficulty.Easy) =>
        new("Goblin", 30, 8, 3, d);

    [Fact]
    public void QueueAction_CanBeQueued()
    {
        var cm = new CombatManager();
        var player = MakePlayer();
        var action = new CombatAction("Strike", 20, player);
        Assert.True(cm.QueueCombatAction(action));
    }

    [Fact]
    public void PlayRound_ReturnsLootWhenEnemyDies()
    {
        var combatManager = new CombatManager();
        var player = MakePlayer();
        var enemy = MakeEnemy();

        combatManager.QueueCombatAction(new CombatAction("Strike", 20, player), player);
        combatManager.QueueCombatAction(new CombatAction("Strike", 8, enemy), enemy);
        combatManager.QueueCombatAction(new CombatAction("Flee", 0, player), player);
        var result = combatManager.PlayCombatRound(player, enemy);
        Assert.Equal(EventType.Loot, result?.Type);
    }

    [Fact]
    public void PlayRound_ReturnsGameOverWhenPlayerDies()
    {
        var combatManager = new CombatManager();
        var player = new Player("Aria", health: 5, attack: 1, defence: 0);
        var enemy = MakeEnemy();

        combatManager.QueueCombatAction(new CombatAction("Strike", 100, enemy), enemy);
        combatManager.QueueRoundOver(enemy);

        var result = combatManager.PlayCombatRound(player, enemy);
        Assert.Equal(EventType.Dialogue, result?.Type);
    }

    [Fact]
    public void PlayRound_AccountsForDefence()
    {
        var combatManager = new CombatManager();
        var player = new Player("Aria", health: 100, attack: 20, defence: 10);
        var enemy = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);

        combatManager.QueueCombatAction(new CombatAction("Strike", 8, enemy), enemy);
        combatManager.QueueRoundOver(enemy);

        combatManager.PlayCombatRound(player, enemy);
        Assert.Equal(100, player.Health);
    }

    [Fact]
    public void PlayRound_RoundOverBreaksLoop()
    {
        var combatManager = new CombatManager();
        var player = MakePlayer();
        var enemy = MakeEnemy();

        combatManager.QueueRoundOver(player);
        combatManager.QueueCombatAction(new CombatAction("Strike", 999, player), player);

        combatManager.PlayCombatRound(player, enemy);
        Assert.True(enemy.IsAlive);
    }

    [Fact]
    public void Flee_EndsCombat()
    {
        var combatManager = new CombatManager();
        var player = MakePlayer();
        var enemy = MakeEnemy();

        combatManager.QueueCombatAction(new CombatAction("Flee", 0, player), player);
        var result = combatManager.PlayCombatRound(player, enemy);

        Assert.Equal(EventType.Dialogue, result?.Type);
        Assert.Contains("fled", result?.Description);
    }

    [Fact]
    public void GetLog_ContainsEntries()
    {
        var combatManager = new CombatManager();
        var player = MakePlayer();
        var enemy = MakeEnemy();

        combatManager.QueueCombatAction(new CombatAction("Strike", 10, player), player);
        combatManager.QueueRoundOver(player);
        combatManager.PlayCombatRound(player, enemy);

        Assert.NotEmpty(combatManager.GetLog());
    }
}