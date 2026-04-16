using QuestForge.Engine.Managers;
using QuestForge.Engine.World;
namespace QuestForge.Tests;

public class ZoneManagerTests
{
    private Zone MakeZone(string name) => new(name, "test zone");

    [Fact]
    public void AddZone_FirstZone_NoNeighboursRequired()
    {
        var zoneManager = new ZoneManager();
        var town = MakeZone("Town");
        zoneManager.AddZone(town, null, null);
        Assert.Single(zoneManager.GetZones());
    }

    [Fact]
    public void AddZone_ThrowsIfBothNullAndZonesExist()
    {
        var zoneManager = new ZoneManager();
        zoneManager.AddZone(MakeZone("Town"), null, null);
        Assert.Throws<InvalidOperationException>(() =>
            zoneManager.AddZone(MakeZone("Outskirts"), null, null));
    }

    [Fact]
    public void AddZone_AfterPrev_InsertsCorrectly()
    {
        var zoneManager = new ZoneManager();
        var town = MakeZone("Town");
        var outskirts = MakeZone("Outskirts");
        zoneManager.AddZone(town, null, null);
        zoneManager.AddZone(outskirts, town, null);

        var zones = zoneManager.GetZones().ToList();
        Assert.Equal("Town", zones[0].Name);
        Assert.Equal("Outskirts", zones[1].Name);
    }

    [Fact]
    public void AddZone_BeforeNext_InsertsCorrectly()
    {
        var zoneManager = new ZoneManager();
        var town = MakeZone("Town");
        var cave = MakeZone("Cave");
        var outskirts = MakeZone("Outskirts");
        zoneManager.AddZone(town, null, null);
        zoneManager.AddZone(cave, town, null);
        zoneManager.AddZone(outskirts, null, cave);

        var zones = zoneManager.GetZones().ToList();
        Assert.Equal("Outskirts", zones[1].Name);
    }

    [Fact]
    public void GetZone_ReturnsCorrectZone()
    {
        var zoneManager = new ZoneManager();
        var town = MakeZone("Town");
        zoneManager.AddZone(town, null, null);
        Assert.Equal(town, zoneManager.GetZone("Town"));
    }

    [Fact]
    public void GetZone_ReturnsNullWhenNotFound()
    {
        var zoneManager = new ZoneManager();
        Assert.Null(zoneManager.GetZone("Nowhere"));
    }

    [Fact]
    public void SetCurrentZone_UpdatesCurrentZone()
    {
        var zoneManager = new ZoneManager();
        var town = MakeZone("Town");
        zoneManager.AddZone(town, null, null);
        zoneManager.SetCurrentZone(town);
        Assert.Equal(town, zoneManager.CurrentZone);
    }
}