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
        /// 階層の表示設定をまとめて変更する
        /// </summary>
        public static void SetRenderEnabledChildren(GameObject root, bool isRender)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

            foreach(Renderer rend in renderers)
                rend.enabled = isRender;
        }

        /// <summary>
        /// 階層全体のBounds(Local)を取得する
        /// </summary>
        public static Bounds CalcLocalObjBounds(GameObject obj)
        {
            Bounds totalBounds = CalcChildObjWorldBounds(obj, new Bounds());

            Vector3 objWorldPosition = obj.transform.position;
            Vector3 objWorldScale = obj.transform.lossyScale;
            Vector3 totalBoundsLocalCenter = new Vector3(
                (totalBounds.center.x - objWorldPosition.x) / objWorldScale.x,
                (totalBounds.center.y - objWorldPosition.y) / objWorldScale.y,
                (totalBounds.center.z - objWorldPosition.z) / objWorldScale.z);
            Vector3 meshBoundsLocalSize = new Vector3(
                totalBounds.size.x / objWorldScale.x,
                totalBounds.size.y / objWorldScale.y,
                totalBounds.size.z / objWorldScale.z);

            Bounds localBounds = new Bounds(totalBoundsLocalCenter, meshBoundsLocalSize);

            return localBounds;
        }

        /// <summary>
        /// 階層全体のBounds(World)を取得する
        /// </summary>
        public static Bounds CalcChildObjWorldBounds(GameObject obj, Bounds bounds)
        {
            for(int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                if(!child.gameObject.activeSelf)
                    continue;

                MeshFilter filter = child.gameObject.GetComponent<MeshFilter>();
                MeshRenderer renderer = child.gameObject.GetComponent<MeshRenderer>();

                if(filter != null && renderer != null)
                {
                    filter.mesh.RecalculateBounds();

                    Bounds meshBounds = renderer.bounds;
                    Vector3 meshBoundsWorldCenter = meshBounds.center;
                    Vector3 meshBoundsWorldSize = meshBounds.size;
                    Vector3 meshBoundsWorldMin = meshBoundsWorldCenter - (meshBoundsWorldSize / 2.0f);
                    Vector3 meshBoundsWorldMax = meshBoundsWorldCenter + (meshBoundsWorldSize / 2.0f);

                    // 取得した最小座標と最大座標を含むように拡大/縮小を行う
                    if(bounds.size == Vector3.zero)
                        bounds = new Bounds(meshBoundsWorldCenter, Vector3.zero);
                    bounds.Encapsulate(meshBoundsWorldMin);
                    bounds.Encapsulate(meshBoundsWorldMax);
                }

                // 再帰
                bounds = CalcChildObjWorldBounds(child.gameObject, bounds);
            }

            return bounds;
        }
    }
}
