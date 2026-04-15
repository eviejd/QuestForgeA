namespace QuestForge.Engine.World;

using QuestForge.Engine.Models;

public enum EventType { Combat, Loot, Dialogue, Campfire }

public class GameEvent
{
    public EventType Type { get; set; }
    public string Description { get; set; }
    public Difficulty? CombatDifficulty { get; set; }
    public Rarity? LootRarity { get; set; }
    
    public GameEvent(EventType type, string description)
    {
        Type = type;
        Description = description;
    }

    public override string ToString() => $"[{Type}] {Description}";
}