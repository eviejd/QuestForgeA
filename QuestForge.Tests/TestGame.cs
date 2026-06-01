using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;
using QuestForge.Engine.World;

namespace QuestForge.Tests;

public class TestGame
{
    [Fact]
    public void PlayCompleteGame()
    {
        var gameManager = new GameManager();
        var zoneManager = new ZoneManager();
        var combatManager = new CombatManager();

        var startingZone = new Zone("Village", "A peaceful starting village", 1);
        var forestZone   = new Zone("Forest", "A dense and dangerous forest", 2);
        var ruinsZone    = new Zone("Ruins", "Ancient ruins full of traps", 2);
        var fortressZone = new Zone("Fortress", "The enemy stronghold", 3);

        zoneManager.AddZone(startingZone, null, null);
        zoneManager.AddZone(forestZone, startingZone, null);
        zoneManager.AddZone(ruinsZone, forestZone, null);
        zoneManager.AddZone(fortressZone, ruinsZone, null);

        var ruinsGateGuardEvent = new GameEvent(EventType.Combat, "Ruins guard");

        zoneManager.AddRequirement(
            fortressZone,
            new List<GameEvent> { ruinsGateGuardEvent },
            new List<Item>());

        var ancientKeyItem = new Item(
            "Ancient Key",
            "Opens the ruins gate",
            0.5f,
            0,
            Category.QuestItem,
            Rarity.Uncommon);

        zoneManager.AddRequirement(
            ruinsZone,
            new List<GameEvent>(),
            new List<Item> { ancientKeyItem });

        zoneManager.PushEvent(startingZone, new GameEvent(EventType.Combat, "Bandit ambush!")
        {
            CombatDifficulty = Difficulty.Easy
        });

        zoneManager.PushEvent(startingZone, new GameEvent(EventType.Loot, "Supply crate")
        {
            LootRarity = Rarity.Common
        });

        zoneManager.PushEvent(startingZone, new GameEvent(EventType.Loot, "Hidden stash")
        {
            LootRarity = Rarity.Uncommon
        });

        zoneManager.PushEvent(startingZone, new GameEvent(EventType.Campfire, "Village hearth"));

        zoneManager.PushEvent(startingZone, new GameEvent(EventType.Dialogue,
            "An elder warns you of dangers ahead."));

        zoneManager.PushEvent(forestZone, new GameEvent(EventType.Combat, "Wolf pack!")
        {
            CombatDifficulty = Difficulty.Hard
        });

        zoneManager.PushEvent(forestZone, new GameEvent(EventType.Loot, "Forest cache")
        {
            LootRarity = Rarity.Rare
        });

        zoneManager.PushEvent(forestZone, new GameEvent(EventType.Dialogue, "A ranger nods at you."));

        zoneManager.PushEvent(ruinsZone, new GameEvent(EventType.Combat, "Skeleton warrior")
        {
            CombatDifficulty = Difficulty.Hard
        });

        zoneManager.PushEvent(ruinsZone, new GameEvent(EventType.Loot, "Ancient treasure")
        {
            LootRarity = Rarity.Rare
        });

        zoneManager.PushEvent(ruinsZone, new GameEvent(EventType.Campfire, "A sheltered alcove."));

        zoneManager.PushEvent(ruinsZone, new GameEvent(EventType.Dialogue, "Runes cover the walls."));

        zoneManager.PushEvent(fortressZone, new GameEvent(EventType.Loot, "Final reward")
        {
            LootRarity = Rarity.Legendary
        });

        zoneManager.PushEvent(fortressZone, new GameEvent(EventType.Combat, "Dark Lord!")
        {
            CombatDifficulty = Difficulty.Boss
        });

        var player = new Player("Aria", 100, 25, 10);

        player.AddItemToInventory(new Item("Potion", "Heals 20 HP", 1f, 10, Category.Consumable, Rarity.Common));
        player.AddItemToInventory(new Item("Elixir", "Full heal", 1f, 100, Category.Consumable, Rarity.Rare));
        player.AddItemToInventory(new Item("Sword", "Attack +10", 5f, 25, Category.Weapon, Rarity.Common));
        player.AddItemToInventory(new Item("Shield", "Defence +5", 6f, 50, Category.Armour, Rarity.Uncommon));
        player.AddItemToInventory(ancientKeyItem);

        zoneManager.SetCurrentZone(startingZone);

        int playerId = gameManager.Register(player);

        gameManager.LoadRankings("/tmp/qf_testgame_lb.json");

        // === VILLAGE ===

        var currentEvent = zoneManager.PopNextEvent(startingZone)!;
        Assert.Equal(EventType.Dialogue, currentEvent.Type);
        gameManager.ApplyEffects(currentEvent, zoneManager);

        currentEvent = zoneManager.PopNextEvent(startingZone)!;
        Assert.Equal(EventType.Campfire, currentEvent.Type);

        player.Health = 50;
        gameManager.ApplyEffects(currentEvent, zoneManager);

        Assert.Equal(100, player.Health);
        Assert.Equal(GameManager.ScoreCampfire, player.Score);

        currentEvent = zoneManager.PopNextEvent(startingZone)!;
        Assert.Equal(EventType.Loot, currentEvent.Type);
        gameManager.ApplyEffects(currentEvent, zoneManager);

        currentEvent = zoneManager.PopNextEvent(startingZone)!;
        gameManager.ApplyEffects(currentEvent, zoneManager);

        var upcomingCombatEvent = zoneManager.CurrentZone!.PeekEvent();
        Assert.Equal(EventType.Combat, upcomingCombatEvent?.Type);

        player.Health = 60;

        var healingPotion = player.Inventory.First(i => i.Name == "Potion");
        Assert.True(player.Interrupt(zoneManager, healingPotion));

        Assert.Equal(80, player.Health);
        Assert.True(player.HasUsedInterrupt);

        Assert.False(player.Interrupt(zoneManager,
            new Item("Potion", "", 1f, 10, Category.Consumable, Rarity.Common)));

        currentEvent = zoneManager.PopNextEvent(startingZone)!;
        Assert.Equal(EventType.Combat, currentEvent.Type);

        var banditEnemy = new Enemy("Bandit", 15, 5, 0, Difficulty.Easy);

        var combatResult = combatManager.BeginCombat(player, banditEnemy);

        Assert.Equal(EventType.Loot, combatResult?.Type);

        gameManager.ApplyCombatScore(Difficulty.Easy);
        player.ClearEvent(currentEvent);

        // === FOREST ===

        Assert.True(player.CanMove(zoneManager, "Forest"));
        Assert.True(player.MovePlayer(zoneManager, "Forest"));

        Assert.False(player.HasUsedInterrupt);
        Assert.Equal("Forest", zoneManager.CurrentZone?.Name);

        currentEvent = zoneManager.PopNextEvent(forestZone)!;
        gameManager.ApplyEffects(currentEvent, zoneManager);

        currentEvent = zoneManager.PopNextEvent(forestZone)!;
        Assert.Equal(EventType.Loot, currentEvent.Type);
        gameManager.ApplyEffects(currentEvent, zoneManager);

        var shieldItem = player.Inventory.FirstOrDefault(i => i.Name == "Shield");

        if (shieldItem != null)
        {
            int defenceBefore = player.Defence;
            Assert.True(player.Interrupt(zoneManager, shieldItem));
            Assert.True(player.Defence > defenceBefore);
        }

        currentEvent = zoneManager.PopNextEvent(forestZone)!;
        Assert.Equal(EventType.Combat, currentEvent.Type);

        var wolfEnemy = new Enemy("Wolf Pack", 40, 12, 3, Difficulty.Hard);

        player.Health = 100;

        combatResult = combatManager.BeginCombat(player, wolfEnemy);

        Assert.NotNull(combatResult);

        if (combatResult!.Type == EventType.Loot)
        {
            gameManager.ApplyCombatScore(Difficulty.Hard);
            Assert.True(player.Score >= GameManager.ScoreHard);
        }

        player.ClearEvent(ruinsGateGuardEvent);

        // === RUINS ===

        Assert.True(player.CanMove(zoneManager, "Ruins"));
        Assert.True(player.MovePlayer(zoneManager, "Ruins"));

        currentEvent = zoneManager.PopNextEvent(ruinsZone)!;
        gameManager.ApplyEffects(currentEvent, zoneManager);

        currentEvent = zoneManager.PopNextEvent(ruinsZone)!;
        Assert.Equal(EventType.Campfire, currentEvent.Type);

        gameManager.ApplyEffects(currentEvent, zoneManager);
        Assert.Equal(100, player.Health);

        currentEvent = zoneManager.PopNextEvent(ruinsZone)!;
        gameManager.ApplyEffects(currentEvent, zoneManager);

        currentEvent = zoneManager.PopNextEvent(ruinsZone)!;

        var skeletonEnemy = new Enemy("Skeleton", 35, 10, 5, Difficulty.Hard);

        player.Health = 100;

        combatResult = combatManager.BeginCombat(player, skeletonEnemy);

        Assert.NotNull(combatResult);

        gameManager.ApplyCombatScore(Difficulty.Hard);

        player.Health = 30;

        var elixirItem = player.Inventory.FirstOrDefault(i => i.Name == "Elixir");

        if (elixirItem != null)
        {
            Assert.True(player.Interrupt(zoneManager, elixirItem));
            Assert.Equal(100, player.Health);
        }

        // === FORTRESS ===

        Assert.True(player.CanMove(zoneManager, "Fortress"));
        Assert.True(player.MovePlayer(zoneManager, "Fortress"));

        currentEvent = zoneManager.PopNextEvent(fortressZone)!;
        Assert.Equal(EventType.Combat, currentEvent.Type);

        var bossEnemy = new Enemy("Dark Lord", 80, 15, 5, Difficulty.Boss);

        player.Health = 100;

        combatResult = combatManager.BeginCombat(player, bossEnemy);

        Assert.NotNull(combatResult);

        if (combatResult!.Type == EventType.Loot)
        {
            gameManager.ApplyCombatScore(Difficulty.Boss);
            gameManager.ApplyEffects(combatResult, zoneManager);
        }

        currentEvent = zoneManager.PopNextEvent(fortressZone)!;
        Assert.Equal(EventType.Loot, currentEvent.Type);

        gameManager.ApplyEffects(currentEvent, zoneManager);

        Assert.NotEmpty(combatManager.GetLog());
        Assert.NotEmpty(combatManager.GetLogLines());

        bool isTopTen = gameManager.RegisterScore(player);

        var leaderboardText = gameManager.PrintTopTen();

        Assert.Contains("Top 10", leaderboardText);

        gameManager.SaveRankings("/tmp/qf_testgame_lb.json");

        var newGameManager = new GameManager();
        newGameManager.LoadRankings("/tmp/qf_testgame_lb.json");

        Assert.True(newGameManager.Leaderboard.GetTopScores().Any());

        Assert.True(gameManager.Unregister(playerId));
        Assert.Null(gameManager.ActivePlayer);
    }
}