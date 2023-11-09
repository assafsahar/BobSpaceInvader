using COD.Core;
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
        //[SerializeField] private CODLoadBarComponent loadbarComponent;
        private void Start()
        {
            //loadbarComponent.SetTargetAmount(20);
            WaitForSeconds(0.1f, DelayStart);
        }

        private void DelayStart()
        {
            var manager = new CODManager();
            //loadbarComponent.SetTargetAmount(40);
            manager.LoadManager(() =>
            {
                Debug.Log("LoadManager");
                WaitForSeconds(0.1f, () =>
                {
                    //loadbarComponent.SetTargetAmount(98);
                    gameLogicLoader.StartLoad(() =>
                    {
                        WaitForSeconds(0.1f, () =>
                        {
                            //loadbarComponent.SetTargetAmount(100);
                            SceneManager.LoadScene(1);
                            Destroy(this.gameObject);
                        });
                    });
                });
            });
        }
    }
}

