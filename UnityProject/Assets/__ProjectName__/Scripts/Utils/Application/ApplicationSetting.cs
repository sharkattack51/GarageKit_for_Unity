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

public class ApplicationSetting : MonoBehaviour
{
	//singleton
	private static ApplicationSetting instance;
	public static ApplicationSetting Instance { get{ return instance; } }
	
	private XmlDocument xml;
	private string xmlUtf8Str;
	private Dictionary<string, string> dataDict;
	
	private bool isValid = false;
	public bool IsValid { get{ return isValid; } }
	
	//xmlファイル名を入力する(xmlの場所はexeと同一ディレクトリ,utf-8エンコード)
	public string xmlUrl = "ApplicationSetting.xml";
	
	//読み込み後展開されるアクセス可能なハッシュ
	public Dictionary<string, string> Data { get{ return dataDict; } }
	
	
	void Awake()
	{
		//データ取得用のインスタンス
		instance = this;
		
		//読み込み開始
		LoadXML();
	}
	
	
	/*
	 * 読み込み
	 */
	
	public void LoadXML()
	{
		try
		{
			xml = new XmlDocument();
			dataDict = new Dictionary<string, string>();
			
			//xml読み込み
			xml.Load(Path.GetFullPath("./") + xmlUrl);
			xmlUtf8Str = File.ReadAllText(Path.GetFullPath("./") + xmlUrl, Encoding.UTF8);
			
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
		//ノードリストを取得
		XmlNodeList nodeLlist = xml.GetElementsByTagName("item");
		
		//ハッシュに展開
		foreach(XmlNode node in nodeLlist)
		{
			XmlElement element = node as XmlElement;
			string paramName = element.GetAttribute("name");
			string paramValue = element.GetAttribute("value");
			
			dataDict.Add(paramName, paramValue);
		}
	}
	
	
	/*
	 * 保存
	 */
	
	public void SaveXML()
	{
		if(isValid)
		{
			//設定値を更新したxmlを作成
			string xmlStr = BuildXML(xmlUtf8Str);
			
			//書き込み
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
}
