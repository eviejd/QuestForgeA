using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;
namespace QuestForge.Tests;

public class MovementTests
{
    private (Player player, ZoneManager zoneManager) Setup()
    {
        var zoneManager = new ZoneManager();
        var town = new Zone("Town", "A town", 1);
        var outskirts = new Zone("Outskirts", "The outskirts", 2);
        var cave = new Zone("Cave", "A dark cave", 3);

        zoneManager.AddZone(town, null, null);
        zoneManager.AddZone(outskirts, town, null);
        zoneManager.AddZone(cave, outskirts, null);
        zoneManager.SetCurrentZone(town);

        var player = new Player("Aria");
        player.CurrentZone = "Town";

        return (player, zoneManager);
    }

    [Fact]
    public void MovePlayer_ReturnsTrueForAdjacentZone()
    {
        var (player, zoneManager) = Setup();
        Assert.True(player.MovePlayer(zoneManager, "Outskirts"));
    }

    [Fact]
    public void MovePlayer_ReturnsFalseForNonAdjacentZone()
    {
        var (player, zoneManager) = Setup();
        Assert.False(player.MovePlayer(zoneManager, "Cave"));
    }

    [Fact]
    public void MovePlayer_ReturnsFalseForUnknownZone()
    {
        var (player, zoneManager) = Setup();
        Assert.False(player.MovePlayer(zoneManager, "Narnia"));
    }

    [Fact]
    public void MovePlayer_UpdatesPlayerCurrentZone()
    {
        var (player, zoneManager) = Setup();
        player.MovePlayer(zoneManager, "Outskirts");
        Assert.Equal("Outskirts", player.CurrentZone);
    }

    [Fact]
    public void MovePlayer_UpdatesZoneManagerCurrentZone()
    {
        var (player, zoneManager) = Setup();
        player.MovePlayer(zoneManager, "Outskirts");
        Assert.Equal("Outskirts", zoneManager.CurrentZone?.Name);
    }
}