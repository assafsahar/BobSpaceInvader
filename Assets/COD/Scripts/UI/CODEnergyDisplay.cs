using COD.Core;
using COD.Shared;
using DG.Tweening;
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
        [SerializeField] private float criticalTextScale = 1.6f;
        private Sequence sequence;

        private void OnEnable()
        {
            CODManager.Instance.EventsManager.AddListener(CODEventNames.OnEnergyChanged, HandleEnergyChanged);
            CODManager.Instance.EventsManager.AddListener(CODEventNames.OnEnergyCritical, HandleEnergyCritical);
        }

        private void OnDisable()
        {
            CODManager.Instance.EventsManager.RemoveListener(CODEventNames.OnEnergyChanged, HandleEnergyChanged);
            CODManager.Instance.EventsManager.RemoveListener(CODEventNames.OnEnergyCritical, HandleEnergyCritical);
            sequence?.Kill();
        }
        
        private void HandleEnergyChanged(object obj)
        {
            if (obj is EnergyData energyData)
            {
                UpdateEnergyUI(energyData.CurrentEnergy, energyData.MaxEnergy);
            }
        }
        private void HandleEnergyCritical(object isCritical)
        {
            bool critical = (bool)isCritical;
            if (critical)
            {
                if (sequence == null || !sequence.IsPlaying())
                {
                    StartLowEnergyAlert();
                }
            }
            else
            {
                StopLowEnergyAlert();
            }
        }
        private void StartLowEnergyAlert()
        {
            if (sequence != null && sequence.IsActive()) sequence.Kill();
            sequence = DOTween.Sequence()
                .Append(energyText.transform.DOScale(new Vector3(criticalTextScale, criticalTextScale, 1f), 1f).SetEase(Ease.InOutSine))
                .Append(energyText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Restart);
        }
        private void StopLowEnergyAlert()
        {
            sequence?.Kill();
            energyText.transform.localScale = Vector3.one;
        }
        private void UpdateEnergyUI(float currentEnergy, float? maxEnergy)
        {
            energyText.text = maxEnergy.HasValue ? $"{Mathf.Ceil(currentEnergy)}/{maxEnergy}" : $"{Mathf.Ceil(currentEnergy)}";
        }
    }
}
