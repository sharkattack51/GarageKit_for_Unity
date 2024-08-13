using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    /// <summary>
    /// カメラによる座標変換ユーティリティ
    /// </summary>
    public class CameraUtil
    {
        /// <summary>
        /// レイヤーからカメラを取得する
        /// </summary>
        public static Camera FindCameraForLayer(int layer)
        {
            int layerMask = (1 << layer);
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

            for(int i = 0; i < cameras.Length; i++)
            {
                if((cameras[i].cullingMask & layerMask) != 0)
                    return cameras[i];
            }

            return null;
        }

        /// <summary>
        /// デプスによるカメラリストを取得
        /// </summary>
        public static List<Camera> GetCameraListByDepth()
        {
            List<Camera> cameras = new List<Camera>(Camera.FindObjectsOfType<Camera>());
            cameras.Sort((a, b) => { return a.depth < b.depth ? -1 : 1; });

            return cameras;
        }

        /// <summary>
        /// スクリーン座標をカメラ座標で正規化(-1.0～1.0)
        /// </summary>
        public static Vector3 NormalizeScreenPosition(Camera orthoCamera, Vector3 screenPosition)
        {
            float size = orthoCamera.orthographicSize;

            float v = size * 2;
            float h = v * ((float)Screen.width / (float)Screen.height);

            float x = ((screenPosition.x / Screen.width) - 0.5f) * h ;
            float y = ((screenPosition.y / Screen.height) - 0.5f) * v;

            Vector3 norm = new Vector3(x, y, 0.0f);

            return norm;
        }

        /// <summary>
        /// Viewportでの画面はみ出しを修正する
        /// </summary>
        public static Vector3 ViewportProtrusion(Vector3 viewportPosition, Vector2 ratio)
        {
            float protX;
            if(viewportPosition.x < (1.0f - ratio.x) / 2.0f)
                protX = (1.0f - ratio.x) / 2.0f;
            else if(viewportPosition.x > 1.0f - ((1.0f - ratio.x) / 2.0f))
                protX = 1.0f - ((1.0f - ratio.x) / 2.0f);
            else
                protX = viewportPosition.x;

            float protY;
            if(viewportPosition.y < (1.0f - ratio.y) / 2.0f)
                protY = (1.0f - ratio.y) / 2.0f;
            else if(viewportPosition.y > 1.0f - ((1.0f - ratio.y) / 2.0f))
                protY = 1.0f - ((1.0f - ratio.y) / 2.0f);
            else
                protY = viewportPosition.y;

            return new Vector3(protX, protY, viewportPosition.z);
        }
    }
}
