using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class AsyncStateBase : StateBase, IAsyncState
    {
        [Header("AsyncStateBase")]
        public float fadeTime = 1.0f;


        private void Awake()
        {

        }

        
        public override void StateStart(object context)
        {
            base.StateStart(context);

            // フェードINを開始
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

            // フェードOUTを開始
            if(Fader.UseFade)
            {
                Fader.StartFadeAll(fadeTime, Fader.FADE_TYPE.FADE_OUT);
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
    }
}
