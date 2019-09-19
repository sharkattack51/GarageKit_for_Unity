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
#region 2D sound
	[Serializable]
	public class SoundClipData
	{
		public string clipName = "";
		public AudioClip clip;
	}

	[Serializable]
	public class SoundData
	{
		public bool use = true;
		[Range(0.0f, 1.0f)] public float volume = 1.0f;
		public SoundClipData[] clips;
	}

	[Serializable]
	public class SoundLayerData
	{
		public string layerName = "";
		public SoundData soundData;

		private bool ignoreAllMethod = false;
		public bool IgnoreAllMethod { get; set; }
	}
#endregion

#region 3D sound
	[Serializable]
	public class SoundSourceData
	{
		public string sourceName = "";
		public AudioSource source;
	}

	[Serializable]
	public class Sound3dData
	{
		public bool use = true;
		[Range(0.0f, 1.0f)] public float volume = 1.0f;
		public SoundSourceData[] sources;
	}

	[Serializable]
	public class Sound3dLayerData
	{
		public string layerName = "";
		public Sound3dData soundData;

		private bool ignoreAllMethod = false;
		public bool IgnoreAllMethod { get; set; }
	}
#endregion

	public class SoundManager : ManagerBase
	{
		public SoundLayerData[] soundLayers;
		public Sound3dLayerData[] soundLayers3D;

		private Dictionary<string, AudioSource> audioSources2D;

		private Dictionary<string, SoundLayerData> soundLayersTable;
		public Dictionary<string, SoundLayerData> SoundLayers { get{ return soundLayersTable; } }
		private Dictionary<string, Sound3dLayerData> soundLayers3DTable;
		public Dictionary<string, Sound3dLayerData> SoundLayers3D { get{ return soundLayers3DTable; } }

		
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();

			audioSources2D = new Dictionary<string, AudioSource>();
			soundLayersTable = new Dictionary<string, SoundLayerData>();
			soundLayers3DTable = new Dictionary<string, Sound3dLayerData>();

			foreach(SoundLayerData layer in soundLayers)
			{
				soundLayersTable.Add(layer.layerName, layer);

				// 2D AudioSourceを設定
				GameObject go = new GameObject(string.Format("AudioSource 2D [{0}]", layer.layerName));
				go.transform.parent = this.gameObject.transform;
				AudioSource source = go.AddComponent<AudioSource>();
				source.playOnAwake = false;
				audioSources2D.Add(layer.layerName, source);
			}

			foreach(Sound3dLayerData layer in soundLayers3D)
				soundLayers3DTable.Add(layer.layerName, layer);
		}

		protected override void Update()
		{
			base.Update();
		}
		

#region 2D sound
		public void Play(string layerName, string clipName, bool overlap = false)
		{
			Play(layerName, clipName, overlap, false, true);
		}

		public void Play(string layerName, string clipName, bool overlap, bool loop, bool asOneShot)
		{
			if(!audioSources2D.ContainsKey(layerName))
				return;
			
			SoundLayerData layer = soundLayers.FirstOrDefault(l => l.layerName == layerName);
			if(layer == null)
				return;

			if(layer.soundData.use)
			{
				SoundClipData clipData = layer.soundData.clips.FirstOrDefault(c => c.clipName == clipName);
				if(clipData == null)
					return;

				if(!overlap)
				{
					if(audioSources2D[layerName].isPlaying)
						audioSources2D[layerName].Stop();
				}
				
				audioSources2D[layerName].volume = layer.soundData.volume;
				audioSources2D[layerName].clip = clipData.clip;

				if(asOneShot)
					audioSources2D[layerName].PlayOneShot(clipData.clip);
				else
				{
					audioSources2D[layerName].loop = loop;
					audioSources2D[layerName].Play();
				}
			}
		}

		public void Stop(string layerName = "")
		{
			InternalStop(layerName, false);
		}

		public void StopAll(string layerName = "")
		{
			InternalStop(layerName, true);
		}

		private void InternalStop(string layerName, bool asAll)
		{
			if(layerName != "")
			{
				if(!audioSources2D.ContainsKey(layerName))
					return;
				
				if(audioSources2D[layerName].isPlaying)
					audioSources2D[layerName].Stop();
			}
			else
			{
				foreach(SoundLayerData layer in soundLayers)
				{
					if(asAll && !layer.IgnoreAllMethod)
					{
						if(audioSources2D[layer.layerName].isPlaying)
							audioSources2D[layer.layerName].Stop();
					}
				}
			}
		}
#endregion

