using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

using DG.Tweening;

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
        public List<SoundClipData> clips = new List<SoundClipData>();
    }

    [Serializable]
    public class SoundLayerData
    {
        public string layerName = "";
        public SoundData soundData = new SoundData();

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
    public class Sound3DData
    {
        public bool use = true;
        [Range(0.0f, 1.0f)] public float volume = 1.0f;
        public List<SoundSourceData> sources = new List<SoundSourceData>();
    }

    [Serializable]
    public class Sound3DLayerData
    {
        public string layerName = "";
        public Sound3DData soundData = new Sound3DData();

        public bool IgnoreAllMethod { get; set; }
    }
#endregion

    public class SoundManager : ManagerBase
    {
        public List<SoundLayerData> soundLayers;
        public List<Sound3DLayerData> soundLayers3D;

        private Dictionary<string, AudioSource> audioSources2D;

        private Dictionary<string, SoundLayerData> soundLayersTable;
        public Dictionary<string, SoundLayerData> SoundLayers { get{ return soundLayersTable; } }
        private Dictionary<string, Sound3DLayerData> soundLayers3DTable;
        public Dictionary<string, Sound3DLayerData> SoundLayers3D { get{ return soundLayers3DTable; } }

        [Header("optional MasterAudioMixer settings")]
        public AudioMixerGroup masterMixerGroup;
        public AnimationCurve decibelByStep = AnimationCurve.Linear(-5, -20, 5, 20);
        private int currentMasterVolStep = 0;


        protected override void Awake()
        {
            base.Awake();

            audioSources2D = new Dictionary<string, AudioSource>();
            soundLayersTable = new Dictionary<string, SoundLayerData>();
            soundLayers3DTable = new Dictionary<string, Sound3DLayerData>();

            foreach(SoundLayerData layer in soundLayers)
                AddLayer(layer);

            foreach(Sound3DLayerData layer in soundLayers3D)
                AddLayer3D(layer);

            if(masterMixerGroup != null)
            {
                AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
                foreach(AudioSource source in audioSources)
                    source.outputAudioMixerGroup = masterMixerGroup;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }


#region 2D sound
        public void AddLayer(SoundLayerData layer)
        {
            if(!soundLayersTable.ContainsKey(layer.layerName))
            {
                soundLayersTable.Add(layer.layerName, layer);

                // 2D AudioSourceを設定
                GameObject go = new GameObject(string.Format("AudioSource 2D [{0}]", layer.layerName));
                go.transform.parent = this.gameObject.transform;
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSources2D.Add(layer.layerName, source);
            }
            else
                Debug.LogWarning("SoundManager :: layer is already exists.");
        }

        public void AddClip(string layerName, string clipName, AudioClip clip)
        {
            if(!soundLayersTable.ContainsKey(layerName))
            {
                SoundLayerData layer = new SoundLayerData();
                layer.layerName = layerName;
                AddLayer(layer);
            }

            SoundClipData exist = soundLayersTable[layerName].soundData.clips.Find((c) => c.clipName == clipName);
            if(exist == null)
            {
                SoundClipData clipData = new SoundClipData();
                clipData.clipName = clipName;
                clipData.clip = clip;
                soundLayersTable[layerName].soundData.clips.Add(clipData);
            }
            else
                Debug.LogWarning("SoundManager :: clip is already exists.");
        }

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

        public bool IsPlay(string layerName)
        {
            if(!audioSources2D.ContainsKey(layerName))
                return false;

            return audioSources2D[layerName].isPlaying;
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
        public void AddLayer3D(Sound3DLayerData layer)
        {
            if(!soundLayers3DTable.ContainsKey(layer.layerName))
                soundLayers3DTable.Add(layer.layerName, layer);
            else
                Debug.LogWarning("SoundManager :: layer is already exists.");
        }

        public void AddSource3D(string layerName, string sourceName, AudioSource source)
        {
            if(!soundLayers3DTable.ContainsKey(layerName))
            {
                Sound3DLayerData layer = new Sound3DLayerData();
                layer.layerName = layerName;
                AddLayer3D(layer);
            }

            SoundSourceData exist = soundLayers3DTable[layerName].soundData.sources.Find((s) => s.sourceName == sourceName);
            if(exist == null)
            {
                SoundSourceData sourceData = new SoundSourceData();
                sourceData.sourceName = sourceName;
                sourceData.source = source;
                soundLayers3DTable[layerName].soundData.sources.Add(sourceData);
            }
            else
                Debug.LogWarning("SoundManager :: source is already exists.");
        }

        public void Play3D(string layerName, string sourceName, bool overlap = false)
        {
            Play3D(layerName, sourceName, overlap, false, true);
        }

        public void Play3D(string layerName, string sourceName, bool overlap, bool loop, bool asOneShot)
        {
            Sound3DLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
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

        public bool IsPlay3D(string layerName, string sourceName)
        {
            Sound3DLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
            if(layer == null)
                return false;

            if(layer.soundData.use)
            {
                SoundSourceData sourceData = layer.soundData.sources.FirstOrDefault(c => c.sourceName == sourceName);
                if(sourceData == null)
                    return false;

                return sourceData.source.isPlaying;
            }
            else
                return false;
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
                Sound3DLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
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
                foreach(Sound3DLayerData layer in soundLayers3D)
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
            foreach(Sound3DLayerData layer in soundLayers3D)
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
            foreach(Sound3DLayerData layer in soundLayers3D)
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

                DOVirtual.Float(fromVol, toVol, time,
                    (v) => {
                        cb.fade_updated(v);
                    })
                    .OnComplete(() => {
                        cb.fade_completed();
                    })
                    .Play();
            }
        }

        public void Fade3D(string layerName, string sourceName, float fromVol, float toVol, float time)
        {
            Sound3DLayerData layer = soundLayers3D.FirstOrDefault(l => l.layerName == layerName);
            if(layer == null)
                return;

            SoundSourceData sourceData = layer.soundData.sources.FirstOrDefault(c => c.sourceName == sourceName);
            if(sourceData == null)
                return;

            GameObject target = sourceData.source.gameObject;
            SoundFadeCallbackBehabiour cb = target.AddComponent<SoundFadeCallbackBehabiour>();
            cb.audioSource = sourceData.source;

            DOVirtual.Float(fromVol, toVol, time,
                    (v) => {
                        cb.fade_updated(v);
                    })
                    .OnComplete(() => {
                        cb.fade_completed();
                    })
                    .Play();
        }
#endregion

#region MasterAudioMixer
        public void SetMasterVol(float vol, string exposeProperty = "MasterVolume")
        {
            if(masterMixerGroup == null)
            {
                Debug.LogWarning("SoundManager :: MasterAudioMixer is null");
                return;
            }

            masterMixerGroup.audioMixer.SetFloat(exposeProperty, vol);
            Debug.Log(string.Format("SoundManager :: MasterVolume > {0:F3}", vol));
        }

        public void MasterVolUp(string exposeProperty = "MasterVolume")
        {
            currentMasterVolStep++;

            int max = (int)decibelByStep.keys[decibelByStep.keys.Length - 1].time;
            currentMasterVolStep = Mathf.Min(max, currentMasterVolStep);

            SetMasterVol(decibelByStep.Evaluate(currentMasterVolStep), exposeProperty);
        }

        public void MasterVolDown(string exposeProperty = "MasterVolume")
        {
            currentMasterVolStep--;

            int min = (int)decibelByStep.keys[0].time;
            currentMasterVolStep = Mathf.Max(min, currentMasterVolStep);

            SetMasterVol(decibelByStep.Evaluate(currentMasterVolStep), exposeProperty);
        }
#endregion
    }

    // fade callbacks
    public class SoundFadeCallbackBehabiour : MonoBehaviour
    {
        public AudioSource audioSource;

        public void fade_updated(float newValue)
        {
            audioSource.volume = newValue;
        }

        public void fade_completed()
        {
            if(audioSource.isPlaying)
                audioSource.Stop();

            SoundFadeCallbackBehabiour.Destroy(this);
        }
    }
}
