namespace QuestForge.Engine.Managers;

using QuestForge.Engine.Models;

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = "";
    public int Score { get; set; }
}

public class Leaderboard
{
    private const int MaxEntries = 10;
    private SortedSet<LeaderboardEntry> _scores = new(new ScoreComparer());

    public void Initialise()
    {
        _scores.Clear();
        _scores.Add(new LeaderboardEntry { PlayerName = "Aria",  Score = 500 });
        _scores.Add(new LeaderboardEntry { PlayerName = "Zara",  Score = 350 });
        _scores.Add(new LeaderboardEntry { PlayerName = "Kael",  Score = 200 });
        _scores.Add(new LeaderboardEntry { PlayerName = "Lyra",  Score = 100 });
        _scores.Add(new LeaderboardEntry { PlayerName = "Orin",  Score = 50  });
    }

    public bool AddScore(Player player, int score)
    {
        var entry = new LeaderboardEntry { PlayerName = player.Name, Score = score };
        _scores.Add(entry);

        while (_scores.Count > MaxEntries)
            _scores.Remove(_scores.Min!);

        return _scores.Contains(entry);
    }

    public IEnumerable<LeaderboardEntry> GetTopScores() => _scores.Reverse();

    public string PrintTopTen()
    {
        var lines = new List<string> { "=== Top 10 ===" };
        int rank = 1;
        foreach (var entry in GetTopScores())
            lines.Add($"{rank++}. {entry.PlayerName} - {entry.Score}");
        return string.Join("\n", lines);
    }

    public List<LeaderboardEntry> ToList() => _scores.ToList();

    public void FromList(List<LeaderboardEntry> entries)
    {
        _scores.Clear();
        foreach (var e in entries) _scores.Add(e);
    }

    private class ScoreComparer : IComparer<LeaderboardEntry>
    {
        public int Compare(LeaderboardEntry? x, LeaderboardEntry? y)
        {
            if (x == null || y == null) return 0;
            int cmp = x.Score.CompareTo(y.Score);
            if (cmp != 0) return cmp;
            return string.Compare(x.PlayerName, y.PlayerName, StringComparison.Ordinal);
        }
    }
}