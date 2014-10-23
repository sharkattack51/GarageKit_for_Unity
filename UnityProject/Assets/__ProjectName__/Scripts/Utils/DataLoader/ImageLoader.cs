using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * ディレクトリを指定してまとめて画像を読み込む
 */

public class ImageLoader : MonoBehaviour
{
	//singleton
	private static ImageLoader instance;
	public static ImageLoader Instance { get{ return instance; } }
	
	//読み込み対象のフォルダ
	public string folderPath;
	public bool absolute = false;
	
	//自動読み込み開始
	public bool autoLoad = false;
	
	private List<string> files = new List<string>();
	
	//読み込み画像データリスト
	private Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
	public Dictionary<string, Texture2D> Images { get{ return images; } }
	
	//読み込み完了イベント
	public delegate void OnLoadCompleteDelegate();
	public event OnLoadCompleteDelegate OnLoadComplete;
	protected void InvokeOnLoadComplete()
	{
		if(OnLoadComplete != null)
			OnLoadComplete();
	}
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		//自動読み込み
		if(autoLoad)
			Load();
	}
	
	void Update()
	{
	
	}
	
	
#region 読み込み

	public void Load(string folderPath)
	{
		this.folderPath = folderPath;
		
		Load();
	}
	
	public void Load()
	{
		if(folderPath != "")
			StartCoroutine(LoadImages());
	}
	
	private IEnumerator LoadImages()
	{
		//相対パス設定
		if(!absolute)
			folderPath = Path.Combine(Application.dataPath, Path.Combine("..", folderPath));
		
		//ディレクトリ確認
		if(!Directory.Exists(folderPath))
		{
			Debug.Log("ImageLoader :: directory not found - " + folderPath);
			yield break;
		}
		
		//画像ファイルの取得
		foreach(string serchPattern in new string[]{"*.jpg", "*.jpeg", "*.png"})
			files.AddRange(Directory.GetFiles(folderPath, serchPattern));
		
#if UNITY_3_5
		
		WWW www = null;
		
		foreach(string file in files)
		{	
			www = new WWW("file://" + file);
			
			yield return www;
			
			if(www.texture != null)
			{
				Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.ARGB32, false);
				texture.name = Path.GetFileName(file);
				
				www.LoadImageIntoTexture(texture);
				
				//ハッシュテーブルに登録
				images.Add(texture.name, texture);
			}
			else
			{
				Debug.Log("ImageLoader :: load error [ " + www.error + " ] - " + file);
			}
		}
		
		www.Dispose();
#endif
		
#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		
		//日本語パスの読み込み対応
		foreach(string file in files)
		{
			byte[] readByte = File.ReadAllBytes(file);
			
			yield return readByte;
			
			if(readByte.Length > 0)
			{
				Texture2D texture = new Texture2D(2, 2);
				texture.name = Path.GetFileName(file);
				
				texture.LoadImage(readByte);
				texture.Apply();
				
				//ハッシュテーブルに登録
				images.Add(texture.name, texture);
			}
			else
			{
				Debug.Log("ImageLoader :: load error - " + file);
			}
		}
		
#endif
		
		//読み込み完了イベント
		InvokeOnLoadComplete();
	}
	
#endregion
}
