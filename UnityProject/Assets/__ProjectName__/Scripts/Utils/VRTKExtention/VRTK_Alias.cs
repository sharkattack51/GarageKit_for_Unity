//#define USE_VRTK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_VRTK
using VRTK;
#endif

namespace GarageKit.VRTKExtention
{
    public class VRTK_Alias : MonoBehaviour
    {
#if USE_VRTK
        private static VRTK_Alias instance;
        public static VRTK_Alias Instance { get{ return instance; } }

        private bool isReady = false;
        public bool IsReady { get{ return isReady; } }

        public enum DEVICE_TYPE // VRTK SDKリスト順
        {
            OCULUS = 0,
            VIVE
        }
        private DEVICE_TYPE deviceType = DEVICE_TYPE.OCULUS;
        public DEVICE_TYPE DeviceType { get{ return deviceType; } }

        private string deviceFamily = "";
        public string DeviceFamily { get{ return deviceFamily; } }

        private Transform vrRig;
        public Transform VrRig { get{ return vrRig; } }

        private Transform headset;
        public Transform Headset { get{ return headset; } }

#region  RightController
        private VRTK_ControllerReference rightControllerReference;
        public VRTK_ControllerReference RightControllerReference { get{ return rightControllerReference; } }

        private VRTK_ControllerEvents rightControllerEvents;
        public VRTK_ControllerEvents RightControllerEvents { get{ return rightControllerEvents; } }

        private GameObject rightControllerRoot;
        public GameObject RightControllerRoot { get{ return rightControllerRoot; } }

        private GameObject rightControllerModel;
        public GameObject RightControllerModel { get{ return rightControllerModel; } }

        private Renderer[] rightControllerModelRenders;
        public Renderer[] RightControllerModelRenders
        {
            get
            {
                if(rightControllerModelRenders == null || rightControllerModelRenders.Length == 0)
                {
                    if(rightControllerModel != null)
                        rightControllerModelRenders = rightControllerModel.GetComponentsInChildren<Renderer>();
                }
                return rightControllerModelRenders;
            }
        }

        public bool RightControllerIsOn { get{ return (rightControllerRoot != null) ? rightControllerRoot.activeSelf : false; } }
#endregion

#region  LeftController
        private VRTK_ControllerReference leftControllerReference;
        public VRTK_ControllerReference LeftControllerReference { get{ return leftControllerReference; } }

        private VRTK_ControllerEvents leftControllerEvents;
        public VRTK_ControllerEvents LeftControllerEvents { get{ return leftControllerEvents; } }

        private GameObject leftControllerRoot;
        public GameObject LeftControllerRoot { get{ return leftControllerRoot; } }

        private GameObject leftControllerModel;
        public GameObject LeftControllerModel { get{ return leftControllerModel; } }

        private Renderer[] leftControllerModelRenders;
        public Renderer[] LeftControllerModelRenders
        {
            get
            {
                if(leftControllerModelRenders == null || leftControllerModelRenders.Length == 0)
                {
                    if(leftControllerModel != null)
                        leftControllerModelRenders = leftControllerModel.GetComponentsInChildren<Renderer>();
                }
                return leftControllerModelRenders;
            }
        }

        public bool LeftControllerIsOn { get{ return (leftControllerRoot != null) ? leftControllerRoot.activeSelf : false; } }
#endregion


        void Awake()
        {
            instance = this;
        }

        IEnumerator Start()
        {
            yield return null;

            // VRTKのSDKオートロードはせずに手動セットアップ
            string vrSystem = ApplicationSetting.Instance.GetString("VRSystem");
            int sdkIndex = (int)DEVICE_TYPE.OCULUS;
            if(vrSystem.ToLower() == "oculus")
            {
                Debug.Log("select VR System [ Oculus ]");
                sdkIndex = (int)DEVICE_TYPE.OCULUS;

                deviceType = DEVICE_TYPE.OCULUS;
            }
            else if(vrSystem.ToLower() == "vive")
            {
                Debug.Log("select VR System [ VIVE ]");
                sdkIndex = (int)DEVICE_TYPE.VIVE;

                deviceType = DEVICE_TYPE.VIVE;
            }

            try
            {
                VRTK_SDKManager.instance.TryLoadSDKSetup(sdkIndex, true,  VRTK_SDKManager.instance.setups);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                yield break;
            }

            // デバイス初期化に時間がかかるため、各デバイスのエイリアスが取得できるまで確認ループ
            while(!AllDeviceIsReady())
            {
                yield return null; // 初期化完了待機 1フレ後に以下取得を確認

                // リグRoot
                vrRig = VRTK_DeviceFinder.PlayAreaTransform();

                // ヘッドセット
                headset = VRTK_DeviceFinder.HeadsetCamera();

                // 初期化時 左コントローラー → 右コントローラー の認識順
                leftControllerReference = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
                GameObject leftHand = VRTK_DeviceFinder.GetControllerLeftHand(false);
                if(leftHand != null)
                    leftControllerEvents = leftHand.GetComponent<VRTK_ControllerEvents>();
                if(leftControllerEvents != null)
                {
                    leftControllerRoot = leftControllerEvents.transform.parent.gameObject;

                    if(deviceType == DEVICE_TYPE.OCULUS)
                        leftControllerModel = leftControllerRoot.transform.Find("LeftControllerAnchor").gameObject;
                    else if(deviceType == DEVICE_TYPE.VIVE)
                        leftControllerModel = leftControllerRoot.transform.Find("Model").gameObject;
                }

                rightControllerReference = VRTK_DeviceFinder.GetControllerReferenceRightHand();
                GameObject rightHand = VRTK_DeviceFinder.GetControllerRightHand(false);
                if(rightHand != null)
                    rightControllerEvents = rightHand.GetComponent<VRTK_ControllerEvents>();
                if(rightControllerEvents != null)
                {
                    rightControllerRoot = rightControllerEvents.transform.parent.gameObject;

                    if(deviceType == DEVICE_TYPE.OCULUS)
                        rightControllerModel = rightControllerRoot.transform.Find("RightControllerAnchor").gameObject;
                    else if(deviceType == DEVICE_TYPE.VIVE)
                        rightControllerModel = rightControllerRoot.transform.Find("Model").gameObject;
                }
            }

            isReady = true;

            yield break;
        }

        void Update()
        {

        }


        // コントローラー片方が見つからない場合でも初期化完了後は通過
        public bool AllDeviceIsReady()
        {
            return (
                vrRig != null
                && headset != null
                && rightControllerReference != null
                && rightControllerEvents != null
                && rightControllerRoot != null
                && rightControllerModel != null
                && leftControllerReference != null
                && leftControllerEvents != null
                && leftControllerRoot != null
                && leftControllerModel != null
            );
        }

        // バイブレーション
        public void HapticPulse(int side)
        {
            VRTK_ControllerReference cr;
            if(side == 0)
                cr = rightControllerReference;
            else
                cr = leftControllerReference;
            
            VRTK_ControllerHaptics.TriggerHapticPulse(cr, 1.0f);
        }
#endif
    }
}
