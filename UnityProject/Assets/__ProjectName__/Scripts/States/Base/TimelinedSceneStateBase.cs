using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GarageKit
{
	public class TimelinedSceneStateBase : AsyncStateBase, ITimelinedSceneState
	{
		[Header("TimelinedSceneStateBase")]
		public int durationSec = 30;

		// イベントAction
		protected TimelineEventActionList actionList;
		private float actionTime = 0.0f;
		public float CurrentActionTime { get{ return actionTime;} }

		protected bool isPlay = false;
		public bool IsPlay { get{ return isPlay; } }
		protected bool isPaused = false;
		public bool IsPaused { get{ return isPaused; } }

		// 一時停止管理用
		private static List<AudioSource> managedPausingAudios;
		private static Dictionary<Animation, float> managedPausingAnimations;
		private static List<Animator> managedPausingAnimators;


		public override void StateStart(object context)
		{
			base.StateStart(context);
			
			actionTime = 0.0f;

			isPlay = true;
			isPaused = false;

			actionList = new TimelineEventActionList();

			// タイマー設定
			if(durationSec > 0)
			{
				AppMain.Instance.timeManager.mainTimer.OnCompleteTimer += OnStateTimer;
				AppMain.Instance.timeManager.mainTimer.StartTimer(durationSec);
			}
			else
				AppMain.Instance.timeManager.mainTimer.StartTimer(int.MaxValue);
		}

		public override void StateUpdate()
		{
			base.StateUpdate();

			// Timelineイベント実行
			actionTime = AppMain.Instance.timeManager.mainTimer.ElapsedTime;
			actionList.Update(actionTime);
		}

		public override void StateExit()
		{
			base.StateExit();

			actionList.Clear();

			isPlay = false;
			isPaused = false;

			AppMain.Instance.timeManager.mainTimer.OnCompleteTimer -= OnStateTimer;
		}

		public virtual void OnStateTimer(GameObject sender)
		{

		}


		// 一時停止
		public virtual void Pause()
		{
			isPaused = true;

			// タイマーを停止
			AppMain.Instance.timeManager.mainTimer.StopTimer();

			// Tweenライブラリを停止
			iTween.Pause();

			// 再生中のAudioを一時停止
			managedPausingAudios = new List<AudioSource>();
			AudioSource[] sources = FindObjectsOfType<AudioSource>();
			foreach(AudioSource source in sources)
			{
				if(source.isPlaying)
				{
					managedPausingAudios.Add(source);
					source.Pause();
				}
			}

			// 再生中のAnimationを一時停止
			managedPausingAnimations = new Dictionary<Animation, float>();
			Animation[] anims = FindObjectsOfType<Animation>();
			foreach(Animation anim in anims)
			{
				if(anim.isPlaying)
				{
					managedPausingAnimations.Add(anim, anim[anim.clip.name].time);
					anim.Stop();
				}
			}

			// 再生中のAnimatorを一時停止
			managedPausingAnimators = new List<Animator>();
			Animator[] animators = FindObjectsOfType<Animator>();
			foreach(Animator animator in animators)
			{
				managedPausingAnimators.Add(animator);
				animator.enabled = false;
			}

			// フェードINを開始
			foreach(Fader fader in this.faders)
				fader.StartFade(0.1f, Fader.FADE_TYPE.FADE_IN, 0.3f);
		}

		// 一時停止復帰
		public virtual void Resume()
		{
			isPaused = false;

			// タイマーを再開
			AppMain.Instance.timeManager.mainTimer.ResumeTimer();

			// Tweenライブラリを再開
			iTween.Resume();

			// Audioを一時停止復帰
			foreach(AudioSource source in managedPausingAudios)
				source.UnPause();
			managedPausingAudios = new List<AudioSource>();

			// Animationを一時停止復帰
			foreach(Animation anim in managedPausingAnimations.Keys)
			{
				anim[anim.clip.name].time = managedPausingAnimations[anim];
				anim.Play();
			}
			managedPausingAnimations = new Dictionary<Animation, float>();

			// Animator
			foreach(Animator animator in managedPausingAnimators)
				animator.enabled = true;
			managedPausingAnimators = new List<Animator>();

			// フェードOUTを開始
			foreach(Fader fader in this.faders)
				fader.StartFade(0.1f, Fader.FADE_TYPE.FADE_OUT, 0.3f);
		}
	}
}
