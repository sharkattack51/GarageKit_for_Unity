using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/*
 * UVの自動スクロールアニメーション
 */
namespace GarageKit
{
    public class AutoUVScroll : MonoBehaviour
    {
        public int materialID = 0;
        public string texturePropName = "_MainTex";

        public float scrollTime = 1.0f;
        public Vector2 startUV = Vector2.zero;
        public Vector2 endUV = Vector2.zero;
        public Ease easeType = Ease.Linear;
        public LoopType loopType = LoopType.Restart;

        private Material targetMat;


        void Start()
        {
            targetMat = this.GetComponent<Renderer>().materials[materialID];

            DOVirtual.Float(0.0f, 1.0f, scrollTime,
                (v) => {
                    targetMat.SetTextureOffset(texturePropName, Vector2.Lerp(startUV, endUV, v));
                })
                .SetEase(easeType)
                .SetLoops(-1, loopType)
                .Play();
        }
    }
}
