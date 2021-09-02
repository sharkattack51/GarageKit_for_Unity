using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/*
 * オブジェクトの自動スケールアニメーション
 */
namespace GarageKit
{
    public class AutoScale : MonoBehaviour
    {	
        public bool isLoop = true;
        public float scaleTime = 1.0f;
        public Vector3 startScale = Vector3.zero;
        public Vector3 endScale = Vector3.one;


        void Start()
        {	
            StartTween();
        }

        private void StartTween()
        {
            this.gameObject.transform.localScale = startScale;
            this.gameObject.transform.DOScale(endScale, scaleTime)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(isLoop ? -1 : 1)
                .Play();
        }
    }
}
