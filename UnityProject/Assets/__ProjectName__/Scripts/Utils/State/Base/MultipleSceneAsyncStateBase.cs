using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class MultipleSceneAsyncStateBase : MultipleSceneStateBase, IAsyncState
    {
        [Header("MultipleSceneAsyncStateBase")]
        public float fadeTime = 1.0f;
        public Color fadeColor = Color.white;

        private Fader fader;


        private void Awake()
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
        }

        public override void SceneLoaded()
        {
            base.SceneLoaded();

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
