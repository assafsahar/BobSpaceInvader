using COD.Core;
using COD.Shared;
using System;
using TMPro;
using UnityEngine;

namespace COD.UI
{
    /// <summary>
    /// Manages the display of the player's energy in the UI. 
    /// It listens for changes in the player's energy level 
    /// and updates the energy display accordingly.
    /// </summary>
    public class CODEnergyDisplay : CODMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI energyText;

        private void OnEnable()
        {
            CODManager.Instance.EventsManager.AddListener(CODEventNames.OnEnergyChanged, HandleEnergyChanged);
        }

        private void OnDisable()
        {
            CODManager.Instance.EventsManager.RemoveListener(CODEventNames.OnEnergyChanged, HandleEnergyChanged);
        }
        private void HandleEnergyChanged(object obj)
        {
            if (obj is EnergyData energyData)
            {
                UpdateEnergyUI(energyData.CurrentEnergy, energyData.MaxEnergy);
            }
        }
        public void UpdateEnergyUI(float currentEnergy, float? maxEnergy)
        {
            if (maxEnergy.HasValue)
            {
                energyText.text = $"{Math.Ceiling(currentEnergy)}/{maxEnergy}";
            }
            else
            {
                energyText.text = $"{Math.Ceiling(currentEnergy)}";
            }
        }
    }
}