#region 3D sound
		public void Play3D(string layerName, string sourceName, bool overlap = false)
		{
			Play3D(layerName, sourceName, overlap, false, true);
		}

		public void Play3D(string layerName, string sourceName, bool overlap, bool loop, bool asOneShot)
		{
			Sound3dLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
			if(layer == null)
				return;

			if(layer.soundData.use)
			{
				SoundSourceData sourceData = layer.soundData.sources.FirstOrDefault(c => c.sourceName == sourceName);
				if(sourceData == null)
					return;
				
				if(!overlap)
				{
					if(sourceData.source.isPlaying)
						sourceData.source.Stop();
				}
				
				sourceData.source.volume = layer.soundData.volume;

				if(asOneShot)
					sourceData.source.PlayOneShot(sourceData.source.clip);
				else
				{
					sourceData.source.loop = loop;
					sourceData.source.Play();
				}
			}
		}

		public void Stop3D(string layerName = "", string sourceName = "")
		{
			InternalStop3D(layerName, sourceName, false);
		}

		public void Stop3DAll(string layerName = "")
		{
			InternalStop3D(layerName, "", true);
		}

		private void InternalStop3D(string layerName, string sourceName, bool asAll)
		{
			if(layerName != "")
			{
				Sound3dLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
				if(layer == null)
					return;
				
				if(sourceName == "")
				{
					foreach(SoundSourceData sourceData in layer.soundData.sources)
					{
						if(sourceData.source.isPlaying)
							sourceData.source.Stop();
					}
				}
				else
				{
					SoundSourceData sourceData = layer.soundData.sources.FirstOrDefault(c => c.sourceName == sourceName);
					if(sourceData == null)
						return;

					if(sourceData.source.isPlaying)
						sourceData.source.Stop();
				}
			}
			else
			{
				foreach(Sound3dLayerData layer in soundLayers3D)
				{
					if(asAll && !layer.IgnoreAllMethod)
					{
						foreach(SoundSourceData sourceData in layer.soundData.sources)
						{
							if(sourceData.source.isPlaying)
								sourceData.source.Stop();
						}
					}
				}
			}
		}
#endregion

#region Fade
		public void FadeInAllSound(float time = 1.0f)
		{
			foreach(SoundLayerData layer in soundLayers)
			{
				if(!layer.IgnoreAllMethod)
					Fade(layer.layerName, 0.0f, layer.soundData.volume, time);
			}
			foreach(Sound3dLayerData layer in soundLayers3D)
			{
				if(!layer.IgnoreAllMethod)
				{
					foreach(SoundSourceData source in layer.soundData.sources)
						Fade3D(layer.layerName, source.sourceName, 0.0f, layer.soundData.volume, time);
				}
			}
		}

		public void FadeOutAllSound(float time = 1.0f)
		{
			foreach(SoundLayerData layer in soundLayers)
			{
				if(!layer.IgnoreAllMethod)
					Fade(layer.layerName, layer.soundData.volume, 0.0f, time);
			}
			foreach(Sound3dLayerData layer in soundLayers3D)
			{
				if(!layer.IgnoreAllMethod)
				{
					foreach(SoundSourceData source in layer.soundData.sources)
						Fade3D(layer.layerName, source.sourceName, layer.soundData.volume, 0.0f, time);
				}
			}

			StartCoroutine(FadeOutAllSoundCoroutine(time));
		}

		private IEnumerator FadeOutAllSoundCoroutine(float time)
		{
			yield return new WaitForSeconds(time);

			StopAll();
			Stop3DAll();
		}

		public void Fade(string layerName, float fromVol, float toVol, float time)
		{
			if(audioSources2D.ContainsKey(layerName))
			{
				GameObject target = audioSources2D[layerName].gameObject;
				SoundFadeCallbackBehabiour cb = target.AddComponent<SoundFadeCallbackBehabiour>();
				cb.audioSource = audioSources2D[layerName];

				iTween.ValueTo(target,
					iTween.Hash(
						"from", fromVol,
						"to", toVol,
						"time", time,
						"onupdate", "fade_updated",
						"onupdatetarget", target,
						"oncomplete", "fade_completed",
						"oncompletetarget", target));
			}
		}

		public void Fade3D(string layerName, string sourceName, float fromVol, float toVol, float time)
		{
			Sound3dLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
			if(layer == null)
				return;

			SoundSourceData sourceData = layer.soundData.sources.FirstOrDefault(c => c.sourceName == sourceName);
			if(sourceData == null)
				return;

			GameObject target = sourceData.source.gameObject;
			SoundFadeCallbackBehabiour cb = target.AddComponent<SoundFadeCallbackBehabiour>();
			cb.audioSource = sourceData.source;

			iTween.ValueTo(target,
				iTween.Hash(
					"from", fromVol,
					"to", toVol,
					"time", time,
					"onupdate", "fade_updated",
					"onupdatetarget", target,
					"oncomplete", "fade_completed",
					"oncompletetarget", target));
		}
#endregion
	}

	// fade callbacks
	public class SoundFadeCallbackBehabiour : MonoBehaviour
	{
		public AudioSource audioSource;

		private void fade_updated(float newValue)
		{
			audioSource.volume = newValue;
		}

		private void fade_completed()
		{
			if(audioSource.isPlaying)
				audioSource.Stop();

			SoundFadeCallbackBehabiour.Destroy(this);
		}
	}
}
