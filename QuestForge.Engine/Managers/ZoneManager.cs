namespace QuestForge.Engine.Managers;

using QuestForge.Engine.World;

public class ZoneManager
{
    private LinkedList<Zone> _zones = new();
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
            var prevNode = _zones.Find(prevZone);
            if (prevNode == null)
                throw new InvalidOperationException("prevZone not found in zone list");
            _zones.AddAfter(prevNode, zone);
        }
        else
        {
            var nextNode = _zones.Find(nextZone!);
            if (nextNode == null)
                throw new InvalidOperationException("nextZone not found in zone list");

            _zones.AddBefore(nextNode, zone);
        }

        return _zones;
    }

    public Zone? GetZone(string name)
    {
        foreach (var zone in _zones)
        {
            if (zone.Name == name)
                return zone;
        }
        return null;
    }

    public bool SetCurrentZone(Zone zone)
    {
        if (_zones.Find(zone) == null)
            return false;
        CurrentZone = zone;
        return true;
    }

    public LinkedList<Zone> GetZones() => _zones;
}