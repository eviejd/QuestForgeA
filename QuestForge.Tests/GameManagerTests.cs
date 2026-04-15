using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class GameManagerTests
{
    [Fact]
    public void Register_ReturnsUniqueIds()
    {
        var gm = new GameManager();
        int id1 = gm.Register(new Player("Aria"));
        int id2 = gm.Register(new Enemy("Goblin", 30, 8, 3, Difficulty.Easy));
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void Register_SetsActivePlayer()
    {
        var gm = new GameManager();
        var p = new Player("Aria");
        gm.Register(p);
        Assert.Equal(p, gm.ActivePlayer);
    }

    [Fact]
    public void GetEntity_ReturnsCorrectEntity()
    {
        var gm = new GameManager();
        var e = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
        int id = gm.Register(e);
        Assert.Equal(e, gm.GetEntity(id));
    }

    [Fact]
    public void GetEntity_ReturnsNullForBadId()
    {
        Assert.Null(new GameManager().GetEntity(999));
    }

    [Fact]
    public void Unregister_ReturnsTrueWhenFound()
    {
        var gm = new GameManager();
        int id = gm.Register(new Player("Aria"));
        Assert.True(gm.Unregister(id));
    }

    [Fact]
    public void Unregister_ReturnsFalseWhenNotFound()
    {
        Assert.False(new GameManager().Unregister(99));
    }

    [Fact]
    public void Unregister_ClearsActivePlayer()
    {
        var gm = new GameManager();
        int id = gm.Register(new Player("Aria"));
        gm.Unregister(id);
        Assert.Null(gm.ActivePlayer);
    }

    [Fact]
    public void GetAll_ReturnsAllEntities()
    {
        var gm = new GameManager();
        gm.Register(new Player("Aria"));
        gm.Register(new Enemy("Goblin", 30, 8, 3, Difficulty.Easy));
        Assert.Equal(2, gm.GetAll().Count);
    }
}