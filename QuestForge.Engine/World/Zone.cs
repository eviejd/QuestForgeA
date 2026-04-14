namespace QuestForge.Engine.World;

public class Zone
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }

    public Zone(string name, string description, int difficulty = 1)
    {
        Name = name;
        Description = description;
        Difficulty = difficulty;
    }
    public override string ToString() => $"[Zone] {Name} (Diff:{Difficulty}) - {Description}";
}