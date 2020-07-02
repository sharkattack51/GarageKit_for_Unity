//#define USE_ARFOUNDATION

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace GarageKit.ARFoundationExtention
{
    [Serializable]
    public class ARImageAnchorOriginData
    {
        public string imageName = "";
        public GameObject imageAnchorPrefab = null;
    }

    [DisallowMultipleComponent]
#if USE_ARFOUNDATION
    [RequireComponent(typeof(ARTrackedImageManager))]
    [RequireComponent(typeof(ARAnchorManager))]
#endif
    public class ARTrackedImageAnchorManager : MonoBehaviour
    {
#if USE_ARFOUNDATION
        // Origins
        public List<ARImageAnchorOriginData> imageAnchorOrigins;

        // アンカーの画面外判定でDestroyを行う
        public bool destroyOnInvisibleAnchor = false;

        private ARTrackedImageManager trackedImageManager;
        private ARAnchorManager anchorManager;
        private Camera arCamera;

        // トラッキング中のアンカー
        private Dictionary<string, ARAnchor> trackedImageAnchors;
        public Dictionary<string, ARAnchor> TrackedImageAnchors { get{ return trackedImageAnchors; } }

        // 画像マーカーの認識時イベント
        public Action<string> OnFoundImageAnchor;

        // 画像マーカーの削除時イベント
        public Action<string> OnRemoveImageAnchor;


        void Awake()
        {
            trackedImageManager = this.gameObject.GetComponent<ARTrackedImageManager>();
            anchorManager = this.gameObject.GetComponent<ARAnchorManager>();
            arCamera = FindObjectOfType<ARCameraManager>().GetComponent<Camera>();
        }

        void Start()
        {
            trackedImageAnchors = new Dictionary<string, ARAnchor>();
        }

        void OnEnable()
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable()
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        void Update()
        {
            // アンカーの画面外削除
            if(destroyOnInvisibleAnchor)
            {
                foreach(KeyValuePair<string, ARAnchor> kvp in trackedImageAnchors)
                {
                    string imageName = kvp.Key;
                    ARAnchor anchor = kvp.Value;

                    if(IsOutOfView(anchor.gameObject.transform.position))
                        RemoveImageAnchorFromImageName(imageName);
                }
            }
        }


        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs e)
        {
            // 新規マーカーの認識
            foreach(ARTrackedImage trackedImage in e.added)
            {
                if(trackedImage.trackingState != TrackingState.None)
                    AddImageAnchor(trackedImage);
            }

            // マーカー認識の継続更新
            foreach(ARTrackedImage trackedImage in e.updated)
            {
                if(trackedImage.trackingState != TrackingState.None)
                {
                    // 検出後 削除済みの場合に再度追加
                    if(!trackedImageAnchors.ContainsKey(trackedImage.referenceImage.name))
                        AddImageAnchor(trackedImage);
                }
            }

            // マーカー認識のロスト
            foreach(ARTrackedImage trackedImage in e.removed)
                RemoveImageAnchorFromImageName(trackedImage.referenceImage.name);
        }

        private void AddImageAnchor(ARTrackedImage trackedImage)
        {
            if(trackedImageAnchors.ContainsKey(trackedImage.referenceImage.name))
            {
                // 同一マーカーの複数認識を拒否
                Debug.LogWarning("Multiple identical markers have been recognized.");
                return;
            }

            // 画面外判定
            if(IsOutOfView(trackedImage.transform.position))
                return;

            // マーカー画像検出位置にアンカーを設定
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(trackedImage.transform.position, trackedImage.transform.rotation));
            anchor.destroyOnRemoval = true;
            float scale = trackedImage.size.x / trackedImage.referenceImage.size.x;
            anchor.gameObject.transform.localScale = Vector3.one * scale;

            // アンカーの子供にオブジェクトを生成
            ARImageAnchorOriginData org = imageAnchorOrigins.Find(o => o.imageName == trackedImage.referenceImage.name);
            if(org != null && org.imageAnchorPrefab != null)
            {
                GameObject go = Instantiate(org.imageAnchorPrefab) as GameObject;
                go.name += string.Format(" [{0}]", org.imageName);  
                go.transform.SetParent(anchor.gameObject.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }

            trackedImageAnchors.Add(trackedImage.referenceImage.name, anchor);

            // マーカー検出イベント
            this.OnFoundImageAnchor?.Invoke(trackedImage.referenceImage.name);
        }

        public void RemoveImageAnchorFromImageName(string imageName)
        {
            if(trackedImageAnchors.ContainsKey(imageName))
            {
                // マーカー削除イベント
                this.OnRemoveImageAnchor?.Invoke(imageName);

                anchorManager.RemoveAnchor(trackedImageAnchors[imageName]);
                trackedImageAnchors.Remove(imageName);
            }
        }

        public void ResetImageAnchors()
        {
            foreach(ARImageAnchorOriginData org in imageAnchorOrigins)
                RemoveImageAnchorFromImageName(org.imageName);
        }

        private bool IsOutOfView(Vector3 worldPosition)
        {
            Vector3 vp = arCamera.WorldToViewportPoint(worldPosition);
            return (vp.x < 0.0f || vp.x > 1.0f || vp.y < 0.0f || vp.y > 1.0f);
        }
#endif
    }
}
