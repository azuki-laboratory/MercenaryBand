namespace MercenaryBand.Data;

public class EnemyCountRange
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class ContractDef
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int BaseReward { get; set; }
    public int TimeDays { get; set; }
    public EnemyCountRange EnemyCount { get; set; } = new();
    public string[] EnemyTypes { get; set; } = [];
}
