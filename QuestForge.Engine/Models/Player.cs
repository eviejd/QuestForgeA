namespace QuestForge.Engine.Models;

using QuestForge.Engine.Managers;
using QuestForge.Engine.World;

public class Player : GameEntity
{
    public int Score { get; set; }
    public string CurrentZone { get; set; } = "Unknown";

    private const int InventoryLimit = 20;
    private readonly List<Item> _inventory = new();
    public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

    public List<GameEvent> ClearedEvents { get; } = new();
    private bool _hasUsedInterrupt = false;
    public bool HasUsedInterrupt => _hasUsedInterrupt;

    public Player(string name, int health = 100, int attack = 10, int defence = 5)
        : base(name, health, attack, defence) { }

    public int AddScore(int amount)
    {
        Score += amount;
        return Score;
    }

    public void ClearEvent(GameEvent gameEvent)
    {
        ClearedEvents.Add(gameEvent);
    }

    public void ResetInterrupt()
    {
        _hasUsedInterrupt = false;
    }

    public bool AddItemToInventory(Item item)
    {
        if (_inventory.Count >= InventoryLimit) return false;
        _inventory.Add(item);
        return true;
    }

    public bool RemoveItemFromInventory(Item item)
    {
        if (_inventory.Remove(item)) return true;
        var match = _inventory.FirstOrDefault(i => i.Name == item.Name);
        if (match is null) return false;
        _inventory.Remove(match);
        return true;
    }

    public List<Item> FindItemByName(string name)
    {
        var found = new List<Item>();
        foreach (Item item in _inventory)
        {
            if (item.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                found.Add(item);
        }
        return found;
    }

    public bool CanMove(ZoneManager zm, string toZoneName)
    {
        var destination = zm.GetZone(toZoneName);
        if (destination == null) return false;
        if (zm.CurrentZone == null) return false;

        var currentNode = zm.GetZones().Find(zm.CurrentZone);
        if (currentNode == null) return false;

        bool isAdjacent = currentNode.Next?.Value == destination ||
                          currentNode.Previous?.Value == destination;

        if (!isAdjacent) return false;
        return zm.MeetsRequirements(destination, this);
    }

    public bool MovePlayer(ZoneManager zm, string toZoneName)
    {
        if (!CanMove(zm, toZoneName)) return false;
        CurrentZone = toZoneName;
        zm.SetCurrentZone(zm.GetZone(toZoneName)!);
        ResetInterrupt();
        return true;
    }

// tracks temporary defence bonus from interrupt
    public int InterruptDefenceBonus { get; private set; } = 0;

    public bool Interrupt(ZoneManager zm, Item item)
    {
        if (_hasUsedInterrupt) return false;
        if (item.Category != Category.Consumable && item.Category != Category.Armour)
            return false;

        bool hasItem = _inventory.Contains(item) || _inventory.Any(i => i.Name == item.Name);
        if (!hasItem) return false;

        if (item.Name.Contains("Elixir", StringComparison.OrdinalIgnoreCase))
        {
            Health = 100;
            RemoveItemFromInventory(item);
        }
        else if (item.Name.Contains("Potion", StringComparison.OrdinalIgnoreCase))
        {
            Health = Math.Min(Health + 20, 100);
            RemoveItemFromInventory(item);
        }
        else if (item.Category == Category.Armour)
        {
            // temporary defence boost, lasts until next zone
            InterruptDefenceBonus = item.Value / 10;
            Defence += InterruptDefenceBonus;
        }
        else
        {
            return false;
        }

        _hasUsedInterrupt = true;
        return true;
    }

    public void ResetInterrupt()
    {
        // remove any temporary defence boost on zone change
        Defence -= InterruptDefenceBonus;
        InterruptDefenceBonus = 0;
        _hasUsedInterrupt = false;
    }

    public override string ToString()
    {
        return $"[Player] {Name} | HP:{Health} ATK:{Attack} DEF:{Defence} Score:{Score} Items:{_inventory.Count}/20";
    }
}