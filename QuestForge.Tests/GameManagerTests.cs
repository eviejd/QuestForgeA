using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using Xunit;
namespace QuestForge.Tests;

public class GameManagerTests
{
    [Fact]
    public void Register_ReturnsUniqueIds()
    {
        var gameManager = new GameManager();
        int id1 = gameManager.Register(new Player("Aria"));
        int id2 = gameManager.Register(new Enemy("Goblin", 30, 8, 3, Difficulty.Easy));
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void Register_SetsActivePlayer()
    {
        var gameManager = new GameManager();
        var player = new Player("Aria");
        gameManager.Register(player);
        Assert.Equal(player, gameManager.ActivePlayer);
    }

    [Fact]
    public void GetEntity_ReturnsCorrectEntity()
    {
        var gameManager = new GameManager();
        var enemy = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        int id = gameManager.Register(enemy);
        Assert.Equal(enemy, gameManager.GetEntity(id));
    }

    [Fact]
    public void GetEntity_ReturnsNullForBadId()
    {
        Assert.Null(new GameManager().GetEntity(999));
    }

    [Fact]
    public void Unregister_ReturnsTrueWhenFound()
    {
        var gameManager = new GameManager();
        int id = gameManager.Register(new Player("Aria"));
        Assert.True(gameManager.Unregister(id));
    }

    [Fact]
    public void Unregister_ReturnsFalseWhenNotFound()
    {
        Assert.False(new GameManager().Unregister(99));
    }

    [Fact]
    public void Unregister_ClearsActivePlayer()
    {
        var gameManager = new GameManager();
        int id = gameManager.Register(new Player("Aria"));
        gameManager.Unregister(id);
        Assert.Null(gameManager.ActivePlayer);
    }

    [Fact]
    public void GetAll_ReturnsAllEntities()
    {
        var gameManager = new GameManager();
        gameManager.Register(new Player("Aria"));
        gameManager.Register(new Enemy("Goblin", 30, 8, 3, Difficulty.Easy));
        Assert.Equal(2, gameManager.GetAll().Count);
    }
}