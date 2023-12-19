using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace COD.Core
{
    public class CODLoadBarComponent : CODMonoBehaviour
    {
        [SerializeField] private Image loadingImage;
        [SerializeField] private TMP_Text loaderNumber;
        [SerializeField] private float fillSpeed = 1;
        [SerializeField] float animationDuration = 1.0f;

        public event Action OnLoadingStepComplete;

        private float targetAmount = 0;
        private int currentAmount = 0;
        private bool isAnimating = false;

        private void Awake()
        {
            loadingImage.fillAmount = 0;
            UpdateView();
        }
        public void SetTargetAmount(float amount)
        {
            if (isAnimating)
            {
                StopAllCoroutines();
            }
            targetAmount = amount;
            UpdateView();
        }

        private void UpdateView()
        {
            //CODDebug.Log(targetAmount);
            loadingImage.DOFillAmount(targetAmount / 100, fillSpeed).SetEase(Ease.Linear);
            StartCoroutine(AnimateToTargetAmount((int)targetAmount));
        }

        private IEnumerator AnimateToTargetAmount(int targetAmount)
        {
            isAnimating = true;
            float elapsedTime = 0.0f;
            int startAmount = currentAmount;

            while (elapsedTime < animationDuration) 
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / animationDuration);
                currentAmount = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
                loaderNumber.text = currentAmount.ToString("N0") + "%";
                yield return null;
            }

            currentAmount = targetAmount;
            loaderNumber.text = currentAmount.ToString("N0") + "%";
            isAnimating = false;
            OnLoadingStepComplete?.Invoke();
        }
    }
}