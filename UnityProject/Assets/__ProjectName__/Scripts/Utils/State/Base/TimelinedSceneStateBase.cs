using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace GarageKit
{
    public class TimelinedSceneStateBase : AsyncStateBase, ITimelinedSceneState
    {
        [Header("TimelinedSceneStateBase")]
        public int durationSec = -1;

        private TimerEvent timer;
        private const string TIMER_NAME = "TimelinedSceneStateTimer";

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


        protected virtual void Start()
        {
            // タイムライン用タイマーの作成
            for(int i = 0; i < AppMain.Instance.timeManager.timerEvents.Count; i++)
            {
                if(AppMain.Instance.timeManager.timerEvents[i].gameObject.name.Contains(TIMER_NAME))
                {
                    timer = AppMain.Instance.timeManager.timerEvents[i];;
                    break;
                }
            }

            if(timer == null)
            {
                GameObject timerGo = new GameObject(TIMER_NAME);
                timerGo.transform.SetParent(AppMain.Instance.timeManager.gameObject.transform);
                timer = timerGo.AddComponent<TimerEvent>();
                AppMain.Instance.timeManager.timerEvents.Add(timer);
            }
        }

        public override void StateStart(object context)
        {
            base.StateStart(context);

            actionTime = 0.0f;

            isPlay = true;
            isPaused = false;

            actionList = new TimelineEventActionList();
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            // Timelineイベント実行
            actionTime = timer.ElapsedTime;
            actionList.Update(actionTime);
        }

        public override void StateExit()
        {
            base.StateExit();

            actionList.Clear();

            isPlay = false;
            isPaused = false;

            timer.OnCompleteTimer -= OnStateTimer;
        }

        public virtual void OnStateTimer(GameObject sender)
        {

        }


        // タイムラインを開始
        public void StartTimeline()
        {
            if(durationSec < 0)
                durationSec = int.MaxValue;

            timer.OnCompleteTimer += OnStateTimer;
            timer.StartTimer(durationSec);
        }

        // 一時停止
        public virtual void Pause()
        {
            isPaused = true;

            // タイマーを停止
            timer.StopTimer();

            // Tweenライブラリを停止
            DOTween.PauseAll();

            // 再生中のAudioを一時停止
            managedPausingAudios = new List<AudioSource>();
#if UNITY_2023_2_OR_NEWER
            AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
#else
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
#endif
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
#if UNITY_2023_2_OR_NEWER
            Animation[] anims = FindObjectsByType<Animation>(FindObjectsSortMode.None);
#else
            Animation[] anims = FindObjectsOfType<Animation>();
#endif
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
#if UNITY_2023_2_OR_NEWER
            Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
#else
            Animator[] animators = FindObjectsOfType<Animator>();
#endif
            foreach(Animator animator in animators)
            {
                managedPausingAnimators.Add(animator);
                animator.enabled = false;
            }

            // フェードINを開始
            foreach(Fader fader in Fader.Faders)
                fader.StartFade(0.1f, Fader.FADE_TYPE.FADE_IN, 0.3f);
        }

        // 一時停止復帰
        public virtual void Resume()
        {
            isPaused = false;

            // タイマーを再開
            timer.ResumeTimer();

            // Tweenライブラリを再開
            DOTween.PlayAll();

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
            foreach(Fader fader in Fader.Faders)
                fader.StartFade(0.1f, Fader.FADE_TYPE.FADE_OUT, 0.3f);
        }
    }
}
