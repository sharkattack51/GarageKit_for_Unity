using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ターゲットオブジェクトのスクリーン位置に一致させる
 */
namespace GarageKit
{
    public class ScreenPositionFollower : MonoBehaviour
    {
        [Header("reference target")]
        public GameObject targetObject;
        public Camera rayCamera;

        [Header("screen pos calculation setting")]
        public int screenWidth = 1920;
        public int screenHeight = 1080;
        public bool setAsScreenSize = true;
        public float uiScale = 2.0f;

        private Vector3 defaultScale;


        void Awake()
        {

        }

        void Start()
        {
            defaultScale = this.gameObject.transform.localScale;

            if(rayCamera == null)
                rayCamera = Camera.main;

            if(setAsScreenSize)
            {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
            }

            SetFollowScreenPosition();
        }

        void Update()
        {
            SetFollowScreenPosition();
        }


        // スクリーン位置を設定
        private void SetFollowScreenPosition()
        {
            if(targetObject != null)
            {
                Vector3 viewportPoint = rayCamera.WorldToViewportPoint(targetObject.transform.position);
                Vector3 screenPosition = new Vector3(
                    screenWidth / 2.0f * (viewportPoint.x - 0.5f) * uiScale,
                    screenHeight / 2.0f * (viewportPoint.y - 0.5f) * uiScale,
                    0.0f);
                this.gameObject.transform.localPosition = screenPosition;

                // カメラより後ろのものはスケールで隠す
                if(viewportPoint.z <= 0.0f)
                    this.gameObject.transform.localScale = Vector3.zero;
                else
                    this.gameObject.transform.localScale = defaultScale;
            }
        }
    }
}
