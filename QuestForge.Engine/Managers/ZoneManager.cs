namespace QuestForge.Engine.Managers;

using QuestForge.Engine.Models;
using QuestForge.Engine.World;

public class ZoneManager
{
    private LinkedList<Zone> _zones = new();
    private Dictionary<string, ZoneRequirement> _requirements = new();
    public Zone? CurrentZone { get; private set; }

    public LinkedList<Zone> AddZone(Zone zone, Zone? prevZone, Zone? nextZone)
    {
        if (prevZone == null && nextZone == null)
        {
            if (_zones.Count > 0)
                throw new InvalidOperationException("Must provide prevZone or nextZone when zones already exist");

            _zones.AddFirst(zone);
            CurrentZone = zone;
            return _zones;
        }

        if (prevZone != null)
        {
            var prevNode = _zones.Find(prevZone)
                ?? throw new InvalidOperationException($"prevZone '{prevZone.Name}' not found");
            _zones.AddAfter(prevNode, zone);
        }
        else
        {
            var nextNode = _zones.Find(nextZone!)
                ?? throw new InvalidOperationException($"nextZone '{nextZone!.Name}' not found");
            _zones.AddBefore(nextNode, zone);
        }

        if (CurrentZone == null) CurrentZone = zone;
        return _zones;
    }

    public void AddRequirement(Zone zone, List<GameEvent> requiredEvents, List<Item> requiredItems)
    {
        if (requiredEvents.Count == 0 && requiredItems.Count == 0)
            throw new ArgumentException("At least one requirement must be provided");

        _requirements[zone.Name] = new ZoneRequirement(requiredEvents, requiredItems);
    }

    public bool MeetsRequirements(Zone zone, Player player)
    {
        if (!_requirements.TryGetValue(zone.Name, out var req))
            return true;

        foreach (var required in req.RequiredEvents)
        {
            if (!player.ClearedEvents.Any(e => e.Type == required.Type))
                return false;
        }

        foreach (var required in req.RequiredItems)
        {
            if (!player.Inventory.Any(i => i.Name == required.Name))
                return false;
        }

        return true;
    }

    public Zone? GetZone(string name)
    {
        foreach (var zone in _zones)
            if (zone.Name == name) return zone;
        return null;
    }

    public bool SetCurrentZone(Zone zone)
    {
        if (_zones.Find(zone) == null) return false;
        CurrentZone = zone;
        return true;
    }

    public LinkedList<Zone> GetZones() => _zones;

    public bool PushEvent(Zone zone, GameEvent gameEvent)
    {
        if (_zones.Find(zone) == null) return false;
        return zone.PushEvent(gameEvent);
    }

    public GameEvent? PopNextEvent(Zone zone)
    {
        if (_zones.Find(zone) == null) return null;
        return zone.PopEvent();
    }

    public GameEvent? PeekNextEvent(Zone zone)
    {
        if (_zones.Find(zone) == null) return null;
        return zone.PeekEvent();
    }
}