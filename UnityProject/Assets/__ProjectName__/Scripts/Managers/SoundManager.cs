using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

/*
 * 音声再生を管理する
 */
namespace GarageKit
{
	[Serializable]
	public class SoundClipData
	{
		public string name;
		public AudioClip clip;
	}

	[Serializable]
	public class SoundData
	{
		public bool use = true;
		public float volume = 1.0f;
		public SoundClipData[] clips;
	}

	public class SoundManager : ManagerBase
	{
		public SoundData sound_BGM;
		public SoundData sound_SE;
		
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
			sound_BGM.use = ApplicationSetting.Instance.GetBool("UseBGM");
			sound_SE.use = ApplicationSetting.Instance.GetBool("UseSE");
			sound_BGM.volume = ApplicationSetting.Instance.GetFloat("VolBGM");
			sound_SE.volume = ApplicationSetting.Instance.GetFloat("VolSE");

			// AudioSourceを設定
			audioSource_SE = this.gameObject.AddComponent<AudioSource>();
			audioSource_BGM = this.gameObject.AddComponent<AudioSource>();
		}

		protected override void Update()
		{
			base.Update();
		}
		

		// SEの再生
		public void PlaySE(string clipName, bool overlap = false)
		{
			if(sound_SE.use)
			{
				if(!overlap)
				{
					if(audioSource_SE.isPlaying)
						audioSource_SE.Stop();
				}
				
				audioSource_SE.volume = sound_SE.volume;
				
				SoundClipData clipData = sound_SE.clips.First(c => c.name == clipName);
				if(clipData != null)
					audioSource_SE.PlayOneShot(clipData.clip);
			}
		}

		// SEの停止
		public void StopSE()
		{
			if(audioSource_SE.isPlaying)
				audioSource_SE.Stop();
		}

		// BGMの再生
		public void PlayBGM(string clipName, bool loop = true, bool overlap = false)
		{
			if(sound_BGM.use)
			{
				if(!overlap)
				{
					if(audioSource_BGM.isPlaying)
						audioSource_BGM.Stop();
				}
				
				audioSource_BGM.volume = sound_BGM.volume;

				SoundClipData clipData = sound_BGM.clips.First(c => c.name == clipName);
				if(clipData != null)
				{
					audioSource_BGM.clip = clipData.clip;
					audioSource_BGM.loop = loop;
					audioSource_BGM.Play();
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
			FadeBGM(0.0f, sound_BGM.volume, time);
			FadeSE(0.0f, sound_SE.volume, time);
		}

		public void FadeOutAllSound(float time = 1.0f)
		{
			FadeBGM(sound_BGM.volume, 0.0f, time);
			FadeSE(sound_SE.volume, 0.0f, time);

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
