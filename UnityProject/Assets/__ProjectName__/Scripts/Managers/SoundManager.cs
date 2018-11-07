using UnityEngine;
using System.Collections;
using System.IO;
using System;

/*
 * 音声再生を管理する
 */
namespace GarageKit
{
	public class SoundManager : ManagerBase
	{
		// SE選択用
		public enum SE
		{
			CLICK = 0,
			CANCEL
		}
		
		// BGM選択用
		public enum BGM
		{
			WAIT = 0,
			PLAY
		}

		public bool useBGM = true;
		public bool useSE = true;
		
		public float volBGM = 1.0f;
		public float volSE = 1.0f;
		
		public AudioClip BGM_Wait;
		public AudioClip BGM_Play;
		public AudioClip SE_Click;
		public AudioClip SE_Cancel;
		
		private AudioSource audioSource_BGM;
		private AudioSource audioSource_SE;
		
		
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();

			// 設定値を取得
			useBGM = ApplicationSetting.Instance.GetBool("UseBGM");
			useSE = ApplicationSetting.Instance.GetBool("UseSE");
			volBGM = ApplicationSetting.Instance.GetFloat("VolBGM");
			volSE = ApplicationSetting.Instance.GetFloat("VolSE");

			// AudioSourceを設定
			audioSource_SE = this.gameObject.AddComponent<AudioSource>();
			audioSource_BGM = this.gameObject.AddComponent<AudioSource>();
		}

		protected override void Update()
		{
			base.Update();
		}
		

		// SEの再生
		public void PlaySE(SE type, bool overlap = false)
		{
			if(useSE)
			{
				if(!overlap)
				{
					if(audioSource_SE.isPlaying)
						audioSource_SE.Stop();
				}
				
				audioSource_SE.volume = volSE;
				
				switch(type)
				{
					case SE.CLICK:
						if(SE_Click != null)
							audioSource_SE.PlayOneShot(SE_Click);
						break;
					
					case SE.CANCEL:
						if(SE_Cancel != null)
							audioSource_SE.PlayOneShot(SE_Cancel);
						break;
					
					default: break;
				}
			}
		}

		// SEの停止
		public void StopSE()
		{
			if(audioSource_SE.isPlaying)
				audioSource_SE.Stop();
		}

		// BGMの再生
		public void PlayBGM(BGM type, bool loop = true, bool overlap = false)
		{
			if(useBGM)
			{
				if(!overlap)
				{
					if(audioSource_BGM.isPlaying)
						audioSource_BGM.Stop();
				}
				
				audioSource_BGM.volume = volBGM;
				
				switch(type)
				{
					case BGM.WAIT:
						if(BGM_Wait != null)
						{
							audioSource_BGM.clip = BGM_Wait;
							audioSource_BGM.loop = loop;
							audioSource_BGM.Play();
						}
						break;
					
					case BGM.PLAY:
						if(BGM_Play != null)
						{
							audioSource_BGM.clip = BGM_Play;
							audioSource_BGM.loop = loop;
							audioSource_BGM.Play();
						}
						break;
					
					default: break;
				}
			}
		}

		// BGMの停止
		public void StopBGM()
		{
			if(audioSource_BGM.isPlaying)
				audioSource_BGM.Stop();
		}

#region Fade

		public void FadeInAllSound(float time = 1.0f)
		{
			FadeBGM(0.0f, volBGM, time);
			FadeSE(0.0f, volSE, time);
		}

		public void FadeOutAllSound(float time = 1.0f)
		{
			FadeBGM(volBGM, 0.0f, time);
			FadeSE(volSE, 0.0f, time);

			Invoke("StopBGM", time);
			Invoke("StopSE", time);
		}

		public void FadeBGM(float fromVol, float toVol, float time)
		{
			iTween.ValueTo(this.gameObject,
				iTween.Hash(
					"from", fromVol,
					"to", toVol,
					"time", time,
					"onupdate", "volume_bgm_updated",
					"onupdatetarget", this.gameObject));
		}

		private void volume_bgm_updated(float newValue)
		{
			audioSource_BGM.volume = newValue;
		}

		public void FadeSE(float fromVol, float toVol, float time)
		{
			iTween.ValueTo(this.gameObject,
				iTween.Hash(
					"from", fromVol,
					"to", toVol,
					"time", time,
					"onupdate", "volume_se_updated",
					"onupdatetarget", this.gameObject));
		}

		private void volume_se_updated(float newValue)
		{
			audioSource_SE.volume = newValue;
		}
		
#endregion
	}
}
