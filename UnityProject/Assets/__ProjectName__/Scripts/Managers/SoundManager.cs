using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class SoundManager : MonoBehaviour
{
	//SE選択用
	public enum SE
	{
		CLICK = 0,
		CANCEL
	}
	
	//BGM選択用
	public enum BGM
	{
		WAIT = 0,
		PLAY
	}
	
	//singleton
	private static SoundManager instance;
	public static SoundManager Instance { get{ return instance; } }
	
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
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		//設定値を取得
		ApplicationSetting setting = ApplicationSetting.Instance;
		if(setting.IsValid)
		{
			if(setting.Data.ContainsKey("UseBGM")) useBGM = bool.Parse(setting.Data["UseBGM"]);
			if(setting.Data.ContainsKey("UseSE")) useSE = bool.Parse(setting.Data["UseSE"]);
			if(setting.Data.ContainsKey("VolBGM")) volBGM = float.Parse(setting.Data["VolBGM"]);
			if(setting.Data.ContainsKey("VolSE")) volSE = float.Parse(setting.Data["VolSE"]);
		}
		
		//AudioSourceを設定
		audioSource_SE = this.gameObject.AddComponent<AudioSource>();
		audioSource_BGM = this.gameObject.AddComponent<AudioSource>();
	}
	
	
	/// <summary>
	/// SEの再生
	/// </summary>
	/// <param name="type">
	/// A <see cref="SE"/>
	/// </param>
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
	
	
	/// <summary>
	/// BGMの再生
	/// </summary>
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
}
