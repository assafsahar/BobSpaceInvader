using UnityEngine;
using DG.Tweening;
using COD.Core;

namespace COD.UI
{
    public class CODToastMessage : CODPoolable
    {
        public TextMesh textMesh;
        public float moveDuration = 1f;
        public float fadeDuration = 0.5f;
        public Vector3 targetPosition;

        private void Awake()
        {
            textMesh = GetComponent<TextMesh>();
        }

        public void Initialize(string text, Vector3 startPosition, Vector3 targetPos)
        {
            transform.position = startPosition;
            textMesh.text = text;
            targetPosition = targetPos;

            Color textColor = textMesh.color;
            textColor.a = 1;
            textMesh.color = textColor;

            StartMovement();
        }

        private void StartMovement()
        {
            // Move towards the target position
            transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutQuad);

            // Fade out
            DOTween.To(() => textMesh.color, x => textMesh.color = x, new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0), fadeDuration)
                   .SetDelay(moveDuration * 0.5f)
                   .OnComplete(() =>
                   {
                       gameObject.SetActive(false);
                       CODManager.Instance.PoolManager.ReturnPoolable(this);
                   });
        }
    }
}
