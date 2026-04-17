namespace QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

public class CombatManager
{
    private Queue<CombatAction> _combatQueue = new();
    private List<string> _log = new();

    public bool QueueCombatAction(CombatAction action, GameEntity source)
    {
        action.Source = source;
        _combatQueue.Enqueue(action);
        return true;
    }

    public void QueueRoundOver(GameEntity source)
    {
        _combatQueue.Enqueue(new CombatAction("RoundOver", 0, source));
    }

public GameEvent? PlayCombatRound(Player player, Enemy enemy)
    {
        _log.Clear();

        while (_combatQueue.Count > 0)
        {
            var action = _combatQueue.Dequeue();

            if (action.Name == "RoundOver")
            {
                _log.Add("-- Round Over --");
                break;
            }

            _log.Add(action.ToString());

            if (action.Name == "Flee")
            {
                _log.Add($"{action.Source.Name} fled from combat!");
                return new GameEvent(EventType.Dialogue, $"{action.Source.Name} fled.");
            }

            if (action.Name == "Defend")
            {
                _log.Add($"{action.Source.Name} takes a defensive stance.");
                continue;
            }

            if (action.Source is Player)
            {
                int damgage = Math.Max(0, action.Power - enemy.Defence);
                enemy.Health -= damgage;
                _log.Add($"  -> {enemy.Name} takes {damgage} damage (HP: {enemy.Health})");
            }
            else
            {
                int damgage = Math.Max(0, action.Power - player.Defence);
                player.Health -= damgage;
                _log.Add($"  -> {player.Name} takes {damgage} damage (HP: {player.Health})");
            }
        }

        if (!enemy.IsAlive)
        {
            var loot = new GameEvent(EventType.Loot, $"Defeated {enemy.Name}!");
            loot.LootRarity = enemy.Difficulty == Difficulty.Boss ? Rarity.Legendary : Rarity.Common;
            return loot;
        }

        if (!player.IsAlive)
            return new GameEvent(EventType.Dialogue, "Game Over");

        return new GameEvent(EventType.Combat, "Combat continues...");
    }
    public void PrintLog()
    {
        Console.WriteLine("--- Combat Log ---");
        foreach (var entry in _log)
            Console.WriteLine(entry);
        Console.WriteLine("------------------");
    }

    public List<string> GetLog() => _log;

    internal bool QueueCombatAction(CombatAction action)
    {
        throw new NotImplementedException();
    }
}