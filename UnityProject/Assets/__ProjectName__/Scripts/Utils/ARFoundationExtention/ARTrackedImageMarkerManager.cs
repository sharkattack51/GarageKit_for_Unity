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
    public class ARTrackedImageMarkerData
    {
        public string imageName = "";
        public GameObject imageMarkerPrefab = null;
    }

#if USE_ARFOUNDATION
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ARTrackedImageManager))]
#endif
    public class ARTrackedImageMarkerManager : MonoBehaviour
    {
#if USE_ARFOUNDATION
        // 生成するイメージマーカーPrefab
        public List<ARTrackedImageMarkerData> trackedImageMarkerDatas;

        // 画面外判定でDestroyを行う
        public bool destroyOnInvisible = false;

        // トラッキングされるイメージ数の制限
        public int numberOfTrackingImage = 1;

        private ARTrackedImageManager trackedImageManager;
        private Camera arCamera;
        private ARSessionOrigin sessionOrigin;
        private GameObject worldOrigin;

        // トラッキング中のイメージ
        private Dictionary<string, ARTrackedImage> trackedImages;
        public Dictionary<string, ARTrackedImage> TrackedImages { get{ return trackedImages; } }

        // イメージマーカーの追加時イベント
        public Action<ARTrackedImage> OnAddedImageMarker;

        // イメージマーカーの削除時イベント
        public Action<ARTrackedImage> OnRemoveImageMarker;


        void Awake()
        {
            trackedImageManager = this.gameObject.GetComponent<ARTrackedImageManager>();
            trackedImageManager.trackedImagePrefab = null;

            arCamera = FindObjectOfType<ARCameraManager>().GetComponent<Camera>();
            sessionOrigin = FindObjectOfType<ARSessionOrigin>();

            worldOrigin = new GameObject("AR Foundation World Origin");
            worldOrigin.transform.position = Vector3.zero;
            worldOrigin.transform.rotation = Quaternion.identity;
            worldOrigin.transform.localScale = Vector3.one;
        }

        void Start()
        {
            trackedImages = new Dictionary<string, ARTrackedImage>();
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
            // 画面外判定で削除
            if(destroyOnInvisible)
            {
                List<string> removeReserved = new List<string>();

                foreach(KeyValuePair<string, ARTrackedImage> kvp in trackedImages)
                {
                    string imageName = kvp.Key;
                    ARTrackedImage trackedImage = kvp.Value;

                    if(IsOutOfView(trackedImage.transform.position))
                        removeReserved.Add(imageName);
                }

                foreach(string name in removeReserved)
                    RemoveImageMarkerByImageName(name);
            }
        }


        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs e)
        {
            // 新規イメージの認識
            foreach(ARTrackedImage trackedImage in e.added)
            {
                if(trackedImage.trackingState != TrackingState.None)
                    AddImageMarker(trackedImage);
            }

            // イメージ認識の継続更新
            foreach(ARTrackedImage trackedImage in e.updated)
            {
                if(trackedImage.trackingState != TrackingState.None)
                {
                    // 検出後 削除済みの場合に再度追加
                    if(!trackedImages.ContainsKey(trackedImage.referenceImage.name))
                        AddImageMarker(trackedImage);

                    UpdateImageMarker(trackedImage);
                }
            }

            // イメージ認識のロスト
            foreach(ARTrackedImage trackedImage in e.removed)
                RemoveImageMarkerByImageName(trackedImage.referenceImage.name);
        }

        private void AddImageMarker(ARTrackedImage trackedImage)
        {
            trackedImage.destroyOnRemoval = true;

            if(trackedImages.Count >= numberOfTrackingImage)
            {
                // トラッキングされるイメージ数の制限
                Debug.LogWarning("ARTrackedImageMarkerManager :: Limit of number recognized.");
                return;
            }

            if(trackedImages.ContainsKey(trackedImage.referenceImage.name))
            {
                // 同一イメージの複数認識を拒否
                Debug.LogWarning("ARTrackedImageMarkerManager :: Multiple identical markers have been recognized.");
                return;
            }

            // 画面外判定
            if(IsOutOfView(trackedImage.transform.position))
                return;

            Debug.LogFormat("ARTrackedImageMarkerManager :: ImageMarker added [{0}].", trackedImage.referenceImage.name);

            // イメージマーカー生成
            ARTrackedImageMarkerData data = trackedImageMarkerDatas.Find(d => d.imageName == trackedImage.referenceImage.name);
            if(data != null && data.imageMarkerPrefab != null)
            {
                GameObject go = Instantiate(data.imageMarkerPrefab) as GameObject;
                if(!go.name.Contains(data.imageName))
                    go.name += string.Format(" [{0}]", data.imageName);  
                go.transform.SetParent(trackedImage.transform); // set to child of ARTrackedImage obj
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }

            trackedImages.Add(trackedImage.referenceImage.name, trackedImage);

            // マーカー追加イベント
            this.OnAddedImageMarker?.Invoke(trackedImage);
        }

        private void UpdateImageMarker(ARTrackedImage trackedImage)
        {
            sessionOrigin.MakeContentAppearAt(
                worldOrigin.transform,
                trackedImage.transform.position,
                trackedImage.transform.localRotation);
        }

        public void RemoveImageMarkerByImageName(string imageName)
        {
            if(trackedImages.ContainsKey(imageName))
            {
                Debug.LogFormat("ARTrackedImageMarkerManager :: ImageMarker remove [{0}].", imageName);

                // マーカー削除イベント
                this.OnRemoveImageMarker?.Invoke(trackedImages[imageName]);

                if(trackedImages[imageName].transform.childCount > 0)
                {
                    // get from child of ARTrackedImage obj
                    GameObject go = trackedImages[imageName].transform.GetChild(0).gameObject;
                    GameObject.Destroy(go);
                }
                trackedImages.Remove(imageName);
            }
        }

        public void ResetImageMarkers()
        {
            foreach(ARTrackedImageMarkerData data in trackedImageMarkerDatas)
                RemoveImageMarkerByImageName(data.imageName);
        }

        private bool IsOutOfView(Vector3 worldPosition)
        {
            Vector3 vp = arCamera.WorldToViewportPoint(worldPosition);
            return (vp.x < 0.0f || vp.x > 1.0f || vp.y < 0.0f || vp.y > 1.0f);
        }
#endif
    }
}
