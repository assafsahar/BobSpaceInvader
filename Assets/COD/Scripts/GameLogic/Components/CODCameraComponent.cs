using COD.Core;
using DG.Tweening;

namespace COD.GameLogic
{
    public class CODCameraComponent : CODMonoBehaviour
    {
        private float shakeDuration = 0.01f;
        private float baseStrengthShake = 0.01f;
        private int shakeVibBase = 1;

        private void OnEnable()
        {
            AddListener(CODEventNames.OnShipCrash, OnCrash);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnShipCrash, OnCrash);
        }
        private void OnCrash(object obj)
        {
            ShakeCamera(30);
        }

        private void ShakeCamera(int multiplyer)
        {
            transform.DOShakePosition(shakeDuration * multiplyer, baseStrengthShake * multiplyer, shakeVibBase);
        }
    }
}