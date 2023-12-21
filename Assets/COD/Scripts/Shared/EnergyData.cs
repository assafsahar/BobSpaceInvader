namespace COD.Shared
{
    /// <summary>
    /// a structure that encapsulates data about energy within the game. 
    /// This could include the current energy level, maximum energy capacity, 
    /// and any other relevant information that needs to be tracked. 
    /// The EnergyData might be used in events or functions that handle 
    /// changes to the player's energy, such as collecting energy items 
    /// or spending energy to perform actions
    /// </summary>
    public struct EnergyData
    {
        public float CurrentEnergy;
        public float? MaxEnergy;

        public EnergyData(float currentEnergy, float? maxEnergy = null)
        {
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
        }
    }
}