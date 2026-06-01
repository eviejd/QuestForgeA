using QuestForge.Engine.Managers;
using QuestForge.Engine.Models;

namespace QuestForge.Tests;

public class LeaderboardTests
{
    private string CreateTemporaryLeaderboardPath() =>
        Path.Combine(Path.GetTempPath(), $"questforge_lb_{Guid.NewGuid()}.json");

    [Fact]
    public void SaveAndLoad_RoundTripsSuccessfully()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        var player = new Player("Aria") { Score = 999 };
        gameManager.RegisterScore(player);

        var tempFilePath = CreateTemporaryLeaderboardPath();

        gameManager.SaveRankings(tempFilePath);

        var reloadedGameManager = new GameManager();
        reloadedGameManager.LoadRankings(tempFilePath);

        Assert.True(
            reloadedGameManager.Leaderboard.GetTopScores()
                .Any(entry => entry.PlayerName == "Aria")
        );

        File.Delete(tempFilePath);
    }

    [Fact]
    public void LoadRankings_MissingFile_UsesDefaultLeaderboard()
    {
        var gameManager = new GameManager();

        gameManager.LoadRankings("/tmp/doesnotexist_qf.json");

        Assert.NotEmpty(gameManager.Leaderboard.GetTopScores());
    }

    [Fact]
    public void LoadRankings_CorruptFile_UsesDefaultLeaderboard()
    {
        var corruptFilePath = CreateTemporaryLeaderboardPath();

        File.WriteAllText(corruptFilePath, "this is not valid json {{{");

        var gameManager = new GameManager();
        gameManager.LoadRankings(corruptFilePath);

        Assert.NotEmpty(gameManager.Leaderboard.GetTopScores());

        File.Delete(corruptFilePath);
    }

    [Fact]
    public void RegisterScore_ReturnsTrue_WhenPlayerEntersTopTen()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        var highScoringPlayer = new Player("Aria") { Score = 9999 };

        Assert.True(gameManager.RegisterScore(highScoringPlayer));
    }

    [Fact]
    public void RegisterScore_EnforcesMaximumOfTenEntries()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        for (int i = 0; i < 20; i++)
        {
            var player = new Player($"Player{i}") { Score = i * 10 };
            gameManager.RegisterScore(player);
        }

        Assert.True(gameManager.Leaderboard.GetTopScores().Count() <= 10);
    }

    [Fact]
    public void RegisterScore_RemovesLowestScoreWhenOverflowing()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        for (int i = 0; i < 10; i++)
        {
            var player = new Player($"Player{i}") { Score = 1000 + i };
            gameManager.RegisterScore(player);
        }

        var lowScorePlayer = new Player("LowScorePlayer") { Score = 1 };

        gameManager.RegisterScore(lowScorePlayer);

        Assert.False(
            gameManager.Leaderboard.GetTopScores()
                .Any(entry => entry.PlayerName == "LowScorePlayer")
        );
    }

    [Fact]
    public void PrintTopTen_ContainsHeaderText()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        Assert.Contains("Top 10", gameManager.PrintTopTen());
    }

    [Fact]
    public void PrintTopTen_ReturnsScoresInDescendingOrder()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        var leaderboardEntries = gameManager.Leaderboard.GetTopScores().ToList();

        for (int i = 0; i < leaderboardEntries.Count - 1; i++)
        {
            Assert.True(leaderboardEntries[i].Score >= leaderboardEntries[i + 1].Score);
        }
    }

    [Fact]
    public void Initialise_SeedsDefaultLeaderboardEntries()
    {
        var leaderboard = new Leaderboard();

        leaderboard.Initialise();

        Assert.NotEmpty(leaderboard.GetTopScores());
    }

    [Fact]
    public void SaveRankings_ReturnsFalse_WhenPathIsInvalid()
    {
        var gameManager = new GameManager();
        gameManager.Leaderboard.Initialise();

        Assert.False(
            gameManager.SaveRankings("/invalid/path/that/does/not/exist/lb.json")
        );
    }
}