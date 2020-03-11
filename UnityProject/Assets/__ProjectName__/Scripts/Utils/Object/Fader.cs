using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * カメラに設定してGL描画でフェードを行う
 * 全カメラの3Dレンダリングの最後で描画される
 */ 
namespace GarageKit
{
	[RequireComponent(typeof(Camera))]
	public class Fader : MonoBehaviour
	{
		private static bool useFade = true;
		public static bool UseFade { get{ return useFade && (Faders.Count > 0); } }

		private static List<Fader> faders = new List<Fader>();
		public static List<Fader> Faders { get{ return faders; } }
		
		public float fadeTime = 1.0f;
		public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		public Material fadeMaterial = null;
		
		public enum FADE_TYPE
		{
			FADE_IN = 0,
			FADE_OUT
		}
		public FADE_TYPE fadeType = FADE_TYPE.FADE_IN;
		
		private bool isFading = false;
		public bool IsFading { get{ return isFading; } }
		
		
		void Awake()
		{
			Faders.Add(this);

			if(fadeMaterial == null)
				fadeMaterial = new Material(Shader.Find("Custom/ShaderLib/Unlit/MaterialAlpha"));
			fadeMaterial.color = fadeColor;
		}
		
		void Start()
		{

		}
		
		void OnPostRender()
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0.0f, 0.0f, -1.0f);
			GL.Vertex3(0.0f, 1.0f, -1.0f);
			GL.Vertex3(1.0f, 1.0f, -1.0f);
			GL.Vertex3(1.0f, 0.0f, -1.0f);
			GL.End();
			GL.PopMatrix();
		}

		// Fadeの有効無効
		public static void EnableFade()
		{
			useFade = true;
		}

		public static void DisableFade()
		{
			foreach(Fader fader in Faders)
			{
				Color color = fader.fadeMaterial.color;
				fader.fadeMaterial.color = new Color(color.r, color.g, color.b, 0.0f);
			}

			useFade = false;
		}
		
		// Fadeの開始
		public static void StartFadeAll(FADE_TYPE fadeType)
		{
			foreach(Fader fader in Fader.faders)
				fader.StartFade(fader.fadeTime, fadeType);
		}

		public static void StartFadeAll(float fadeTime, FADE_TYPE fadeType)
		{
			foreach(Fader fader in Fader.faders)
				fader.StartFade(fadeTime, fadeType);
		}
		
		public void StartFade(FADE_TYPE fadeType, float targetAlpha = 1.0f)
		{
			StartFade(fadeTime, fadeType, targetAlpha);
		}

		public void StartFade(float fadeTime, FADE_TYPE fadeType, float targetAlpha = 1.0f)
		{
			if(UseFade)
			{
				this.fadeTime = fadeTime;
				this.fadeType = fadeType;
			
				StartCoroutine(FadeIn(targetAlpha));
			}
		}
		
		IEnumerator FadeIn(float targetAlpha)
		{
			float elapsedTime = 0.0f;
			Color color = fadeColor;
			isFading = true;

			while(elapsedTime < fadeTime)
			{
				yield return new WaitForEndOfFrame();

				elapsedTime += Time.deltaTime;

				if(fadeType == FADE_TYPE.FADE_IN)
					color.a = targetAlpha - Mathf.Clamp01(elapsedTime / fadeTime);
				else if(fadeType == FADE_TYPE.FADE_OUT)
					color.a = Mathf.Clamp01(elapsedTime / fadeTime);

				fadeMaterial.color = color;
			}
			
			isFading = false;
		}
	}
}
