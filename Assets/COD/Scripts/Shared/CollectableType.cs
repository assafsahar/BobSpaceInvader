namespace COD.Shared
{
    /// <summary>
    /// different types of collectable items within the game. 
    /// Each item that can be collected by the player (like coins, power-ups, etc.) 
    /// would have a corresponding type in this enumeration.
    /// </summary>
    public enum CollectableType
    {
        Coin = 1,
        Energy = 2,
        Bomb = 3,
        SuperCoin = 4,
        Shield = 5,
        Magnet = 6,
        Shooting = 7
    }
}