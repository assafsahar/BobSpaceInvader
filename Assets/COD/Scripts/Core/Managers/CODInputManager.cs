using UnityEngine;

namespace COD.Core
{
    public class InputManager : CODMonoBehaviour
    {
        private Vector2 startPos;

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InvokeEvent(CODEventNames.OnTouchStarted, Input.mousePosition.y);
            }
            else if (Input.GetMouseButton(0))
            {
                InvokeEvent(CODEventNames.OnTouchStayed, Input.mousePosition.y);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                InvokeEvent(CODEventNames.OnTouchEnded, null);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }

        private void QuitGame()
        {
            Debug.Log("QuitGame");
            // If we are running in a standalone build of the game
#if UNITY_STANDALONE
            // Quit the application
            Application.Quit();
#endif

            // If we are running in the editor
#if UNITY_EDITOR
            Debug.Log("Editor");
            // Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
        }
    }
}
