namespace QuestForge.Engine.Models;

public enum Difficulty { Easy, Hard, Boss }

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

public class Enemy : GameEntity
{
    public Difficulty Difficulty { get; set; }
    public List<CombatAction> CombatActions { get; } = new();

    public Enemy(Difficulty difficulty, GameEntity template)
        : base(
            $"{difficulty} {template.Name}",
            ScaleHealth(template.Health, difficulty),
            ScaleAttack(template.Attack, difficulty),
            ScaleDefence(template.Defence, difficulty))
    {
        Difficulty = difficulty;
        AddDefaultActions();
    }

    public Enemy(string name, int health, int attack, int defence, Difficulty difficulty)
        : base(name, health, attack, defence)
    {
        Difficulty = difficulty;
        AddDefaultActions();
    }

    private void AddDefaultActions()
        {
            CombatActions.Add(new CombatAction("Strike", Attack, this));
            CombatActions.Add(new CombatAction("Defend", 0, this));

            if (Difficulty >= Difficulty.Hard)
                CombatActions.Add(new CombatAction("Power Slam", (int)(Attack * 1.5), this));

            if (Difficulty == Difficulty.Boss)
                CombatActions.Add(new CombatAction("Rage", Attack * 2, this));
        }

    private static int ScaleHealth(int base_, Difficulty d) => d switch
    {
        Difficulty.Easy => base_,
        Difficulty.Hard => (int)(base_ * 1.5),
        Difficulty.Boss => base_ * 3,
        _ => base_
    };

    private static int ScaleAttack(int base_, Difficulty d) => d switch
    {
        Difficulty.Easy => base_,
        Difficulty.Hard => (int)(base_ * 1.3),
        Difficulty.Boss => base_ * 2,
        _ => base_
    };

    private static int ScaleDefence(int base_, Difficulty d) => d switch
    {
        Difficulty.Easy => base_,
        Difficulty.Hard => (int)(base_ * 1.2),
        Difficulty.Boss => (int)(base_ * 1.8),
        _ => base_
    };

public override string ToString()
        {
            return $"[Enemy/{Difficulty}] {Name} | HP:{Health} ATK:{Attack} DEF:{Defence} " + $"| Actions: {string.Join(", ", CombatActions.Select(a => a.Name))}";
        }
}