namespace QuestForge.Engine.Managers;
using QuestForge.Engine.Models;

public class GameManager
{
    private Dictionary<int, GameEntity> _entities = new();
    private int _nextId = 1;

    public Player? ActivePlayer { get; private set; }

    public int Register(GameEntity entity)
    {
        int id = _nextId++;
        _entities[id] = entity;

        if (entity is Player p)
            ActivePlayer = p;
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
}