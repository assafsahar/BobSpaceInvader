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
        public float? MaxEnergy { get; private set; }
        public float CurrentEnergy { get; private set; }
        public float EnergyDecreaseRate { get; private set; }

        private float defaultEnergyValue = 20f;


        public CODEnergyManager(float? maxEnergy, float initialEnergy, float energyDecreaseRate)
        {
            MaxEnergy = maxEnergy;
            CurrentEnergy = initialEnergy;
            EnergyDecreaseRate = energyDecreaseRate;
        }

        public void UpdateEnergy(float deltaTime)
        {
            CurrentEnergy -= EnergyDecreaseRate * deltaTime;
            if (MaxEnergy.HasValue && CurrentEnergy > MaxEnergy.Value)
            {
                CurrentEnergy = MaxEnergy.Value;
            }
            /*else if (!MaxEnergy.HasValue && CurrentEnergy < 0)
            {
                CurrentEnergy = defaultEnergyValue; 
            }*/
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
            if (MaxEnergy.HasValue)
            {
                CurrentEnergy = Mathf.Min(CurrentEnergy + amount, MaxEnergy.Value);
            }
            else
            {
                CurrentEnergy += amount; 
            }
            EnergyData energyData = new EnergyData(CurrentEnergy, MaxEnergy);
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnEnergyChanged, energyData);
        }
        public void ResetEnergy()
        {
            if (MaxEnergy.HasValue)
            {
                CurrentEnergy = MaxEnergy.Value;
            }
            else
            {
                CurrentEnergy = defaultEnergyValue;
            }
        }
    }
}

