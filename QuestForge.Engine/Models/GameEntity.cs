namespace QuestForge.Engine.Models;

public abstract class GameEntity
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }
    public bool IsAlive => Health > 0;

    protected GameEntity(string name, int health, int attack, int defence)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Defence = defence;
    }

    public override string ToString()
    {
        string result = $"Name: {Name}" + $" | Health: {Health}" + $" | Attack: {Attack}" + $" | Defence: {Defence}";
        return result;
    }
}