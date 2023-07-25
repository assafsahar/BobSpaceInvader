using COD.Core;
using System.Collections.Generic;
using static COD.Shared.GameEnums;

namespace COD.GameLogic
{
    public class CODScoreManager
    {
        public CODPlayerScoreData PlayerScoreData = new();
        public CODScoreManager()
        {
            /*CODManager.Instance.SaveManager.Load<CODPlayerScoreData>(delegate (CODPlayerScoreData data)
            {
                PlayerScoreData = data ?? new CODPlayerScoreData();
            });*/
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
            CODManager.Instance.EventsManager.InvokeEvent(CODEventNames.OnScoreSet, (tag, amount));
            PlayerScoreData.ScoreByTag[tag] = amount;
            //CODDebug.Log($" set score {amount}");
            //CODManager.Instance.SaveManager.Save(PlayerScoreData);
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
    }

    public class CODPlayerScoreData
    {
        public Dictionary<ScoreTags, int> ScoreByTag = new();
    }
}