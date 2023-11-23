using COD.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static COD.Shared.GameEnums;

namespace COD.GameLogic
{
    /// <summary>
    /// Responsible for managing the player's score. Can handle different score types
    /// </summary>
    public class CODScoreManager
    {
        public bool IsInitialized { get; private set; } = false;
        public CODPlayerScoreData PlayerScoreData = new();
        private CODPlayerScoreData initialScoreData;
        public CODScoreManager()
        {
            CODManager.Instance.EventsManager.AddListener(CODEventNames.RequestScoreUpdate, PushCurrentScores);
        }
        ~CODScoreManager()
        {
            // Remove listener
            CODManager.Instance.EventsManager.RemoveListener(CODEventNames.RequestScoreUpdate, PushCurrentScores);
        }

        public void Initialize()
        {
            InitializeScores(() =>
            {
                Debug.Log("Scores have been initialized!");
            });
        }

        public bool TryGetScoreByTag(ScoreTags tag, ref int scoreOut)
        {
            if (PlayerScoreData.ScoreByTag.TryGetValue(tag, out var score))
            {
                scoreOut = score;
                return true;
            }

            return false;
        }

        public void SetScoreByTag(ScoreTags tag, int amount = 0)
        {
            PlayerScoreData.ScoreByTag[tag] = amount;
            UpdateHighestScore(tag, amount);
            NotifyScoreChange(tag, amount);
            //Debug.Log($"Score for {tag} set to: {amount}");
            //CODManager.Instance.SaveManager.Save(PlayerScoreData);
        }

        public void AddScoreByTag(ScoreTags tag, int amount = 0)
        {
            int currentScore = GetCurrentScore(tag);
            var score = currentScore + amount;
            if (TryGetScoreByTag(tag, ref score))
            {
                SetScoreByTag(tag, score);
            }
        }

        public void ChangeScoreByTagByAmount(ScoreTags tag, int amount = 0)
        {
            if (PlayerScoreData.ScoreByTag.ContainsKey(tag))
            {
                SetScoreByTag(tag, PlayerScoreData.ScoreByTag[tag] + amount);
            }
            else
            {
                SetScoreByTag(tag, amount);
            }
        }

        public bool TryUseScore(ScoreTags scoreTag, int amountToReduce, bool makeTheReduction = true)
        {
            var score = 0;
            var hasType = TryGetScoreByTag(scoreTag, ref score);
            var hasEnough = false;

            if (hasType)
            {
                hasEnough = amountToReduce <= score;
            }

            if (hasEnough && makeTheReduction)
            {
                ChangeScoreByTagByAmount(scoreTag, -amountToReduce);
            }

            return hasEnough;
        }
        public int GetCurrentScore(ScoreTags tag)
        {
            if (PlayerScoreData.ScoreByTag.TryGetValue(tag, out var score))
            {
                return score;
            }
            return 0; // or some default value if the tag does not exist
        }

        public void ResetToInitialState()
        {
            PlayerScoreData = initialScoreData.Clone();
            // Notify systems that the score has been reset (UI might need updating).
            foreach (var pair in PlayerScoreData.ScoreByTag)
            {
                CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (pair.Key, pair.Value));
            }
        }
        public void AddDistance(int distance)
        {
            PlayerScoreData.AccumulatedDistance += distance;
            ChangeScoreByTagByAmount(ScoreTags.Distance, distance);
            ChangeScoreByTagByAmount(ScoreTags.MainScore, distance);
            NotifyAccumulatedDistanceChange(PlayerScoreData.AccumulatedDistance);
        }
        public int GetCurrentDistance()
        {
            return GetCurrentScore(ScoreTags.Distance);
        }
        public void CalculateScore()
        {
            int score = GetCurrentScore(ScoreTags.MainScore) + GetCurrentDistance(); 
        }
        public void ResetGameScores()
        {
            SetScoreByTag(ScoreTags.MainScore, 0);
            SetScoreByTag(ScoreTags.Distance, 0);
            // Reset any other scores as needed
        }
        public int GetHighestScore(ScoreTags tag)
        {
            return PlayerScoreData.HighestScoresByTag.TryGetValue(tag, out var highestScore) ? highestScore : 0;
        }
        public void ResetCurrentScores()
        {
            foreach (var tag in Enum.GetValues(typeof(ScoreTags)))
            {
                SetScoreByTag((ScoreTags)tag, 0);
            }
        }

        private void NotifyAccumulatedDistanceChange(float newDistance)
        {
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnAccumulatedDistanceUpdated, newDistance);
        }
        private void UpdateHighestScore(ScoreTags tag, int newScore)
        {
            if (!PlayerScoreData.HighestScoresByTag.ContainsKey(tag) || PlayerScoreData.HighestScoresByTag[tag] < newScore)
            {
                PlayerScoreData.HighestScoresByTag[tag] = newScore;
            }
        }
        private void NotifyScoreChange(ScoreTags tag, int newScore)
        {
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (tag, newScore));
        }
        
        private void PushCurrentScores(object unused)
        {
            foreach (var pair in PlayerScoreData.ScoreByTag)
            {
                CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (pair.Key, pair.Value));
            }
        }
        private void InitializeScores(Action onCompleted = null)
        {
            CODManager.Instance.SaveManager.Load<CODPlayerScoreData>(data =>
            {
                PlayerScoreData = data ?? new CODPlayerScoreData();
                initialScoreData = PlayerScoreData.Clone();

                foreach (var pair in PlayerScoreData.ScoreByTag)
                {
                    CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (pair.Key, pair.Value));
                }
                IsInitialized = true;
                onCompleted?.Invoke();
            });
        }
    }

    public class CODPlayerScoreData : ICODSaveData
    {
        public Dictionary<ScoreTags, int> ScoreByTag = new();
        public Dictionary<ScoreTags, int> HighestScoresByTag = new();
        public float AccumulatedDistance { get; set; }
        public CODPlayerScoreData Clone()
        {
            return new CODPlayerScoreData
            {
                ScoreByTag = new Dictionary<ScoreTags, int>(this.ScoreByTag),
                HighestScoresByTag = new Dictionary<ScoreTags, int>(this.ScoreByTag),
                AccumulatedDistance = this.AccumulatedDistance
            };
        }
    }
}