using UnityEngine;
using System.Collections;

public class AsyncStateBase : MonoBehaviour, IState
{
	public FaderObject fader;
	
	
	public virtual void StateStart(object context)
	{
		// フェードINを開始
		fader.FadeStart(FaderObject.FADE_TYPE.FADE_IN, 1.0f, 0.0f);
	}

	public virtual void StateUpdate()
	{
		
	}

	public virtual void StateExit()
	{
		// フェードOUTを開始
		fader.OnFaded += OnFaded;
		fader.FadeStart(FaderObject.FADE_TYPE.FADE_OUT, 1.0f, 1.0f);
	}
		
	private void OnFaded(GameObject sender)
	{
		fader.OnFaded -= OnFaded;

		// フェードOUT完了でState切り替えを実行 同期してStateを切り替え
		AppMain.Instance.sceneStateManager.SyncState();
	}
}
