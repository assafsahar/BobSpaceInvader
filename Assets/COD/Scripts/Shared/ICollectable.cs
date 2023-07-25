namespace COD.Shared
{
    public interface ICollectable
    {
        CollectableType Type { get; }
        int ScoreValue { get; }
    }
}
