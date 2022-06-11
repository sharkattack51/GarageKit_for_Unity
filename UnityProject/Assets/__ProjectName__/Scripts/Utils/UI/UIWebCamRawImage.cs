using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit
{
    [RequireComponent(typeof(RawImage))]
    public class UIWebCamRawImage : MonoBehaviour
    {
        [Header("camera device setting")]
        public bool webcamPlayOnStart = true;
        public string deviceName = "";
        public int deviceIndex = 0;
        public int requestedWidth = 1920;
        public int requestedHeight = 1080;
        public int requestedFps = 30;
        public int anisoLevel = 9;
        public FilterMode filteMode = FilterMode.Bilinear;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public bool isAutoScreenAspect = true;
        public bool isMirror = false;

        public enum APP_ORIENTATION
        {
            PORTRAIT = 0,
            LANDSCAPE
        }
        public APP_ORIENTATION appOrientation = APP_ORIENTATION.PORTRAIT;

        private WebCamDevice[] devices;
        private WebCamTexture webCamTexture;
        public WebCamTexture GetWebCamTexture() { return webCamTexture; }

        private RectTransform rectTrans;
        private RawImage uiRawImage;

        private bool disabled = false;


        void Awake()
        {
        
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if(webcamPlayOnStart)
                yield return WebcamPlay();
        }

        void OnEnable()
        {
            if(disabled)
            {
                disabled = false;

                StartCoroutine(WebcamPlay());
            }
        }

        void OnDisable()
        {
            disabled = true;

            WebcamStop();
        }


        public IEnumerator WebcamPlay()
        {
            rectTrans = this.gameObject.GetComponent<RectTransform>();
            uiRawImage = this.gameObject.GetComponent<RawImage>();
            uiRawImage.enabled = false;

            if(Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.OSXEditor
                || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    appOrientation = APP_ORIENTATION.LANDSCAPE;
                }

#if UNITY_ANDROID || UNITY_IPHONE
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            
            if(Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
#endif
                yield return new WaitForEndOfFrame();

                for(int i = 0; i < WebCamTexture.devices.Length; i++)
                    Debug.LogFormat("UIWebCamRawImage :: devices[{0}] name:{1}", i, WebCamTexture.devices[i].name);

                // Webカメラを開く
                try
                {
                    devices = WebCamTexture.devices;

                    if(!string.IsNullOrEmpty(deviceName))
                        webCamTexture = new WebCamTexture(deviceName, requestedWidth, requestedHeight, requestedFps);
                    else
                        webCamTexture = new WebCamTexture(devices[deviceIndex].name, requestedWidth, requestedHeight, requestedFps);

                    webCamTexture.anisoLevel = anisoLevel;
                    webCamTexture.filterMode = filteMode;
                    webCamTexture.wrapMode = wrapMode;
                    webCamTexture.mipMapBias = -1.0f;

                    Debug.LogFormat("usb camera open requested: {0}/{1}/{2}", webCamTexture.deviceName, webCamTexture.requestedWidth, webCamTexture.requestedHeight);
                }
                catch(Exception e)
                {
                    Debug.Log("usb camera open error: " + e.Message);
                    yield break;
                }

                // 映像の更新を開始
                webCamTexture.Play();

                yield return new WaitUntil(() => IsWebcamPlay());

                Debug.LogFormat("usb camera opened: {0}/{1}", webCamTexture.width, webCamTexture.height);

                // テクスチャを適用
                uiRawImage.texture = webCamTexture;

                // アスペクトを自動調整
                if(isAutoScreenAspect)
                {
                    float webcamAspect = (float)webCamTexture.width / (float)webCamTexture.height;
                    float screenAspect =  (float)Screen.width / (float)Screen.height;
                    rectTrans.localScale = new Vector3(webcamAspect / screenAspect, 1.0f);

                    if(appOrientation == APP_ORIENTATION.PORTRAIT)
                        rectTrans.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
                }

                // 反転設定
#if UNITY_IOS
                rectTrans.localScale = new Vector3(rectTrans.localScale.x, -rectTrans.localScale.y);
#else
                rectTrans.localScale = new Vector3(rectTrans.localScale.x, rectTrans.localScale.y);
#endif
                if(isMirror)
                {
                    if(appOrientation == APP_ORIENTATION.LANDSCAPE)
                        rectTrans.localScale = new Vector3(-rectTrans.localScale.x, rectTrans.localScale.y);
                    else if(appOrientation == APP_ORIENTATION.PORTRAIT)
                        rectTrans.localScale = new Vector3(rectTrans.localScale.x, -rectTrans.localScale.y);
                }

                uiRawImage.enabled = true;

#if UNITY_ANDROID || UNITY_IPHONE
            }
#endif
        }

        public void WebcamStop()
        {
            if(webCamTexture != null)
            {
                webCamTexture.Stop();
                WebCamTexture.Destroy(webCamTexture);
                webCamTexture = null;
            }
        }

        public bool IsWebcamPlay()
        {
            return webCamTexture != null
                && webCamTexture.isPlaying
                && webCamTexture.width > 16
                && webCamTexture.height > 16;
        }

        // 現在使用しているWebカメラ映像を取得
        public WebCamTexture GetTexture()
        {
            if(webCamTexture != null)
                return webCamTexture;
            else
                return null;
        }
    }
}
