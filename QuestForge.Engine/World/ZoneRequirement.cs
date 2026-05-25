namespace QuestForge.Engine.World;

using QuestForge.Engine.Models;

public class ZoneRequirement
{
    public List<GameEvent> RequiredEvents { get; }
    public List<Item> RequiredItems { get; }

    public ZoneRequirement(List<GameEvent> requiredEvents, List<Item> requiredItems)
    {
        RequiredEvents = requiredEvents;
        RequiredItems = requiredItems;
    }
}