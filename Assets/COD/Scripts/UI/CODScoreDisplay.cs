using COD.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static COD.Shared.GameEnums;

namespace COD.UI
{
    public class CODScoreDisplay : CODMonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text superCoinText;
        [SerializeField] private TMP_Text scoreText;

        private void OnEnable()
        {
            AddListener(CODEventNames.OnScoreSet, UpdateScoreDisplay);
            InvokeEvent(CODEventNames.RequestScoreUpdate);

        }

        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnScoreSet, UpdateScoreDisplay);
        }


        private void UpdateScoreDisplay(object data)
        {
            if (data is ValueTuple<ScoreTags, int> scoreData)
            {
                Debug.Log($"UI Update for {scoreData.Item1} with score: {scoreData.Item2}");
                switch (scoreData.Item1)
                {
                    case ScoreTags.Coin:
                        coinText.text = $"{scoreData.Item2}";
                        break;
                    case ScoreTags.SuperCoin:
                        superCoinText.text = $"{scoreData.Item2}";
                        break;
                    case ScoreTags.MainScore:
                        scoreText.text = $"{scoreData.Item2}";
                        break;
                }
            }
        }
    }
}
