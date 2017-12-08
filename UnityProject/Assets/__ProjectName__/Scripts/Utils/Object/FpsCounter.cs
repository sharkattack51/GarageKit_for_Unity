using UnityEngine;
using UnityEngine.UI;
using System;

/*
 * uGUIによるFPSカウンター
 */ 
namespace GarageKit
{
	public class FpsCounter : MonoBehaviour
	{
		public bool displayFPS = true;
		public Text text;
		public int targetFPS = 90;
		public Color goodColor = Color.green;
		public Color warnColor = Color.yellow;
		public Color badColor = Color.red;

		private const float updateInterval = 0.5f;
		private int framesCount;
		private float framesTime;


		void Awake()
		{
			
		}

		void Start()
		{
			if(text == null)
				text = this.gameObject.GetComponent<Text>();
		}

		void Update()
		{
			framesCount++;
			framesTime += Time.unscaledDeltaTime;

			if(framesTime > updateInterval)
			{
				if(text != null)
				{
					if(displayFPS)
					{
						float fps = framesCount / framesTime;
						text.text = String.Format("{0:F2} FPS", fps);
						text.color = (fps > (targetFPS - 5) ? goodColor : (fps > (targetFPS - 30) ? warnColor : badColor));
					}
					else
					{
						text.text = "";
					}
				}

				framesCount = 0;
				framesTime = 0;
			}
		}
	}
}