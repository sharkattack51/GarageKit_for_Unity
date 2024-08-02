using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/*
 * マテリアルの自動点滅アニメーション
 */
namespace GarageKit
{
    public class AutoBlink : MonoBehaviour
    {	
        public bool isPinpon = true;
        public float blinkTime = 1.0f;
        public float startAlpha = 1.0f;
        public float endAlpha = 0.0f;


        void Start()
        {
            StartTween();
        }

        private void StartTween()
        {
            Renderer rend = this.gameObject.GetComponent<Renderer>();
            Color startColor = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, startAlpha);
            Color endColor = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, endAlpha);

            rend.material.color = startColor;
            rend.material.DOColor(endColor, blinkTime)
                .SetEase(Ease.Linear)
                .SetLoops(-1, isPinpon ? LoopType.Yoyo : LoopType.Restart)
                .Play();
        }
    }
}
