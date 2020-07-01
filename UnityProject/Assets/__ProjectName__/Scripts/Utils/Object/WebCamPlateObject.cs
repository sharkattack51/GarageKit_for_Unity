using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * WebCameraの映像をテクスチャに設定する
 * 使用する前にWebカメラのドライバをインストールすること!
 */
namespace GarageKit
{
    [RequireComponent(typeof(Renderer))]
    public class WebCamPlateObject : MonoBehaviour
    {
        // デバイス設定
        public string deviceName = "";
        public int deviceIndex = 0;
        public int requestedWidth = 1280;
        public int requestedHeight = 720;
        public int requestedFPS = 30;
        public int anisoLevel = 9;
        public string texturePropName = "_MainTex";
        public FilterMode filteMode = FilterMode.Bilinear;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public bool isAutoAspect = true;
        public bool isMirror = false;
        public bool isMobileCameraRotation = false;
        public OBJECTAXIS_Y vertical = OBJECTAXIS_Y.Z;

        private WebCamDevice[] devices;
        private WebCamTexture webCamTexture;
        public WebCamTexture GetWebCamTexture() { return webCamTexture; }

        private Renderer rend;
        private Vector3 defaultAspect;

        public enum OBJECTAXIS_Y
        {
            Y = 0,
            Z
        }


        void Awake()
        {

        }

        IEnumerator Start()
        {
            rend = this.gameObject.GetComponent<Renderer>();
            rend.enabled = false;

            // Webカメラを開く
            try
            {
                devices = WebCamTexture.devices;

                if(!string.IsNullOrEmpty(deviceName))
                    webCamTexture = new WebCamTexture(deviceName, requestedWidth, requestedHeight, requestedFPS);
                else
                    webCamTexture = new WebCamTexture(devices[deviceIndex].name, requestedWidth, requestedHeight, requestedFPS);

                webCamTexture.anisoLevel = anisoLevel;
                webCamTexture.filterMode = filteMode;
                webCamTexture.wrapMode = wrapMode;
                webCamTexture.mipMapBias = -1.0f;

                Debug.Log("webcam requested: " + webCamTexture.deviceName + " : " + webCamTexture.requestedWidth.ToString() + " : " + webCamTexture.requestedHeight.ToString());
            }
            catch(Exception e)
            {
                Debug.Log("webcam open error: " + e.Message);
                yield break;
            }

            // アスペクト調整用の初期値を保存
            defaultAspect = this.gameObject.transform.localScale;

            // アスペクトを自動調整
            if(isAutoAspect)
            {
                float textureAspect = (float)requestedWidth / (float)requestedHeight;
                float objectAspect = 1.0f;
                if(vertical == OBJECTAXIS_Y.Z)
                    objectAspect = defaultAspect.x / defaultAspect.z;
                else
                    objectAspect = defaultAspect.x / defaultAspect.y;
                float scaleRatio = textureAspect / objectAspect;
                
                this.gameObject.transform.localScale = new Vector3(defaultAspect.x * scaleRatio, defaultAspect.y, defaultAspect.z);
            }

            // 反転設定
            if(isMirror)
            {
                this.gameObject.transform.localScale = new Vector3(
                    -this.gameObject.transform.localScale.x,
                    this.gameObject.transform.localScale.y,
                    this.gameObject.transform.localScale.z);
            }

            // テクスチャを適用
            rend.material.SetTexture(texturePropName, webCamTexture);

            // 映像の更新を開始
            webCamTexture.Play();

            yield return new WaitUntil(() =>{ return IsWebCamPlaySuccess(); });

            Debug.Log("webcam opened: " + webCamTexture.deviceName + " : " + webCamTexture.width.ToString() + " : " + webCamTexture.height.ToString());
            Debug.Log("webcam video rotation angle: " + webCamTexture.videoRotationAngle.ToString());
            Debug.Log("webcam video virtical mirror: " + webCamTexture.videoVerticallyMirrored.ToString());

            // モバイルデバイスのカメラ回転設定
            if(isMobileCameraRotation && webCamTexture.videoRotationAngle > 0)
                this.gameObject.transform.rotation = Quaternion.Euler(0, 0, -webCamTexture.videoRotationAngle);

            rend.enabled = true;

            yield break;
        }

        void OnDisable()
        {
            if(webCamTexture != null)
            {
                webCamTexture.Stop();
                WebCamTexture.Destroy(webCamTexture);
            }
        }

        // 現在使用しているWebカメラ映像を取得
        public WebCamTexture GetTexture()
        {
            if(webCamTexture != null)
                return webCamTexture;
            else
                return null;
        }

        // WebCamがPlayされた後に正常に開かれたかどうか
        public bool IsWebCamPlaySuccess()
        {
            return webCamTexture != null && webCamTexture.isPlaying
                && webCamTexture.width > 16 && webCamTexture.height > 16;
        }
    }
}
