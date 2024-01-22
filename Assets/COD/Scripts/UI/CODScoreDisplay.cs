using COD.Core;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static COD.Shared.GameEnums;

namespace COD.UI
{
    /// <summary>
    /// Responsible for updating and displaying the score UI elements 
    /// in the game. It reacts to events that change the player's score 
    /// and updates the relevant UI components with the new score values.
    /// </summary>
    public class CODScoreDisplay : CODMonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text superCoinText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text distanceText;

        [SerializeField] private float tweenFontSize = 50f;
        [SerializeField] private float tweenFontDuration = 0.4f;

        private void OnEnable()
        {
            AddListener(CODEventNames.OnScoreSet, UpdateScoreDisplay);
            //AddListener(CODEventNames.OnDistanceSet, UpdateDistanceDisplay);
            AddListener(CODEventNames.OnAccumulatedDistanceUpdated, UpdateAccumulatedDistanceDisplay);
            InvokeEvent(CODEventNames.RequestScoreUpdate);

        }

        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnScoreSet, UpdateScoreDisplay);
            //RemoveListener(CODEventNames.OnDistanceSet, UpdateDistanceDisplay);
        }


        private void UpdateAccumulatedDistanceDisplay(object obj)
        {
            //CODDebug.Log("UpdateAccumulatedDistanceDisplay " + obj.ToString());
        }

        private void UpdateScoreDisplay(object data)
        {
            if (data is ValueTuple<ScoreTags, int> scoreData)
            {
                //CODDebug.Log($"UI Update for {scoreData.Item1} with score: {scoreData.Item2}");
                switch (scoreData.Item1)
                {
                    case ScoreTags.Coin:
                        StartCoroutine(AnimateTextCoroutine(coinText, scoreData.Item2));

                        break;
                    case ScoreTags.SuperCoin:
                        StartCoroutine(AnimateTextCoroutine(superCoinText, scoreData.Item2));
                        break;
                    case ScoreTags.MainScore:
                        scoreText.text = $"{scoreData.Item2}";
                        break;
                    case ScoreTags.Distance:
                        distanceText.text = $"{scoreData.Item2}";
                        break;
                }
            }
        }
       
        private IEnumerator AnimateTextCoroutine(TMP_Text textComponent, int newValue)
        {
            textComponent.text = $"{newValue}";
            float originalSize = textComponent.fontSize;
            float elapsedTime = 0;

            while (elapsedTime < tweenFontDuration / 2)
            {
                textComponent.fontSize = Mathf.Lerp(originalSize, tweenFontSize, elapsedTime / (tweenFontDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0;
            while (elapsedTime < tweenFontDuration / 2)
            {
                textComponent.fontSize = Mathf.Lerp(tweenFontSize, originalSize, elapsedTime / (tweenFontDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            textComponent.fontSize = originalSize;
        }
        private void UpdateDistanceDisplay(object data)
        {
            /*if (data is int distance)
            {
                distanceText.text = $"{distance:F0}"; 
            }*/
        }
    }
}
