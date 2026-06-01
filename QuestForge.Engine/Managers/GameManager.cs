namespace QuestForge.Engine.Managers;

using System.Text.Json;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

public class GameManager
{
    private Dictionary<int, GameEntity> _entities = new();
    private int _nextId = 1;

    public Player? ActivePlayer { get; private set; }
    public Leaderboard Leaderboard { get; } = new();

    // score values per event type
    public const int ScoreLoot     = 1;
    public const int ScoreCampfire = 5;
    public const int ScoreEasy     = 1;
    public const int ScoreHard     = 5;
    public const int ScoreBoss     = 10;

    public int Register(GameEntity entity)
    {
        int id = _nextId++;
        _entities[id] = entity;
        if (entity is Player p) ActivePlayer = p;
        return id;
    }

    public bool Unregister(int id)
    {
        if (!_entities.TryGetValue(id, out var entity)) return false;
        if (entity is Player) ActivePlayer = null;
        _entities.Remove(id);
        return true;
    }

    public GameEntity? GetEntity(int id)
    {
        _entities.TryGetValue(id, out var entity);
        return entity;
    }

    public List<GameEntity> GetAll() => _entities.Values.ToList();

    public GameEvent? PeekNextEvent(ZoneManager zm)
    {
        if (zm.CurrentZone == null) return null;
        return zm.PeekNextEvent(zm.CurrentZone);
    }

    public bool ApplyEffects(GameEvent gameEvent, ZoneManager zm)
    {
        if (ActivePlayer == null) return false;

        switch (gameEvent.Type)
        {
            case EventType.Campfire:
                ActivePlayer.Health = 100;
                ActivePlayer.AddScore(ScoreCampfire);
                Console.WriteLine($"{ActivePlayer.Name} rested. HP restored. +{ScoreCampfire} score.");
                break;

            case EventType.Loot:
                var item = Item.MakeLoot(gameEvent.LootRarity ?? Rarity.Common);
                bool added = ActivePlayer.AddItemToInventory(item);
                Console.WriteLine(added ? $"Found: {item}" : "Inventory full.");
                ActivePlayer.AddScore(ScoreLoot);
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

        ActivePlayer.ClearEvent(gameEvent);
        return true;
    }

    public int ApplyCombatScore(Difficulty difficulty)
    {
        if (ActivePlayer == null) return 0;

        int pts = difficulty switch
        {
            Difficulty.Easy => ScoreEasy,
            Difficulty.Hard => ScoreHard,
            Difficulty.Boss => ScoreBoss,
            _               => ScoreEasy
        };

        ActivePlayer.AddScore(pts);
        return pts;
    }

    public bool RegisterScore(Player player)
    {
        return Leaderboard.AddScore(player, player.Score);
    }

    public bool SaveRankings(string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(Leaderboard.ToList(),
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save rankings: {ex.Message}");
            return false;
        }
    }

    public Leaderboard LoadRankings(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Leaderboard.Initialise();
                return Leaderboard;
            }

            var entries = JsonSerializer.Deserialize<List<LeaderboardEntry>>(
                File.ReadAllText(filePath));
            if (entries != null) Leaderboard.FromList(entries);
            return Leaderboard;
        }
        catch
        {
            Leaderboard.Initialise();
            return Leaderboard;
        }
    }

    public string PrintTopTen() => Leaderboard.PrintTopTen();
}