using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

var (gameManager, zoneManager, player) = SampleWorld.Build();
var combatManager = new CombatManager();

Console.WriteLine("=== QuestForge ===");
Console.WriteLine(player);
Console.WriteLine();

RunGame();

void RunGame()
{
    while (true)
    {
        var zone = zoneManager.CurrentZone!;
        var next = gameManager.PeekNextEvent(zoneManager);

        Console.WriteLine($"\n[ {zone.Name} ]");
        Console.WriteLine(next != null ? $"Next: {next}" : "No events remaining.");
        Console.WriteLine($"HP: {player.Health} | Score: {player.Score} | Items: {player.Inventory.Count}/20");
        Console.WriteLine("\n1) Process event  2) Move  3) Inventory  4) Stats  5) Quit");
        Console.Write("> ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1": ProcessEvent(); break;
            case "2": Move(); break;
            case "3": Inventory(); break;
            case "4": Console.WriteLine(player); break;
            case "5": return;
            default: Console.WriteLine("Invalid."); break;
        }
    }
}

void ProcessEvent()
{
    var zone = zoneManager.CurrentZone!;
    var event = zone.PopNextEvent(zone);

    if (event == null)
    {
        Console.WriteLine("Nothing here. Move to another zone.");
        return;
    }

    Console.WriteLine($"\n{event}");

    if (event.Type == EventType.Combat)
        Combat(event);
    else
        gm.ApplyEffects(event, zoneManager);
}

void EndGame()
{
    Console.WriteLine($"\nGame over. Final score: {player.Score}");
    bool madeIt = gameManager.RegisterScore(player);
    Console.WriteLine(madeIt ? "You made the top 10!" : "Better luck next time.");
    Console.WriteLine(gameManager.PrintTopTen());
    gm.SaveRankings(lbPath);
}

void Combat(GameEvent event)
{
    var difficulty = event.CombatDifficulty ?? Difficulty.Easy;
    var template = new Enemy("Goblin", 30, 8, 3, Difficulty.Easy);
    var enemy = new Enemy(difficulty, template);
    Console.WriteLine($"A {enemy.Name} appears! HP:{enemy.Health} ATK:{enemy.Attack} DEF:{enemy.Defence}");

    while (player.IsAlive && enemy.IsAlive)
    {
        Console.WriteLine($"\nYour HP: {player.Health} | {enemy.Name} HP: {enemy.Health}");
        Console.WriteLine("1) Attack  2) Defend  3) Flee");
        Console.Write("> ");

        var input = Console.ReadLine()?.Trim();
        var playerAction = input switch
        {
            "2" => new CombatAction("Defend", 0, player),
            "3" => new CombatAction("Flee", 0, player),
            _  => new CombatAction("Strike", player.Attack, player)
        };

        var enemyAction = enemy.CombatActions[Random.Shared.Next(enemy.CombatActions.Count)];
        combatManager.QueueCombatAction(playerAction, player);
        combatManager.QueueCombatAction(new CombatAction(ea.Name, ea.Power, enemy), enemy);
        ombatManager.QueueRoundOver(player);

        var result = ombatManager.PlayCombatRound(player, enemy);
        combatManager.PrintLog();

        if (result == null) continue;

        if (result.Type == EventType.Loot)
        {
            int pts = gameManager.ApplyCombatScore(difficulty);
            Console.WriteLine($"Victory! +{pts} score. Total: {player.Score}");
            gameManager.ApplyEffects(result, zoneManager);
            player.ClearEvent(event);
            return;
        }

        if (result.Description == "Draw")
        {
            Console.WriteLine("Combat ended in a draw.");
            EndGame();
            Environment.Exit(0);
        }

        if (result.Description?.Contains("fled") == true)
        {
            Console.WriteLine("You fled.");
            return;
        }

        if (result.Description == "Game Over")
        {
            Console.WriteLine("You were defeated.");
            EndGame();
            Environment.Exit(0);
        }
    }
}

void Move()
{
    Console.WriteLine("\nAdjacent zones:");
    var zones = zoneManager.GetZones();
    var node = zones.Find(zoneManager.CurrentZone!);
    if (node?.Previous != null) Console.WriteLine($"  <- {node.Previous.Value.Name}");
    if (node?.Next != null)     Console.WriteLine($"  -> {node.Next.Value.Name}");

    Console.Write("Move to: ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) return;

    if (player.MovePlayer(zoneManager, input))
        Console.WriteLine($"Moved to {input}.");
    else
        Console.WriteLine("Can't move there - not adjacent.");
}

void Inventory()
{
    if (!player.Inventory.Any())
    {
        Console.WriteLine("Empty.");
        return;
    }

    Console.WriteLine($"\nInventory ({player.Inventory.Count}/20):");
    foreach (var item in player.Inventory)
        Console.WriteLine($"  {item}");
}