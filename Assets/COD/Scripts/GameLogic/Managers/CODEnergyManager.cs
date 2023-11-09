using COD.Core;
using COD.Shared;
using UnityEngine;

namespace COD.GameLogic
{
    /// <summary>
    /// manages the energy systems within the game, 
    /// tracking energy levels, consumption, regeneration, 
    /// and interactions with energy-related game elements.
    /// </summary>
    public class CODEnergyManager
    {
        public float MaxEnergy { get; private set; }
        public float CurrentEnergy { get; private set; }
        public float EnergyDecreaseRate { get; private set; }


        public CODEnergyManager(float maxEnergy, float initialEnergy, float energyDecreaseRate)
        {
            MaxEnergy = maxEnergy;
            CurrentEnergy = initialEnergy;
            EnergyDecreaseRate = energyDecreaseRate;
        }

        public void UpdateEnergy(float deltaTime)
        {
            CurrentEnergy -= EnergyDecreaseRate * deltaTime;
            EnergyData energyData = new EnergyData(CurrentEnergy, MaxEnergy);
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnEnergyChanged, energyData);
            if (CurrentEnergy <= 0)
            {
                CurrentEnergy = 0;
                CODGameLogicManager.Instance.GameFlowManager.ChangeToFallState();
                //CODGameLogicManager.Instance.GameFlowManager.EndGame();
            }
        }

        public void AddEnergy(float amount)
        {
            CurrentEnergy = Mathf.Min(CurrentEnergy + amount, MaxEnergy);
            EnergyData energyData = new EnergyData(CurrentEnergy, MaxEnergy);
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnEnergyChanged, energyData);
        }
        public void ResetEnergy()
        {
            CurrentEnergy = MaxEnergy;
        }
    }
}

