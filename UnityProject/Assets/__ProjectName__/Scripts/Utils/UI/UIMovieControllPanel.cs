//#define AVPRO_VIDEO
//#define TMP

/*
If you want to use this script, please import AVProVideo and TextMeshPro. and Uncomment #define.
Add the DIsplayUGUI and Text components to the Prefabs/Utils/UI/MovieControllPanel
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if AVPRO_VIDEO
using RenderHeads.Media.AVProVideo;
#endif

#if TMP
using TMPro;
#endif

namespace GarageKit
{
    public class UIMovieControllPanel : MonoBehaviour
    {
#if AVPRO_VIDEO
        public MediaPlayer player;
        public bool setupOnStart = true;

        [Header("UI")]
        public DisplayUGUI uiMovie;
        public Button uiPlayPauseBtn;
        public Slider uiSeekSlider;
#if TMP
        public TMP_Text uiElapsedTxt;
#else
        public Text uiElapsedTxt;
#endif
        public EventTrigger seekEventTrg;

        [Header("Play/Pause button sprites")]
        public Sprite playSprite;
        public Sprite pauseSprite;

        private bool movieLoaded = false;
        private bool isSeeking = false;

        public Action OnPlay;
        public Action OnPause;
        public Action<float> OnSeekStart;
        public Action<float> OnSeek;
        public Action<float> OnSeekEnd;
        public Action OnFinishedPlaying;


        void Awake()
        {
            if(player.AutoOpen || player.AutoStart)
                Debug.LogError("AVPro MediaPlayer setting [Auto Open] & [Auto Play] to False");
        }

        void Start()
        {
            if(setupOnStart)
                Setup();
        }

        void Update()
        {
            if(player != null && player.MediaOpened)
            {
                // ボタンスプライトの切り替え
                if(!player.Control.IsSeeking())
                {
                    if(!player.Control.IsPlaying() || player.Control.IsFinished())
                        uiPlayPauseBtn.image.sprite = playSprite;
                    else
                        uiPlayPauseBtn.image.sprite = pauseSprite;
                }

                // シークバーの更新
                if(movieLoaded && player.Control.CanPlay() && !player.Control.IsPaused())
                    uiSeekSlider.value = (float)player.Control.GetCurrentTime() / (float)player.Info.GetDuration();

                // 経過時間の更新
                uiElapsedTxt.text = string.Format("{0:D2}:{1:D2} / {2:D2}:{3:D2}",
                    (int)(player.Control.GetCurrentTime() / 60.0f),
                    (int)(player.Control.GetCurrentTime() % 60.0f),
                    (int)(player.Info.GetDuration() / 60.0f),
                    (int)(player.Info.GetDuration() % 60.0f));
            }
        }

        void OnEnable()
        {
            if(movieLoaded && player.Control.CanPlay())
                uiSeekSlider.value = (float)player.Control.GetCurrentTime() / (float)player.Info.GetDuration();
        }

        void OnDestroy()
        {
            Clear();
        }


        public void Clear()
        {
            if(player != null)
            {
                player.Stop();
                player.CloseMedia();
            }
        }

        public void Setup()
        {
            // MediaPlayerイベント
            player.Events.AddListener((mp, e, err) => {
                switch(e)
                {
                    case MediaPlayerEvent.EventType.FinishedPlaying:
                        OnFinishedPlaying?.Invoke();
                        break;

                    default: break;
                }
            });

            // 再生/一時停止ボタン
            uiPlayPauseBtn.onClick.AddListener(() => {
                if(!player.Control.IsPlaying() || player.Control.IsFinished())
                {
                    if(!movieLoaded)
                        Debug.LogWarning("not loaded movie. please use UIMovieControllPanel.Load()");

                    OnPlay?.Invoke();
                    player.Play();
                }
                else
                {
                    OnPause?.Invoke();
                    player.Pause();
                }
            });

            // シーク スライダー値変更
            uiSeekSlider.onValueChanged.RemoveAllListeners();
            uiSeekSlider.onValueChanged.AddListener((v) => {
                if(EventSystem.current.currentSelectedGameObject == uiSeekSlider.gameObject && isSeeking)
                {
                    float time = v * (float)player.Info.GetDuration();
                    OnSeek?.Invoke(time);
                    player.Control.Seek(time);
                }
            });

            seekEventTrg.triggers.Clear();

            // シーク ポインターダウンで一時停止
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => {
                isSeeking = true;

                float time = uiSeekSlider.value * (float)player.Info.GetDuration();
                OnSeekStart?.Invoke(time);
                player.Pause();
            });
            seekEventTrg.triggers.Add(pointerDown);

            // シーク ポインターアップでシーク
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => {
                isSeeking = false;

                float time = uiSeekSlider.value * (float)player.Info.GetDuration();
                OnSeekEnd?.Invoke(time);
                player.Control.Seek(time);
            });
            seekEventTrg.triggers.Add(pointerUp);
        }

        public bool Load(string moviePathOrUrl, MediaPathType pathType = MediaPathType.AbsolutePathOrURL, bool autoPlay = false)
        {
            // 動画の読み込み
            movieLoaded = player.OpenMedia(pathType, moviePathOrUrl, autoPlay);

            return movieLoaded;
        }
#endif
    }
}
