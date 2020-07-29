using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class MultipleSceneAsyncStateBase : MultipleSceneStateBase, IAsyncState
    {
        [Header("MultipleSceneAsyncStateBase")]
        public float fadeTime = 1.0f;


        public override void StateStart(object context)
        {
            base.StateStart(context);
        }

        public override void SceneLoaded()
        {
            base.SceneLoaded();

            if(Fader.UseFade)
                Fader.StartFadeAll(fadeTime, Fader.FADE_TYPE.FADE_IN);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();

            if(Fader.UseFade)
            {
                Fader.StartFadeAll(fadeTime, Fader.FADE_TYPE.FADE_OUT);
                Invoke("OnFaded", fadeTime);
            }
            else
                OnFaded();
        }

        private void OnFaded()
        {
            StateExitAsync();

            // フェードOUT完了でState切り替えを実行 同期してStateを切り替え
            AppMain.Instance.sceneStateManager.SyncState();
        }

        public virtual void StateExitAsync()
        {

        }
    }
}
