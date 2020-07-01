using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * Cinema4Dのキーフレームjsonデータをアニメーションカーブに変換する
 */
namespace GarageKit
{
    [Serializable]
    public class AnimationTrack
    {
        // C4Dでのアニメ―ショントラックタイプ
        public enum TRACK_TYPE
        {
            POS_X = 0,
            POS_Y,
            POS_Z,
            ROT_X,
            ROT_Y,
            ROT_Z
        };
        public TRACK_TYPE type = TRACK_TYPE.POS_X;

        // キーフレームから変換されるアニメカーブ
        public AnimationCurve curve;


        public AnimationTrack()
        {

        }


        // C4Dでのトラック名の取得
        public string GetTrackName()
        {
            string name = "";
            switch(this.type)
            {
                case TRACK_TYPE.POS_X: name = "Position . X"; break;
                case TRACK_TYPE.POS_Y: name = "Position . Y"; break;
                case TRACK_TYPE.POS_Z: name = "Position . Z"; break;
                case TRACK_TYPE.ROT_X: name = "Rotation . P"; break;
                case TRACK_TYPE.ROT_Y: name = "Rotation . H"; break;
                case TRACK_TYPE.ROT_Z: name = "Rotation . B"; break;
                default: break;
            }

            return name;
        }
    }

    public class AnimDataApply : MonoBehaviour
    {
        // 参照ファイル(json)
        public string filePath = "animdata.txt";

        // アニメーションデータ反映トラック
        public AnimationTrack[] animTracks;

        private Dictionary<string, List<float>> data;
        public Dictionary<string, List<float>> Data { get{ return data; } }


        void Awake()
        {
            // ファイルの読み込み
            data = JsonUtility.FromJson<Dictionary<string, List<float>>>(File.ReadAllText(filePath));
        }

        void Start()
        {
            // アニメーションカーブを初期化
            InitAnimCurve();
        }

        void Update()
        {

        }


        // アニメーションカーブを初期化
        private void InitAnimCurve()
        {
            foreach(AnimationTrack track in animTracks)
            {
                if(data.ContainsKey(track.GetTrackName()))
                {
                    List<float> keyframes = new List<float>();
                    
                    //C4Dの座標系を変換してキーフレームを取得
                    foreach(float keyframe in data[track.GetTrackName()])
                        keyframes.Add(ConvertC4DSpace(track.type, keyframe));
                    
                    //アニメーションカーブに反映
                    track.curve = new AnimationCurve();
                    for(int i = 0; i < keyframes.Count; i++)
                        track.curve.AddKey((float)i / (float)keyframes.Count, keyframes[i]);
                }
            }
        }

        // カーブを選択取得
        public AnimationCurve GetCurve(AnimationTrack.TRACK_TYPE type)
        {
            AnimationCurve curve = null;
            AnimationTrack track = Array.Find(animTracks, t => (t.type == type));

            if(track != null)
                curve = track.curve;

            return curve;
        }

        // キーフレーム最大値を取得
        public int GetTotalFrame()
        {
            int totalFrame = 0;
            foreach(AnimationTrack track in animTracks)
            {
                if(totalFrame < track.curve.keys.Length)
                    totalFrame = track.curve.keys.Length;
            }
            return totalFrame;
        }

        // C4Dの座標系を変換
        private float ConvertC4DSpace(AnimationTrack.TRACK_TYPE type, float keyValue)
        {
            switch(type)
            {
                case AnimationTrack.TRACK_TYPE.POS_X: keyValue = keyValue / 100.0f * -1.0f; break;
                case AnimationTrack.TRACK_TYPE.POS_Y: keyValue = keyValue / 100.0f; break;
                case AnimationTrack.TRACK_TYPE.POS_Z: keyValue = keyValue / 100.0f * -1.0f; break;
                case AnimationTrack.TRACK_TYPE.ROT_X: keyValue = keyValue * Mathf.Rad2Deg * -1.0f; break;
                case AnimationTrack.TRACK_TYPE.ROT_Y: keyValue = (keyValue < 0.0f) ? 180.0f - (keyValue * Mathf.Rad2Deg) : (keyValue * Mathf.Rad2Deg) + 180.0f; break;
                case AnimationTrack.TRACK_TYPE.ROT_Z: keyValue = keyValue * Mathf.Rad2Deg * -1.0f; break;
                default: break;
            }

            return keyValue;
        }
    }
}
