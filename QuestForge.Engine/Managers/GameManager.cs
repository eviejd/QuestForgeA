namespace QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

public class GameManager
{
    private Dictionary<int, GameEntity> _entities = new();
    private int _nextId = 1;

    public Player? ActivePlayer { get; private set; }

    public int Register(GameEntity entity)
    {
        int id = _nextId++;
        _entities[id] = entity;

        if (entity is Player player)
            ActivePlayer = player;
        return id;
    }

    public bool Unregister(int id)
        {
            if (!_entities.TryGetValue(id, out var entity))
                return false;

            if (entity is Player)
                ActivePlayer = null;

            _entities.Remove(id);
            return true;
        }

    public GameEntity? GetEntity(int id)
    {
        _entities.TryGetValue(id, out var entity);
        return entity;
    }

    public List<GameEntity> GetAll()
    {
        return _entities.Values.ToList();
    }

    public GameEvent? PeekNextEvent(ZoneManager zoneManager)
    {
        if (zoneManager.CurrentZone == null)
            return null;

        return zoneManager.PeekNextEvent(zoneManager.CurrentZone);
    }

    public bool ApplyEffects(GameEvent gameEvent, ZoneManager zoneManager)
    {
        if (ActivePlayer == null)
            return false;

        switch (gameEvent.Type)
        {
            case EventType.Campfire:
                ActivePlayer.Health = 100;
                Console.WriteLine($"{ActivePlayer.Name} rested. HP restored.");
                break;

            case EventType.Loot:
                var rarity = gameEvent.LootRarity ?? Rarity.Common;
                var item = Item.MakeLoot(rarity);
                bool added = ActivePlayer.AddItemToInventory(item);
                Console.WriteLine(added ? $"Found: {item}" : "Inventory full.");
                break;

            case EventType.Dialogue:
                Console.WriteLine($"NPC: {gameEvent.Description}");
                break;

            case EventType.Combat:
                Console.WriteLine($"Combat: {gameEvent.Description}");
                break;

            default:
                return false;
        }

        return true;
    }
}