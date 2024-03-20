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
        }

        public ParallaxLayer[] layers;

        private float[] initialPositions;
        private float[] singleTextures;

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
            // Store the initial positions and texture width
            initialPositions = new float[layers.Length];
            singleTextures = new float[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                Sprite sprite = layers[i].image.GetComponent<SpriteRenderer>().sprite;
                singleTextures[i] = sprite.texture.width / sprite.pixelsPerUnit;
                initialPositions[i] = layers[i].image.transform.position.x;
            }
        }

        private void Update()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                float delta = layers[i].speed * Time.deltaTime;

                // Move the image to the left
                layers[i].image.transform.position += Vector3.left * delta;

                // Check if the image has moved past the initial position
                if ((Mathf.Abs(layers[i].image.transform.position.x) - singleTextures[i]) > initialPositions[i])
                {
                    // Reset the image to its initial position
                    layers[i].image.transform.position = new Vector3(-initialPositions[i], layers[i].image.transform.position.y, layers[i].image.transform.position.z);
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
