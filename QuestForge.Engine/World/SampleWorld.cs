namespace QuestForge.Engine.World;
using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;

public static class SampleWorld
{
    public static (GameManager gm, ZoneManager zm, Player player) Build()
    {
        var gameManager = new GameManager();
        var zoneManager = new ZoneManager();

        var town = new Zone("Town", "A quiet starting town", 1);
        var outskirts = new Zone("Outskirts", "The edge of civilisation", 2);
        var cave = new Zone("GoblinCave", "Dark and dangerous", 3);

        zoneManager.AddZone(town, null, null);
        zoneManager.AddZone(outskirts, town, null);
        zoneManager.AddZone(cave, outskirts, null);

        zoneManager.PushEvent(town, new GameEvent(EventType.Combat, "A goblin attacks!")
            { CombatDifficulty = Difficulty.Easy });
        zoneManager.PushEvent(town, new GameEvent(EventType.Loot, "You find something on the ground")
            { LootRarity = Rarity.Common });
        zoneManager.PushEvent(town, new GameEvent(EventType.Loot, "A chest sits in the corner")
            { LootRarity = Rarity.Common });
        zoneManager.PushEvent(town, new GameEvent(EventType.Dialogue, "Welcome traveller, beware the caves."));
        zoneManager.PushEvent(outskirts, new GameEvent(EventType.Campfire, "A warm campfire crackles."));
        zoneManager.PushEvent(outskirts, new GameEvent(EventType.Loot, "Supplies left by a traveller")
            { LootRarity = Rarity.Common });
        zoneManager.PushEvent(outskirts, new GameEvent(EventType.Loot, "Something glints in the mud")
            { LootRarity = Rarity.Common });
        zoneManager.PushEvent(outskirts, new GameEvent(EventType.Combat, "A goblin patrol spots you!")
            { CombatDifficulty = Difficulty.Easy });
        zoneManager.PushEvent(outskirts, new GameEvent(EventType.Dialogue, "The town walls fade behind you."));

        zoneManager.PushEvent(cave, new GameEvent(EventType.Loot, "Goblin treasure pile")
            { LootRarity = Rarity.Rare });
        zoneManager.PushEvent(cave, new GameEvent(EventType.Combat, "The goblin king emerges!")
            { CombatDifficulty = Difficulty.Boss });
        zoneManager.PushEvent(cave, new GameEvent(EventType.Campfire, "A makeshift goblin campfire."));
        zoneManager.PushEvent(cave, new GameEvent(EventType.Combat, "Two goblins guard the path.")
            { CombatDifficulty = Difficulty.Hard });
        zoneManager.PushEvent(cave, new GameEvent(EventType.Combat, "A goblin lurks in the shadows.")
            { CombatDifficulty = Difficulty.Easy });
        zoneManager.PushEvent(cave, new GameEvent(EventType.Combat, "A goblin blocks the entrance.")
            { CombatDifficulty = Difficulty.Easy });

        var player = new Player("Evie", health: 85, attack: 20, defence: 10);
        player.Score = 102;
        player.CurrentZone = "Town";

        player.AddItemToInventory(new Item("Potion", "Recovers 20 HP", 1f, 10, Category.Consumable, Rarity.Common));
        player.AddItemToInventory(new Item("Sword", "Increases Attack by 10", 5f, 25, Category.Weapon, Rarity.Common));
        player.AddItemToInventory(new Item("Shield", "Increases Defence by 5", 6f, 50, Category.Armour, Rarity.Uncommon));

        zoneManager.SetCurrentZone(town);
        gameManager.Register(player);

        return (gameManager, zoneManager, player);
    }
}