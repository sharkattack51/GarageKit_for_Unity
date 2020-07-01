using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit
{
    public class SequenceSprite : MonoBehaviour
    {
        public Image uiImage;
        public Sprite[] sprites;

        private int frame = 0;
        private float preUpdtaeTime = 0.0f;


        void Awake()
        {

        }
        
        void Start()
        {

        }

        void Update()
        {

        }


        public void ResetSequence()
        {
            frame = 0;
            uiImage.sprite = sprites[frame];

            StopSequence();
        }

        public void StartSequence()
        {
            ResetSequence();
            StartCoroutine(StartSequenceCoroutine());
        }

        public void StopSequence()
        {
            StopAllCoroutines();
        }

        private IEnumerator StartSequenceCoroutine()
        {
            while(true)
            {
                uiImage.sprite = sprites[frame];
                if(Time.timeSinceLevelLoad - preUpdtaeTime >= (1.0f / 30.0f))
                {
                    preUpdtaeTime = Time.timeSinceLevelLoad;

                    frame++;
                    if(frame >= sprites.Length)
                        break;
                }

                yield return null;
            }

            yield break;
        }
    }
}
