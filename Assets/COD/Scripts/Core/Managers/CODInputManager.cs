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
        }
    }
}
