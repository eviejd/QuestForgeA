using QuestForge.Engine.Managers;
namespace QuestForge.Engine.Models;

public class Player : GameEntity
{
    public int Score { get; set; }
    public string CurrentZone { get; set; } = "Unknown";

    private const int InventoryLimit = 20;
    private readonly List<Item> _inventory = new();
    public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

    public Player(string name, int health = 100, int attack = 10, int defence = 5) : base(name, health, attack, defence) { }

    public bool AddItemToInventory(Item item)
    {
        if (_inventory.Count >= InventoryLimit)
            return false;

        _inventory.Add(item);
        return true;
    }

    public bool RemoveItemFromInventory(Item item)
    {
        if (_inventory.Remove(item))
            return true;
        var match = _inventory.FirstOrDefault(i => i.Name == item.Name);
        if (match is null) return false;

        _inventory.Remove(match);
        return true;
    }

    public List<Item> FindItemByName(string name)
    {
        return _inventory
            .Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public override string ToString()
    {
        return $"[Player] {Name} | HP:{Health} ATK:{Attack} DEF:{Defence} Score:{Score} " + $"Zone:{CurrentZone} Items:{_inventory.Count}/{InventoryLimit}";
    }

public bool MovePlayer(ZoneManager zoneManager, string toZoneName)
    {
        var destination = zoneManager.GetZone(toZoneName);
        if (destination == null)
        {
            Console.WriteLine($"Zone '{toZoneName}' doesn't exist");
            return false;
        }

        if (zoneManager.CurrentZone == null)
        {
            Console.WriteLine("Player has no current zone set");
            return false;
        }

        var zones = zoneManager.GetZones();
        var currentNode = zones.Find(zoneManager.CurrentZone);
        if (currentNode == null)
            return false;

        bool isAdjacent = (currentNode.Next?.Value == destination) || (currentNode.Previous?.Value == destination);

        if (!isAdjacent)
        {
            Console.WriteLine($"Can't move from {zoneManager.CurrentZone.Name} to {toZoneName} - not adjacent");
            return false;
        }

        CurrentZone = toZoneName;
        zoneManager.SetCurrentZone(destination);
        return true;
    }
}