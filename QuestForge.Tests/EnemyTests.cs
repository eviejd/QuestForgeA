using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class EnemyTests
{
    [Fact]
    public void Enemy_HasAtLeastTwoActions()
    {
        var e = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        Assert.True(e.CombatActions.Count >= 2);
    }

    [Fact]
    public void BossEnemy_HasMoreActionsThanEasy()
    {
        var easy = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        var boss = new Enemy("Dragon", 30, 8, 3, Difficulty.Boss);
        Assert.True(boss.CombatActions.Count > easy.CombatActions.Count);
    }

    [Fact]
    public void Enemy_FromTemplate_ScalesHealth()
    {
        var template = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        var hard = new Enemy(Difficulty.Hard, template);
        Assert.True(hard.Health > template.Health);
    }

    [Fact]
    public void ToString_ContainsDifficulty()
    {
        var e = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        Assert.Contains("Easy", e.ToString());
    }
}