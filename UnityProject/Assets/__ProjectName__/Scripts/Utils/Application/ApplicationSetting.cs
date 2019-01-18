using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

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
		
		private XmlDocument xml;
		private string xmlUtf8Str;
		private Dictionary<string, string> dataDict;
		
		private bool isValid = false;
		public bool IsValid { get{ return isValid; } }
		
		// xmlファイル名
		public string xmlUrl = "ApplicationSetting.xml";
		public bool xmlFromStreamingAssets = true;
		
		// 読み込み後展開されるアクセス可能なハッシュ
		public Dictionary<string, string> Data { get{ return dataDict; } }


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
				dataDict = new Dictionary<string, string>();
				
				//xml読み込み
				string path = "";
				if(xmlFromStreamingAssets)
				{
					if(File.Exists(Path.Combine(Application.streamingAssetsPath, xmlUrl)))
						path = Path.Combine(Application.streamingAssetsPath, xmlUrl);
					else
						path = Path.GetFullPath("./") + xmlUrl;
				}
				else
					path = Path.GetFullPath("./") + xmlUrl;

				xml.Load(path);
				xmlUtf8Str = File.ReadAllText(path, Encoding.UTF8);
				
				//xmlをパース
				ParseXML();
				
				isValid = true;
			}
			catch(Exception err)
			{
				Debug.Log(err.Message);
				
				isValid = false;
			}
			
			/*
			foreach(string key in dataDict.Keys)
				Debug.Log("ApplicationSettingData Name: " + key + "  Value: " + dataDict[key]);
			*/
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
				
				dataDict.Add(paramName, paramValue);
			}
		}

#endregion
		
#region Save

		public void SaveXML()
		{
			if(isValid)
			{
				// 設定値を更新したxmlを作成
				string xmlStr = BuildXML(xmlUtf8Str);
				
				// 書き込み
				Stream stream = new FileStream(Path.GetFullPath("./") + xmlUrl, FileMode.Create);
				StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
				writer.Write(xmlStr);
				writer.Close();
			}
		}
		
		private string BuildXML(string sourceStr)
		{
			string buildStr = sourceStr;
			foreach(string key in dataDict.Keys)
			{
				string preSplitter = key + "\" value=\"";
				string postSplitter = "\"";
				
				string[] preStr = buildStr.Split(new string[]{ preSplitter }, StringSplitOptions.None);
				string[] postStr = (preStr[1] as string).Split(new string[]{ postSplitter }, StringSplitOptions.None);
				
				buildStr = preStr[0] + preSplitter + dataDict[key];
				for(int i=1; i<postStr.Length; i++)
				{
					buildStr += postSplitter;
					buildStr += postStr[i];
				}
			}
			
			return buildStr;
		}

#endregion

#region 型チェックしての取得

		public string GetString(string key, string defaultValue = "")
		{
			if(dataDict.ContainsKey(key) && dataDict.ContainsKey(key))
				return dataDict[key];
			else
				return defaultValue;
		}

		public bool GetBool(string key, bool defaultValue = false)
		{
			bool result;
			if(dataDict.ContainsKey(key) && bool.TryParse(dataDict[key], out result))
				return result;
			else
				return defaultValue;
		}
		
		public int GetInt(string key, int defaultValue = 0)
		{
			int result;
			if(dataDict.ContainsKey(key) && int.TryParse(dataDict[key], out result))
				return result;
			else
				return defaultValue;
		}
		
		public float GetFloat(string key, float defaultValue = 0.0f)
		{
			float result;
			if(dataDict.ContainsKey(key) && float.TryParse(dataDict[key], out result))
				return result;
			else
				return defaultValue;
		}

		public string[] GetStringArray(string key, string separator = ",")
		{
			if(dataDict.ContainsKey(key) && dataDict.ContainsKey(key))
				return dataDict[key].Split(new string[]{ separator }, StringSplitOptions.None);
			else
				return new string[0];
		}

		public int[] GetIntArray(string key, string separator = ",", int defaultValue = 0)
		{
			if(dataDict.ContainsKey(key) && dataDict.ContainsKey(key))
			{
				string[] strs = dataDict[key].Split(new string[]{ separator }, StringSplitOptions.None);
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
			if(dataDict.ContainsKey(key) && dataDict.ContainsKey(key))
			{
				string[] strs = dataDict[key].Split(new string[]{ separator }, StringSplitOptions.None);
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
			if(dataDict.ContainsKey(key) && dataDict.ContainsKey(key))
			{
				string[] strs = dataDict[key].Split(new string[]{ separator }, StringSplitOptions.None);
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

#endregion
	}
}
