using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GarageKit
{
	public class UIFadeGroupComponent : MonoBehaviour
	{
		private CanvasGroup uiGroup;

		private float fixTime = 5.0f;
		private float tweenTime = 0.5f;

		private bool isFadeStarted = false;
		private bool isFadeEnded = false;
		private float currentFadeTimer = 0.0f;


		protected virtual void Awake()
		{
			uiGroup = this.gameObject.GetComponent<CanvasGroup>();
		}

		protected virtual void Start()
		{
			ResetFade();
		}
		
		protected virtual void Update()
		{
			TimelinedSceneStateBase stateObj = AppMain.Instance.sceneStateManager.CurrentStateObj as TimelinedSceneStateBase;
			if(stateObj == null || stateObj.IsPaused)
				return;

			if(!isFadeStarted || isFadeEnded)
				return;
			
			if(0.0f <= currentFadeTimer && currentFadeTimer < tweenTime)
			{
				// fade in
				uiGroup.alpha += (1.0f / tweenTime) * Time.deltaTime;
				uiGroup.alpha = Mathf.Min(Mathf.Max(uiGroup.alpha, 0.0f), 1.0f);
			}
			else if(tweenTime <= currentFadeTimer && currentFadeTimer < fixTime)
			{
				// pass
			}
			else if(fixTime <= currentFadeTimer && currentFadeTimer < fixTime + tweenTime)
			{
				// fade out
				uiGroup.alpha -= (1.0f / tweenTime) * Time.deltaTime;
				uiGroup.alpha = Mathf.Min(Mathf.Max(uiGroup.alpha, 0.0f), 1.0f);
			}
			else
			{
				// faded
				isFadeEnded = true;
				uiGroup.alpha = 0.0f;
			}

			currentFadeTimer += Time.deltaTime;
		}


		public void ResetFade()
		{
			uiGroup.alpha = 0.0f;
			currentFadeTimer = 0.0f;

			isFadeStarted = false;
			isFadeEnded = false;
		}

		public void StartFade(float fixTime = 5.0f, float tweenTime = 0.5f)
		{
			this.fixTime = fixTime;
			this.tweenTime = tweenTime;

			ResetFade();

			isFadeStarted = true;
		}

		public void SetUiPosition3D(Vector3 camPos, Vector3 targetPos, float height = 1.8f, float depth = 1.5f)
		{
			Vector3 pos = Vector3.Lerp(camPos, targetPos, depth /  Vector3.Distance(camPos, targetPos));
			this.gameObject.transform.position = new Vector3(pos.x, height, pos.z);

			this.gameObject.transform.LookAt(camPos);
			this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(
				0.0f, this.gameObject.transform.localRotation.eulerAngles.y + 180.0f, 0.0f));
		}
	}
}
