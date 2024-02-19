//#define USE_TIMELINE
//#define USE_AVPRO_VIDEO_TIMELINE

#if USE_TIMELINE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

#if USE_AVPRO_VIDEO_TIMELINE
using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Playables;
#endif

using AudioTrack = UnityEngine.Timeline.AudioTrack;

namespace GarageKit
{
    public class TimelineUtil
    {
        private static PlayableDirector managedDirector = null;
        private static List<TrackAsset> managedTracks = new List<TrackAsset>();
        public static void GavageManagedTracks()
        {
            if(managedDirector != null)
            {
                foreach(TrackAsset track in managedTracks)
                {
                    if(track != null)
                        DeleteTrack(managedDirector, track);
                }
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static TrackAsset AddTrack<T>(PlayableDirector playableDirector, string trackName, UnityEngine.Object bindObj, TrackAsset parent = null) where T : TrackAsset
        {
            TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
            foreach(TrackAsset track in timelineAsset.GetOutputTracks())
            {
                if(track.name == trackName)
                    timelineAsset.DeleteTrack(track);
            }

            TrackAsset newTrack = timelineAsset.CreateTrack(typeof(T), parent, trackName);
            playableDirector.SetGenericBinding(newTrack, bindObj);
            playableDirector.RebuildGraph();

            if(managedDirector == null)
                managedDirector = playableDirector;
            managedTracks.Add(newTrack);
            Debug.LogWarning("don't forget to delete the added track with TimelineUtil.GavageManagedTracks()");

            return newTrack;
        }

        public static void DeleteTrack(PlayableDirector playableDirector, TrackAsset track)
        {
            playableDirector.ClearGenericBinding(track);

            TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
            timelineAsset.DeleteTrack(track);
        }

        public static void DeleteAllTrack(PlayableDirector playableDirector)
        {
            List<TrackAsset> tracks = GetAllTracks(playableDirector);
            foreach(TrackAsset track in tracks)
                DeleteTrack(playableDirector, track);
        }

        public static void DeleteTrackByName(PlayableDirector playableDirector, string trackName)
        {
            TrackAsset track = GetTrackByName(playableDirector, trackName);
            DeleteTrack(playableDirector, track);
        }

        public static TrackAsset GetTrackByName(PlayableDirector playableDirector, string trackName)
        {
            TrackAsset track = null;

            TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
            if(timeline != null)
            {
                IEnumerable<TrackAsset> tracks = timeline.GetOutputTracks();
                track = tracks.FirstOrDefault(e => e.name == trackName);
            }

            return track;
        }

        public static List<TrackAsset> GetAllTracks(PlayableDirector playableDirector)
        {
            List<TrackAsset> tracks = new List<TrackAsset>();

            TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
            if(timeline != null)
            {
                IEnumerable<TrackAsset> outputTracks = timeline.GetOutputTracks();
                foreach(TrackAsset track in outputTracks)
                    tracks.Add(track);
            }

            return tracks;
        }

        public static void ClearTrackByName(PlayableDirector playableDirector, string trackName)
        {
            TrackAsset track = GetTrackByName(playableDirector, trackName);
            if(track != null && track.hasClips)
            {
                IEnumerable<TimelineClip> clips = track.GetClips();
                foreach(TimelineClip clip in clips)
                    track.DeleteClip(clip);
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            playableDirector.RebuildGraph();
        }

        public static void ClearAllTracks(PlayableDirector playableDirector)
        {
            List<TrackAsset> tracks = GetAllTracks(playableDirector);
            foreach(TrackAsset track in tracks)
            {
                if(track != null)
                {
                    SignalTrack signalTrack = track as SignalTrack;
                    if(signalTrack != null)
                    {
                        IEnumerable<IMarker> markers = signalTrack.GetMarkers();
                        foreach(IMarker marker in markers)
                            signalTrack.DeleteMarker(marker);
                    }
                    else if(track.hasClips)
                    {
                        IEnumerable<TimelineClip> clips = track.GetClips();
                        foreach(TimelineClip clip in clips)
                            track.DeleteClip(clip);
                    }
                }
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            playableDirector.RebuildGraph();
        }

        public static void SetTrack<T>(PlayableDirector playableDirector, string trackName, double start, double end) where T : TrackAsset
        {
            T track = GetTrackByName(playableDirector, trackName) as T;
            if(track != null)
            {
                TimelineClip clip = track.CreateDefaultClip();
                clip.start = start;
                if(start < end)
                    clip.duration = end - start;

                playableDirector.RebuildGraph();
            }
        }

        public static void SetAudioTrack(PlayableDirector playableDirector, string trackName, AudioClip audioClip, double start, double? end = null,
            bool loop = false, double easeInDuration = 0.0, double easeOutDuration = 0.0)
        {
            AudioTrack track = GetTrackByName(playableDirector, trackName) as AudioTrack;
            if(track != null)
            {
                TimelineClip clip = track.CreateClip(audioClip);
                clip.start = start;
                clip.easeInDuration = easeInDuration;
                clip.easeOutDuration = easeOutDuration;

                if(end != null && start < end)
                    clip.duration = (double)end - start;

                AudioPlayableAsset audioAsset = clip.asset as AudioPlayableAsset;
                audioAsset.loop = loop;

                playableDirector.RebuildGraph();
            }
        }

        public static void SetRawImageTrack(PlayableDirector playableDirector, string trackName, Texture2D tex, double start, double end)
        {
            ActivationTrack track = GetTrackByName(playableDirector, trackName) as ActivationTrack;
            if(track != null)
            {
                TimelineClip clip = track.CreateDefaultClip();
                clip.start = start;
                if(start < end)
                    clip.duration = end - start;

                playableDirector.RebuildGraph();

                GameObject bindGo = playableDirector.GetGenericBinding(track) as GameObject;
                if(bindGo != null)
                {
                    RawImage rawImg = bindGo.GetComponent<RawImage>();
                    if(rawImg != null)
                        rawImg.texture = tex;
                }
            }
        }

#if USE_AVPRO_VIDEO_TIMELINE
        public static void SetMediaPlayerTrack(PlayableDirector playableDirector, string trackName, MediaPathType mediaPathType, string moviePath, double start, double? end = null)
        {
            MediaPlayerControlTrack track = GetTrackByName(playableDirector, trackName) as MediaPlayerControlTrack;
            if(track != null)
            {
                MediaPlayer bindMp = playableDirector.GetGenericBinding(track) as MediaPlayer;
                if(bindMp != null)
                {
                    bindMp.CloseMedia();
                    bindMp.OpenMedia(mediaPathType, moviePath, false);

                    if(end == null)
                        end = bindMp.Info.GetDuration();
                }
            }

            SetMediaPlayerTrack(playableDirector, trackName, start, end);
        }

        public static void SetMediaPlayerTrack(PlayableDirector playableDirector, string trackName, double start, double? end = null)
        {
            MediaPlayerControlTrack track = GetTrackByName(playableDirector, trackName) as MediaPlayerControlTrack;
            if(track != null && start < end)
            {
                TimelineClip clip = track.CreateDefaultClip();
                clip.start = start;
                if(end != null && start < end)
                    clip.duration = (double)end - start;

                playableDirector.RebuildGraph();
            }
        }
#endif

        public static void SetActivationTrack(PlayableDirector playableDirector, string trackName, double start, double end)
        {
            ActivationTrack track = GetTrackByName(playableDirector, trackName) as ActivationTrack;
            if(track != null)
            {
                TimelineClip clip = track.CreateDefaultClip();
                clip.start = start;
                if(start < end)
                    clip.duration = end - start;

                playableDirector.RebuildGraph();
            }
        }

        public static void SetAnimationTrack(PlayableDirector playableDirector, string trackName, AnimationClip animClip, double start, double end)
        {
            UnityEngine.Timeline.AnimationTrack track = GetTrackByName(playableDirector, trackName) as UnityEngine.Timeline.AnimationTrack;
            if(track != null)
            {
                TimelineClip clip = track.CreateDefaultClip();
                clip.start = start;
                if(start < end)
                    clip.duration = end - start;

                AnimationPlayableAsset animAsset = clip.asset as AnimationPlayableAsset;
                animAsset.clip = animClip;

                playableDirector.RebuildGraph();
            }
        }

        public static void SetSignalTrack(PlayableDirector playableDirector, string trackName, SignalAsset signal, double time, string emitterName = "")
        {
            SignalTrack track = GetTrackByName(playableDirector, trackName) as SignalTrack;
            if(track != null)
            {
                SignalEmitter emitter = track.CreateMarker<SignalEmitter>(time);
                emitter.name = emitterName;
                emitter.asset = signal;

                playableDirector.RebuildGraph();
            }
        }

        public static void SetSignalTrack(PlayableDirector playableDirector, SignalAsset signal, double time, string emitterName = "")
        {
            SetSignalTrack(playableDirector, "Signal Track", signal, time, emitterName);
        }

        // トラック内各ClipのIn/Out時間の取得
        public static List<Tuple<Double, Double>> GetClipsInOut(TrackAsset track)
        {
            List<Tuple<Double, Double>> clipsInOut = new List<Tuple<Double, Double>>();

            IEnumerable<TimelineClip> clips = track.GetClips();
            foreach(TimelineClip clip in clips)
                clipsInOut.Add(new Tuple<Double, Double>(clip.start, clip.end));

            return clipsInOut;
        }
    }
}
#endif
