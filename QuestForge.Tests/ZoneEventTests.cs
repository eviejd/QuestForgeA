using QuestForge.Engine.Managers;
using QuestForge.Engine.World;

namespace QuestForge.Tests;

public class ZoneManagerEventTests
{
    private (ZoneManager zm, Zone zone) Setup()
    {
        var zm = new ZoneManager();
        var zone = new Zone("Town", "A town", 1);
        zm.AddZone(zone, null, null);
        return (zm, zone);
    }

    private GameEvent MakeEvent(EventType type = EventType.Dialogue) =>
        new(type, "test");

    [Fact]
    public void PushEvent_ReturnsTrueForValidZone()
    {
        var (zm, zone) = Setup();
        Assert.True(zm.PushEvent(zone, MakeEvent()));
    }

    [Fact]
    public void PushEvent_ReturnsFalseForUnknownZone()
    {
        var zm = new ZoneManager();
        var orphan = new Zone("Nowhere", "not in map", 1);
        Assert.False(zm.PushEvent(orphan, MakeEvent()));
    }

    [Fact]
    public void PopNextEvent_ReturnsNullWhenEmpty()
    {
        var (zm, zone) = Setup();
        Assert.Null(zm.PopNextEvent(zone));
    }

    [Fact]
    public void PopNextEvent_ReturnsLastPushedFirst()
    {
        var (zm, zone) = Setup();
        var first = MakeEvent(EventType.Dialogue);
        var second = MakeEvent(EventType.Combat);
        zm.PushEvent(zone, first);
        zm.PushEvent(zone, second);

        Assert.Equal(second, zm.PopNextEvent(zone));
    }

    [Fact]
    public void PeekNextEvent_DoesNotRemove()
    {
        var (zm, zone) = Setup();
        zm.PushEvent(zone, MakeEvent());
        zm.PeekNextEvent(zone);
        Assert.NotNull(zm.PopNextEvent(zone));
    }

    [Fact]
    public void PeekNextEvent_ReturnsNullForUnknownZone()
    {
        var zm = new ZoneManager();
        var orphan = new Zone("Nowhere", "not in map", 1);
        Assert.Null(zm.PeekNextEvent(orphan));
    }

    [Fact]
    public void PopNextEvent_ReturnsNullForUnknownZone()
    {
        var zm = new ZoneManager();
        var orphan = new Zone("Nowhere", "not in map", 1);
        Assert.Null(zm.PopNextEvent(orphan));
    }
}