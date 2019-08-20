using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

/*
 * Manage all sound controll in scene
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

	[Serializable]
	public class SoundSourceData
	{
		public string name;
		public AudioSource source;
	}

	[Serializable]
	public class Sound3dData
	{
		public bool use = true;
		public float volume = 1.0f;
		public SoundSourceData[] sources;
	}

	public class SoundManager : ManagerBase
	{
		public SoundData sound_BGM;
		public SoundData sound_SE;
		public Sound3dData sound_SE3D;
		
		private AudioSource audioSource_BGM;
		private AudioSource audioSource_SE;
		
		
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();

			// 2D AudioSourceを設定
			audioSource_SE = this.gameObject.AddComponent<AudioSource>();
			audioSource_BGM = this.gameObject.AddComponent<AudioSource>();
		}

		protected override void Update()
		{
			base.Update();
		}
		

#region BGM
		public void PlayBGM(string clipName, bool loop = true, bool overlap = false)
		{
			if(sound_BGM.use)
			{
				SoundClipData clipData = sound_BGM.clips.FirstOrDefault(c => c.name == clipName);
				if(clipData == null)
					return;

				if(!overlap)
				{
					if(audioSource_BGM.isPlaying)
						audioSource_BGM.Stop();
				}
				
				audioSource_BGM.volume = sound_BGM.volume;
				audioSource_BGM.clip = clipData.clip;
				audioSource_BGM.loop = loop;
				audioSource_BGM.Play();
			}
		}

		public void StopBGM()
		{
			if(audioSource_BGM.isPlaying)
				audioSource_BGM.Stop();
		}
#endregion

#region SE
		public void PlaySE(string clipName, bool overlap = false)
		{
			if(sound_SE.use)
			{
				SoundClipData clipData = sound_SE.clips.FirstOrDefault(c => c.name == clipName);
				if(clipData == null)
					return;

				if(!overlap)
				{
					if(audioSource_SE.isPlaying)
						audioSource_SE.Stop();
				}
				
				audioSource_SE.volume = sound_SE.volume;
				audioSource_SE.PlayOneShot(clipData.clip);
			}
		}

		public void PlaySE3D(string sourceName, bool overlap = false)
		{
			if(sound_SE3D.use)
			{
				SoundSourceData sourceData = sound_SE3D.sources.FirstOrDefault(c => c.name == sourceName);
				if(sourceData == null)
					return;
				
				if(!overlap)
				{
					if(sourceData.source.isPlaying)
						sourceData.source.Stop();
				}
				
				sourceData.source.volume = sound_SE3D.volume;
				sourceData.source.PlayOneShot(sourceData.source.clip);
			}
		}

		public void StopSE()
		{
			if(audioSource_SE.isPlaying)
				audioSource_SE.Stop();

			foreach(SoundSourceData sourceData in sound_SE3D.sources)
			{
				if(sourceData.source.isPlaying)
					sourceData.source.Stop();
			}
		}
#endregion

#region Fade
		public void FadeInAllSound(float time = 1.0f)
		{
			FadeBGM(0.0f, sound_BGM.volume, time);
			FadeSE(0.0f, sound_SE.volume, time);
			FadeSE3D(0.0f, sound_SE3D.volume, time);
		}

		public void FadeOutAllSound(float time = 1.0f)
		{
			FadeBGM(sound_BGM.volume, 0.0f, time);
			FadeSE(sound_SE.volume, 0.0f, time);
			FadeSE3D(sound_SE3D.volume, 0.0f, time);

			Invoke("StopBGM", time);
			Invoke("StopSE", time);
			Invoke("StopSE3D", time);
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

		public void FadeSE3D(float fromVol, float toVol, float time)
		{
			iTween.ValueTo(this.gameObject,
				iTween.Hash(
					"from", fromVol,
					"to", toVol,
					"time", time,
					"onupdate", "volume_se3d_updated",
					"onupdatetarget", this.gameObject));
		}

		private void volume_se3d_updated(float newValue)
		{
			foreach(SoundSourceData sourceData in sound_SE3D.sources)
				sourceData.source.volume = newValue;
		}
#endregion
	}
}
