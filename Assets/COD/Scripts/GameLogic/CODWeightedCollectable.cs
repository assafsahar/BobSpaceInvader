using COD.Shared;
using System;

/// <summary>
/// determines how often to spawn a collectable
/// in relation to other collectables
/// </summary>
[Serializable]
public struct WeightedCollectable
{
    public CollectableType collectableType;
    public int weight;
}
