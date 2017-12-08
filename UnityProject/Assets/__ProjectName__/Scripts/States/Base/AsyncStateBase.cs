using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GarageKit
{
	public class AsyncStateBase : MonoBehaviour, IState
	{
		private float fadeTime = 1.0f;
		
		
		public virtual void StateStart(object context)
		{
			// フェードINを開始
			if(Fader.UseFade)
				Fader.StartFadeAll(fadeTime, Fader.FADE_TYPE.FADE_IN);
		}

		public virtual void StateUpdate()
		{
			
		}

		public virtual void StateExit()
		{
			// フェードOUTを開始
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
			// フェードOUT完了でState切り替えを実行 同期してStateを切り替え
			AppMain.Instance.sceneStateManager.SyncState();
		}
	}
}
