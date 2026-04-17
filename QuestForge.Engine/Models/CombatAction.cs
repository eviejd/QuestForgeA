namespace QuestForge.Engine.Models;

public class CombatAction
{
    public string Name { get; set; }
    public int Power { get; set; }
    public GameEntity Source { get; set; }

    public CombatAction(string name, int power, GameEntity source)
    {
        Name = name;
        Power = power;
        Source = source;
    }

    public override string ToString() => $"{Source.Name} uses {Name} (Power:{Power})";
}