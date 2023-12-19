using COD.Core;
using COD.GameLogic;
using UnityEngine;
using static COD.GameLogic.CODGameFlowManager;

public class CODStartScreenController : MonoBehaviour
{
    public GameObject startScreenUI;
    public CODGameFlowManager gameFlowManager; 

    private void OnEnable()
    {
        CODManager.Instance.EventsManager.AddListener(CODEventNames.OnTouchEnded, StartGameOnTouch);
    }

    private void OnDisable()
    {
        CODManager.Instance.EventsManager.RemoveListener(CODEventNames.OnTouchEnded, StartGameOnTouch);
    }

    void Start()
    {
        gameFlowManager = FindObjectOfType<CODGameFlowManager>();
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        //CODDebug.Log("[CODStartScreenController] Showing start screen");
        startScreenUI.SetActive(true);
        /*if (gameFlowManager != null)
        {
            gameFlowManager.StartGame();
        }*/
    }

    private void StartGameOnTouch(object data)
    {
        //CODDebug.Log("[CODStartScreenController] StartGameOnTouch called");
        startScreenUI.SetActive(false);
        if (gameFlowManager != null && gameFlowManager.CurrentState == GameState.WaitingToStart)
        {
            gameFlowManager.StartGame();
        }
    }
}
