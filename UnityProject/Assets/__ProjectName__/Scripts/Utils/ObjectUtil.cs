using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    /// <summary>
    /// オブジェクト操作関連のユーティリティ
    /// </summary>
    public class ObjectUtil
    {	
        /// <summary>
        /// 子供のレイヤーを全て変更する
        /// </summary>
        public static void SetLayerChildren(GameObject rootObject, int layer, bool changeParent = false)
        {
            if(changeParent)
                rootObject.layer = layer;

            for(int i = 0; i < rootObject.transform.childCount; i++)
            {
                GameObject child = rootObject.transform.GetChild(i).gameObject;
                child.layer = rootObject.layer;

                SetLayerChildren(child, child.layer);

                child = null;
            }
        }

        /// <summary>
        /// 階層のRenderBoundsを取得する
        /// </summary>
        public static Bounds GetRenderBoundsChildren(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

            // rendererが無い場合は無効
            if(renderers.Length == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Vector3 vertexPosMax = renderers[0].bounds.max;
            Vector3 vertexPosMin = renderers[0].bounds.min;

            foreach(Renderer rndr in renderers)
            {
                if(rndr.enabled)
                {
                    // maxを比較
                    vertexPosMax = Vector3.Max(vertexPosMax, rndr.bounds.max);
                    
                    // minを比較
                    vertexPosMin = Vector3.Min(vertexPosMin, rndr.bounds.min);
                }
            }

            Vector3 center = (vertexPosMax + vertexPosMin) * 0.5f;
            Vector3 size = vertexPosMax - vertexPosMin;

            return new Bounds(center, size);
        }

        /// <summary>
        /// 階層の表示設定をまとめて変更する
        /// </summary>
        public static void SetRenderEnabledChildren(GameObject root, bool isRender)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

            foreach(Renderer rndr in renderers)
                rndr.enabled = isRender;
        }
    }
}
