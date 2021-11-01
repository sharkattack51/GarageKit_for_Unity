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
        public bool ignorePlayerControll = false;

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


        void Awake()
        {

        }

        void Start()
        {
            if(setupOnStart)
                Setup();
        }

        void Update()
        {
            if(player != null)
            {
                // ボタンスプライトの切り替え
                if(!player.Control.IsSeeking())
                {
                    if(!player.Control.IsPlaying())
                        uiPlayPauseBtn.image.sprite = playSprite;
                    else
                        uiPlayPauseBtn.image.sprite = pauseSprite;
                }

                // シークバーの更新
                if(movieLoaded && player.Control.CanPlay() && !player.Control.IsPaused())
                    uiSeekSlider.value = player.Control.GetCurrentTimeMs() / player.Info.GetDurationMs();

                // 経過時間の更新
                uiElapsedTxt.text = string.Format("{0:D2}:{1:D2} / {2:D2}:{3:D2}",
                    (int)(player.Control.GetCurrentTimeMs() / 1000.0f / 60.0f),
                    (int)(player.Control.GetCurrentTimeMs() / 1000.0f % 60.0f),
                    (int)(player.Info.GetDurationMs() / 1000.0f / 60.0f),
                    (int)(player.Info.GetDurationMs() / 1000.0f % 60.0f));
            }
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
                player.CloseVideo();
            }
        }

        public void Setup()
        {
            uiMovie._mediaPlayer = player;

            // 再生/一時停止ボタン
            uiPlayPauseBtn.onClick.AddListener(() => {
                if(!player.Control.IsPlaying())
                {
                    OnPlay?.Invoke();

                    if(!ignorePlayerControll)
                        player.Play();
                }
                else
                {
                    OnPause?.Invoke();

                    if(!ignorePlayerControll)
                        player.Pause();
                }
            });

            // シーク スライダー値変更
            uiSeekSlider.onValueChanged.RemoveAllListeners();
            uiSeekSlider.onValueChanged.AddListener((v) => {
                if(EventSystem.current.currentSelectedGameObject == uiSeekSlider.gameObject && isSeeking)
                {
                    float timeMs = v * player.Info.GetDurationMs();
                    OnSeek?.Invoke(timeMs);

                    if(!ignorePlayerControll)
                        player.Control.Seek(timeMs);
                }
            });

            seekEventTrg.triggers.Clear();

            // シーク ポインターダウンで一時停止
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => {
                isSeeking = true;

                float timeMs = uiSeekSlider.value * player.Info.GetDurationMs();
                OnSeekStart?.Invoke(timeMs);

                if(!ignorePlayerControll)
                    player.Pause();
            });
            seekEventTrg.triggers.Add(pointerDown);

            // シーク ポインターアップでシーク
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => {
                isSeeking = false;

                float timeMs = uiSeekSlider.value * player.Info.GetDurationMs();
                OnSeekEnd?.Invoke(timeMs);

                if(!ignorePlayerControll)
                    player.Control.Seek(timeMs);
            });
            seekEventTrg.triggers.Add(pointerUp);
        }

        public bool Load(string moviePathOrUrl, bool autoPlay)
        {
            // 動画の読み込み
            movieLoaded = player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, moviePathOrUrl, autoPlay);

            return movieLoaded;
        }
#endif
    }
}
