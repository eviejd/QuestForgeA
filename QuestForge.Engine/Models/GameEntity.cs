namespace QuestForge.Engine.Models;

public abstract class GameEntity
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }

    protected GameEntity(string name, int health, int attack, int defence)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Defence = defence;
    }

    public bool IsAlive => Health > 0;

    public override string ToString()
    {
        return $"[{GetType().Name}] {Name} | HP: {Health} | ATK: {Attack} | DEF: {Defence}";
    }
}