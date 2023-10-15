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
        public StageManagedObject [] enables;


        public override void StateStart(object context)
        {
            base.StateStart(context);

            // 表示オブジェクト切替
            SetStagingObjects();

            // 音声フェードIn
            if(AppMain.Instance.soundManager != null)
                AppMain.Instance.soundManager.FadeInAllSound(this.fadeTime);

            this.StartTimeline();
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


        // 表示オブジェクト切替
        public void SetStagingObjects()
        {
            StageManagedObject.AllOff();
            foreach(StageManagedObject obj in enables)
            {
                if(obj != null)
                    obj.On();
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
