using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class AsyncStateBase : StateBase, IAsyncState
    {
        [Header("AsyncStateBase")]
        public float fadeTime = 1.0f;
        public Color fadeColor = Color.white;

        private Fader fader;


        protected virtual void Awake()
        {
            List<Camera> cameras = CameraUtil.GetCameraListByDepth();
            Camera cam = cameras[cameras.Count - 1];
            fader = cam.GetComponent<Fader>();
            if(fader == null)
                fader = cam.gameObject.AddComponent<Fader>();
        }


        public override void StateStart(object context)
        {
            base.StateStart(context);

            // フェードINを開始
            if(Fader.UseFade && AppMain.Instance.sceneStateManager.AsyncChangeFading)
            {
                if(fader != null)
                {
                    fader.fadeColor = fadeColor;
                    fader.StartFade(fadeTime, Fader.FADE_TYPE.FADE_IN);
                }
                Invoke("InvokeAsyncChangeFaded", fadeTime);
            }
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();

            // フェードOUTを開始
            CancelInvoke("InvokeAsyncChangeFaded");
            if(Fader.UseFade && AppMain.Instance.sceneStateManager.AsyncChangeFading)
            {
                if(fader != null)
                {
                    fader.fadeColor = fadeColor;
                    fader.StartFade(fadeTime, Fader.FADE_TYPE.FADE_OUT);
                }
                Invoke("OnFaded", fadeTime);
            }
            else
                OnFaded();
        }

        public virtual void StateExitAsync()
        {

        }


        private void OnFaded()
        {
            StateExitAsync();

            // フェードOUT完了でState切り替えを実行 同期してStateを切り替え
            AppMain.Instance.sceneStateManager.SyncState();
        }

        private void InvokeAsyncChangeFaded()
        {
            AppMain.Instance.sceneStateManager.AsyncChangeFaded();
        }
    }
}
