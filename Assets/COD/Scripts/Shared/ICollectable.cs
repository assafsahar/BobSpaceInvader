namespace COD.Shared
{
    /// <summary>
    /// This interface defines the structure and behavior expected of a collectable object 
    /// in the game. Classes that implement this interface would need to provide 
    /// implementations for the properties and methods it declares, 
    /// such as EnergyValue, ScoreValue, and Type.
    /// </summary>
    public interface ICollectable
    {
        CollectableType Type { get; }
        int ScoreValue { get; }
        float EnergyValue { get; set; }
        float MovementSpeed { get; set; }
    }
}
