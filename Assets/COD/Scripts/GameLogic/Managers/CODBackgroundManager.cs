using COD.Core;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// This class is in charge of the game's background elements. 
    /// It controls the scrolling, parallax effects, or dynamic changes 
    /// to the background as the game progresses
    /// </summary>
    public class CODBackgroundManager : CODMonoBehaviour
    {
        [System.Serializable]
        public class ParallaxLayer
        {
            public Transform image; 
            public float speed;
            public float resetPoint; // This is where we'll reset to the original position
            public float loopPoint;  // This is where the image should loop
        }

        public ParallaxLayer[] layers;

        private Vector3[] initialPositions;

        private void OnEnable()
        {
            AddListener(CODEventNames.OnSpeedChange, UpdateSpeed);
        }
        private void OnDisable()
        {
            RemoveListener(CODEventNames.OnSpeedChange, UpdateSpeed);
        }
        private void Start()
        {
            // Store the initial positions
            initialPositions = new Vector3[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                initialPositions[i] = layers[i].image.position;
            }
        }

        private void Update()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                // Move the image to the left
                layers[i].image.position += Vector3.left * layers[i].speed * Time.deltaTime;

                // Check if the image has moved past the loop point
                if (layers[i].image.position.x <= layers[i].loopPoint)
                {
                    // Reset the image to its original position
                    layers[i].image.position = initialPositions[i] + Vector3.left * layers[i].resetPoint;
                }
            }
        }
        private void UpdateSpeed(object obj)
        {
            float baseSpeed = (float)obj;
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].speed = baseSpeed * Mathf.Pow(1.3f, i);
            }
        }
    }
}
