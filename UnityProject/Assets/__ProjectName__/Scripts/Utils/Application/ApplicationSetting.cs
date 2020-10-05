using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

/*
 * アプリケーション設定ファイルを読み込む
 */
namespace GarageKit
{
    public class ApplicationSetting : MonoBehaviour
    {
        // singleton
        private static ApplicationSetting instance;
        public static ApplicationSetting Instance { get{ return instance; } }

        public enum XML_FROM
        {
            STREAMING_ASSETS = 0,
            PROJECT_DIRECTORY,
            CURRENT_WORK_DIRECTORY
        }
        public XML_FROM loadFrom = XML_FROM.STREAMING_ASSETS;
        public string xmlFile = "ApplicationSetting.xml";

        private XmlDocument xml;
        private string xmlUtf8Str;

        private Dictionary<string, string> rawData;
        public Dictionary<string, string> RawData { get{ return rawData; } }

        private bool isValid = false;
        public bool IsValid { get{ return isValid; } }


        void Awake()
        {
            // データ取得用のインスタンス
            instance = this;

            // 読み込み開始
            LoadXML();
        }
        

#region Load
        public void LoadXML()
        {
            try
            {
                xml = new XmlDocument();
                rawData = new Dictionary<string, string>();

                // xml読み込み
                string path = "";
                switch(loadFrom)
                {
                    case XML_FROM.STREAMING_ASSETS:
                        path = path = Path.Combine(Application.streamingAssetsPath, xmlFile);
                        break;

                    case XML_FROM.PROJECT_DIRECTORY:
                        path = path = Path.Combine(Application.dataPath + "/..", xmlFile);
                        break;

                    case XML_FROM.CURRENT_WORK_DIRECTORY:
                        path = Path.GetFullPath("./") + xmlFile;
                        break;

                    default: break;
                }

                if(!File.Exists(path))
                    path = Path.GetFullPath("./") + xmlFile;

                xml.Load(path);
                xmlUtf8Str = File.ReadAllText(path, Encoding.UTF8);

                // xmlをパース
                ParseXML();

                isValid = true;
            }
            catch(Exception err)
            {
                Debug.Log(err.Message);

                isValid = false;
            }
        }

        private void ParseXML()
        {
            // ノードリストを取得
            XmlNodeList nodeLlist = xml.GetElementsByTagName("item");

            // ハッシュに展開
            foreach(XmlNode node in nodeLlist)
            {
                XmlElement element = node as XmlElement;
                string paramName = element.GetAttribute("name");
                string paramValue = element.GetAttribute("value");

                rawData.Add(paramName, paramValue);
            }
        }
#endregion

#region 型チェックしての取得
        public string GetString(string key, string defaultValue = "")
        {
            if(rawData.ContainsKey(key) && rawData.ContainsKey(key))
                return rawData[key];
            else
                return defaultValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            bool result;
            if(rawData.ContainsKey(key) && bool.TryParse(rawData[key], out result))
                return result;
            else
                return defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            int result;
            if(rawData.ContainsKey(key) && int.TryParse(rawData[key], out result))
                return result;
            else
                return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            float result;
            if(rawData.ContainsKey(key) && float.TryParse(rawData[key], out result))
                return result;
            else
                return defaultValue;
        }

        public string[] GetStringArray(string key, string separator = ",")
        {
            if(rawData.ContainsKey(key) && rawData.ContainsKey(key))
                return rawData[key].Split(new string[]{ separator }, StringSplitOptions.None);
            else
                return new string[0];
        }

        public int[] GetIntArray(string key, string separator = ",", int defaultValue = 0)
        {
            if(rawData.ContainsKey(key) && rawData.ContainsKey(key))
            {
                string[] strs = rawData[key].Split(new string[]{ separator }, StringSplitOptions.None);
                List<int> list = new List<int>();
                foreach(string str in strs)
                {
                    int result;
                    if(int.TryParse(str, out result))
                        list.Add(result);
                    else
                        list.Add(defaultValue);
                }
                return list.ToArray();
            }
            else
                return new int[0];
        }

        public float[] GetFloatArray(string key, string separator = ",", float defaultValue = 0.0f)
        {
            if(rawData.ContainsKey(key) && rawData.ContainsKey(key))
            {
                string[] strs = rawData[key].Split(new string[]{ separator }, StringSplitOptions.None);
                List<float> list = new List<float>();
                foreach(string str in strs)
                {
                    float result;
                    if(float.TryParse(str, out result))
                        list.Add(result);
                    else
                        list.Add(defaultValue);
                }
                return list.ToArray();
            }
            else
                return new float[0];
        }

        public bool[] GetBoolArray(string key, string separator = ",", bool defaultValue = false)
        {
            if(rawData.ContainsKey(key) && rawData.ContainsKey(key))
            {
                string[] strs = rawData[key].Split(new string[]{ separator }, StringSplitOptions.None);
                List<bool> list = new List<bool>();
                foreach(string str in strs)
                {
                    bool result;
                    if(bool.TryParse(str, out result))
                        list.Add(result);
                    else
                        list.Add(defaultValue);
                }
                return list.ToArray();
            }
            else
                return new bool[0];
        }

        public DateTime GetFormattedDateTime(string key, string format = "HH:mm:ss")
        {
            DateTime result;
            if(rawData.ContainsKey(key) && DateTime.TryParseExact(rawData[key], format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                return result;
            else
                return DateTime.Now;
        }

        public DateTime GetDateTime(string key)
        {
            DateTime result;
            if(rawData.ContainsKey(key) && DateTime.TryParse(rawData[key], out result))
                return result;
            else
                return DateTime.Now;
        }
#endregion
    }
}
