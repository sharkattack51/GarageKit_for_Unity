using UnityEngine;
using System.Collections;

/*
 * マテリアルの自動点滅アニメーション
 */
namespace GarageKit
{
	public class AutoBrink : MonoBehaviour
	{	
		public bool isPinpon = true;
		public float brinkTime = 1.0f;
		public float startAlpha = 1.0f;
		public float endAlpha = 0.0f;
		
		
		void Start()
		{
			StartTween();
		}
		
		private void StartTween()
		{
			Color startColor = this.gameObject.GetComponent<Renderer>().material.color;
			this.gameObject.GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, startAlpha);
			
			iTween.LoopType loopType;
			if(isPinpon)
			{
				loopType = iTween.LoopType.pingPong;
				brinkTime *= 0.5f;
			}
			else
				loopType = iTween.LoopType.loop;
			
			iTween.ColorTo(
				this.gameObject,
				iTween.Hash(
					"time", brinkTime,
					"alpha", endAlpha,
					"looptype", loopType,
					"easetype", iTween.EaseType.linear
				)
			);
		}
	}
}
