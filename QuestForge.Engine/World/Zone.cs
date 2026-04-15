namespace QuestForge.Engine.World;

public class Zone
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }

    // LIFO
    private Stack<GameEvent> _events = new();
    public IEnumerable<GameEvent> Events => _events;

    public Zone(string name, string description, int difficulty = 1)
    {
        Name = name;
        Description = description;
        Difficulty = difficulty;
    }

    public bool PushEvent(GameEvent gameEvent)
    {
        if (gameEvent == null)
            return false;

        _events.Push(gameEvent);
        return true;
    }

    public GameEvent? PopEvent()
    {
        if (_events.Count == 0)
            return null;

        return _events.Pop();
    }

    public GameEvent? PeekEvent()
    {
        if (_events.Count == 0)
            return null;
        return _events.Peek();
    }

    public override string ToString() => $"[Zone] {Name} (Diff:{Difficulty}) - {Description}";
}