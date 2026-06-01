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

    public GameEvent? BeginCombat(GameEntity obj1, GameEntity obj2)
    {
        _log.Clear();
        var player = obj1 as Player ?? obj2 as Player;
        var enemy  = obj1 as Enemy  ?? obj2 as Enemy;

        if (player == null || enemy == null)
            throw new ArgumentException("BeginCombat requires one Player and one Enemy");

        _log.Add($"=== Combat: {player.Name} vs {enemy.Name} ===");

        int round = 1;
        while (player.IsAlive && enemy.IsAlive)
        {
            _log.Add($"\n-- Round {round++} --");

            if (_combatQueue.Count == 0)
            {
                _combatQueue.Enqueue(new CombatAction("Strike", player.Attack, player));
                var ea = enemy.CombatActions[Random.Shared.Next(enemy.CombatActions.Count)];
                _combatQueue.Enqueue(new CombatAction(ea.Name, ea.Power, enemy));
                QueueRoundOver(player);
            }

            var result = PlayCombatRound(player, enemy);
            if (result == null) continue;
            if (result.Type == EventType.Loot)                { _log.Add($"{player.Name} wins!"); return result; }
            if (result.Description == "Game Over")            { _log.Add($"{player.Name} defeated."); return result; }
            if (result.Description?.Contains("fled") == true) { _log.Add("Combat ended."); return result; }
        }

        return new GameEvent(EventType.Dialogue, "Combat ended.");
    }

    public GameEvent? PlayCombatRound(Player player, Enemy enemy)
    {
        bool playerFled = false;
        bool enemyFled  = false;

        while (_combatQueue.Count > 0)
        {
            var action = _combatQueue.Dequeue();

            if (action.Name == "RoundOver") { _log.Add("-- Round Over --"); break; }

            if (action.Name == "Flee")
            {
                _log.Add($"{action.Source.Name} fled!");
                if (action.Source is Player) playerFled = true;
                else enemyFled = true;
                continue;
            }

            if (action.Name == "Defend") { _log.Add($"{action.Source.Name} defends."); continue; }

            _log.Add(action.ToString());

            if (action.Source is Player)
            {
                int dmg = Math.Max(0, action.Power - enemy.Defence);
                enemy.Health -= dmg;
                _log.Add($"  -> {enemy.Name} takes {dmg} damage (HP:{enemy.Health})");
            }
            else
            {
                int dmg = Math.Max(0, action.Power - player.Defence);
                player.Health -= dmg;
                _log.Add($"  -> {player.Name} takes {dmg} damage (HP:{player.Health})");
            }
        }

        // both fled
        if (playerFled && enemyFled)
        {
            _log.Add("Both parties fled — combat draw!");
            return new GameEvent(EventType.Dialogue, "Both fled.");
        }

        // player fled
        if (playerFled)
        {
            return new GameEvent(EventType.Dialogue, $"{player.Name} fled.");
        }

        // enemy fled
        if (enemyFled)
        {
            _log.Add($"{enemy.Name} fled!");
            var loot = new GameEvent(EventType.Loot, $"{enemy.Name} fled, leaving loot!");
            loot.LootRarity = Rarity.Common;
            return loot;
        }

        // both die same round
        if (!enemy.IsAlive && !player.IsAlive)
        {
            _log.Add("Both combatants fell — it's a draw!");
            return new GameEvent(EventType.Dialogue, "Draw");
        }

        if (!enemy.IsAlive)
        {
            var loot = new GameEvent(EventType.Loot, $"Defeated {enemy.Name}!");
            loot.LootRarity = enemy.Difficulty == Difficulty.Boss ? Rarity.Legendary : Rarity.Common;
            return loot;
        }

        if (!player.IsAlive) return new GameEvent(EventType.Dialogue, "Game Over");

        return new GameEvent(EventType.Combat, "Combat continues...");
    }

    public void PrintLog()
    {
        Console.WriteLine("--- Combat Log ---");
        foreach (var entry in _log) Console.WriteLine(entry);
        Console.WriteLine("------------------");
    }

    public string GetLog() => string.Join("\n", _log);
    public List<string> GetLogLines() => _log;
}