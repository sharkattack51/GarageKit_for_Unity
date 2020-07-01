using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class AnimationByStep : MonoBehaviour
    {
        public Animation anim;
        public float speed = 0.1f;
        public bool useSmooth = true;

        private AnimationState currentAnim;
        private float targetTime = 0.0f;
        private float smooth = 0.3f;


        void Awake()
        {

        }

        void Start()
        {
            currentAnim = anim[anim.clip.name];
            currentAnim.speed = 0.0f;
            anim.Play();
        }

        void Update()
        {
            if(useSmooth)
            {
                // アニメーションをスムース更新
                currentAnim.time += (targetTime - currentAnim.time) * smooth;
            }
            else
                currentAnim.time = targetTime;
        }


        public void Step()
        {
            if(!anim.isPlaying)
                anim.Play();

            if(targetTime < currentAnim.length)
                targetTime += speed;

            if(targetTime > currentAnim.length)
                targetTime = currentAnim.length;
        }

        public void Reset()
        {
            anim.Rewind();
            anim.Play();

            targetTime = 0.0f;
        }

        public void SetSeconds(float sec)
        {
            if(sec > currentAnim.length)
                sec = currentAnim.length;

            targetTime = sec;
        }

        public float GetSeconds()
        {
            return currentAnim.time;
        }

        public float GetDuration()
        {
            return currentAnim.length;
        }
    }
}
