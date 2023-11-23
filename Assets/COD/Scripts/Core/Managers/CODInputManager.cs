using UnityEngine;

namespace COD.Core
{
    /// <summary>
    /// this class is responsible for abstracting user input, 
    /// allowing the rest of the game to react to user commands 
    /// without being tightly coupled to the input source
    /// </summary>
    public class InputManager : CODMonoBehaviour
    {
        private Vector2 startPos;
        private bool inputEnabled = true;

        private void Update()
        {
            if (!inputEnabled) return;
            HandleInput();
        }
        public void EnableInput(bool enable)
        {
            inputEnabled = enable;
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
            // If we are running in a standalone build of the game
#if UNITY_STANDALONE || UNITY_ANDROID
            Application.Quit();
#endif

            // If we are running in the editor
#if UNITY_EDITOR
            // Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
