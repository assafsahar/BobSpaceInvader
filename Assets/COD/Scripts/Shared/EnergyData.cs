namespace COD.Shared
{
    public struct EnergyData
    {
        public float CurrentEnergy;
        public float MaxEnergy;

        public EnergyData(float currentEnergy, float maxEnergy)
        {
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
        }
    }
}