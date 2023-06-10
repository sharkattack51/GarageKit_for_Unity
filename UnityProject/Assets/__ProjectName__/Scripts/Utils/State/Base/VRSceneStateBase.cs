//#define USE_VRTK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class VRSceneStateBase : TimelinedSceneStateBase, ISequentialState, IVRSceneState
    {
        [Header("VRSceneStateBase")]
        public GameObject viewReferenceObj;
        public GameObject viewGuideTarget;
        public StageManagedObject[] enables;

        public Action OnAppFinish;
        private static bool isNotifiedForQuit = false;
        private static bool fromOnDestory = false;


        // アプリケーションの終了処理
        public void FinishApplication()
        {
            if(isNotifiedForQuit)
                return;
            
            isNotifiedForQuit = true;

            Debug.Log("application finish process");

            if(!fromOnDestory)
            {
                StartCoroutine(LazyFinishApplicationCoroutine());
            }
            else
            {
                if(OnAppFinish != null)
                    OnAppFinish();
                
                Debug.Log("quit");
            }
        }

        private IEnumerator LazyFinishApplicationCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(2.0f);

                if(OnAppFinish != null)
                    OnAppFinish();

                Application.Quit();
                Debug.Log("quit");
                break;
            }
        }

        void OnDestroy()
        {
            fromOnDestory = true;

            // 通常動作ではここからは呼ばれない
            if(!isNotifiedForQuit)
                FinishApplication();
        }


        public override void StateStart(object context)
        {
            base.StateStart(context);

            // 表示オブジェクト切替
            SetStagingObjects();

            // 音声フェードIn
            if(AppMain.Instance.soundManager != null)
                AppMain.Instance.soundManager.FadeInAllSound(this.fadeTime);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();

            // 音声フェードOut
            if(AppMain.Instance.soundManager != null)
                AppMain.Instance.soundManager.FadeOutAllSound(this.fadeTime);
        }
        

        public virtual void ToNextState()
        {

        }

        public virtual void ToPrevState()
        {

        }

        public virtual void ResetCurrentState()
        {
            
        }


        // 視点リセット
        public void ResetPosition(Transform trans = null)
        {
#if USE_VRTK
            if(trans == null)
            {
                VRTKUtil_Alias.Instance.VrRig.position = Vector3.zero;
                VRTKUtil_Alias.Instance.VrRig.rotation = Quaternion.identity;
                VRTKUtil_Alias.Instance.VrRig.localScale = Vector3.one;
            }
            else
            {
                VRTKUtil_Alias.Instance.VrRig.position = trans.position;
                VRTKUtil_Alias.Instance.VrRig.rotation = trans.rotation;
                VRTKUtil_Alias.Instance.VrRig.localScale = trans.localScale;
            }
#else
            // do something
#endif
        }

        // 表示オブジェクト切替
        public void SetStagingObjects()
        {
            StageManagedObject.AllOff();
            foreach(StageManagedObject ctrObj in enables)
            {
                if(ctrObj != null)
                    ctrObj.On();
            }
        }

        // 視線方向のEditor表示
        protected virtual void OnDrawGizmos()
        {
            if(viewReferenceObj != null && viewGuideTarget != null)
            {
                Color tempColor = Gizmos.color;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(viewReferenceObj.transform.position, viewGuideTarget.transform.position);
                Gizmos.color = tempColor;
            }
        }
    }
}
