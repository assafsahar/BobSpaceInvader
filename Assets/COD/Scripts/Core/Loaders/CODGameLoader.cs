using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COD.Core
{
    /// <summary>
    /// Loads the different game managers
    /// </summary>
    public class CODGameLoader : CODGameLoaderBase
    {
        [SerializeField] private CODGameLoaderBase gameLogicLoader;
        [SerializeField] private CODLoadBarComponent loadbarComponent;

        private List<float> loadingSteps = new List<float> { 40, 98, 100 };
        private int currentStepIndex = 0;

        private void OnEnable()
        {
            loadbarComponent.OnLoadingStepComplete += OnLoadingStepComplete;
        }
        private void OnDisable()
        {
            loadbarComponent.OnLoadingStepComplete -= OnLoadingStepComplete;
        }

        private void Start()
        {
            WaitForSeconds(0.5f, DelayStart);
        }

        private void OnLoadingStepComplete()
        {
            if (currentStepIndex < loadingSteps.Count - 1)  
            {
                SetNextLoadingTarget();
            }
            else
            {
                FinalizeLoading();
            }
        }
        private void SetNextLoadingTarget()
        {
            if (currentStepIndex < loadingSteps.Count)
            {
                float nextStep = loadingSteps[currentStepIndex++];
                loadbarComponent.SetTargetAmount(nextStep);

                if (currentStepIndex == 1)  
                {
                    gameLogicLoader.StartLoad(() =>
                    {
                        WaitForSeconds(0.5f, SetNextLoadingTarget);
                    });
                }
            }
        }
        private void FinalizeLoading()
        {
            // Final step logic
            SceneManager.LoadScene(1);
            Destroy(gameObject);
        }
        private void DelayStart()
        {
            var manager = new CODManager();
            manager.LoadManager(() =>
            {
                CODDebug.Log("LoadManager");
                SetNextLoadingTarget();
            });
        }
    }
}

