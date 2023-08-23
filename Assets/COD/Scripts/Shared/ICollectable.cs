namespace COD.Shared
{
    public interface ICollectable
    {
        CollectableType Type { get; }
        int ScoreValue { get; }
        int EnergyValue { get; set; }
    }
}
