using UnityEngine;
using System.Collections;
using System.IO;

/*
 * コンテンツデータのダウンロードを管理する
 */

public class ContentsDownLoader : MonoBehaviour
{
	//singleton
	private static ContentsDownLoader instance;
	public static ContentsDownLoader Instance
	{
		get
		{
			if(instance == null)
			{
				GameObject go = new GameObject("ContentsDownLoader");
				instance = go.AddComponent<ContentsDownLoader>();
			}
			
			return instance;
		}
	}
	
	//読み込み完了イベント
	public delegate void OnLoadCompleteDelegate();
	public event OnLoadCompleteDelegate OnLoadComplete;
	private void InvokeOnLoadComplete()
	{
		if(OnLoadComplete != null)
			OnLoadComplete();
	}
	
	//読み込み失敗イベント
	public delegate void OnLoadErrorDelegate();
	public event OnLoadErrorDelegate OnLoadError;
	private void InvokeOnLoadError()
	{
		if(OnLoadError != null)
			OnLoadError();
	}
	
	//最新の読み込みデータパス
	private string latestContentsPath = "";
	public string LatestContentsPath { get{ return latestContentsPath; } }
	
	//読み込み中確認
	private bool isLoading = false;
	public bool IsLoading { get{ return isLoading; } }
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
	
	}
	
	void Update()
	{
	
	}
	
	
	/// <summary>
	/// 読み込みを開始
	/// </summary>
	public void Load(string url, bool forceReload = false)
	{
		isLoading = true;
		
		//TODO:インジケーターの表示
		
		string filePath = Path.Combine(contentsDataPath(), Path.GetFileName(url));
		if(forceReload)
		{
			if(File.Exists(filePath))
				File.Delete(filePath);
			
			StartCoroutine(LoadCoroutine(url));
		}
		else
		{
			if(!File.Exists(filePath))
				StartCoroutine(LoadCoroutine(url));
			else
			{
				//最新パスの設定
				latestContentsPath = filePath;
				
				//TODO:インジケーターの非表示
				
				InvokeOnLoadComplete();
				isLoading = false;
			}
		}
	}
	
	private IEnumerator LoadCoroutine(string url)
	{
		WWW www = new WWW(url);
		
		yield return www;
		
		if(www.error != "")
		{
			string filePath = Path.Combine(contentsDataPath(), Path.GetFileName(url));
			
			//ファイルを保存
			FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write); 
			foreach(byte b in www.bytes)
				fileStream.WriteByte(b);
			fileStream.Close();
			
			//最新パスの設定
			latestContentsPath = filePath;
			
			//読み込み成功
			InvokeOnLoadComplete();
		}
		else
		{
			//読み込み失敗
			InvokeOnLoadError();
		}
		
		//TODO:インジケーターの非表示
		
		isLoading = false;
	}
	
	/// <summary>
	/// コンテンツの保存先を取得
	/// </summary>
	public static string ContentsDataPath { get{ return contentsDataPath(); } }
	private static string contentsDataPath()
	{
		string path;
	
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
			path = path.Substring(0, path.LastIndexOf('/')); 
			path += "/Documents/ContentsData/";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			path = Path.Combine(Application.persistentDataPath, "ContentsData");
		}
		else
		{
			path = Path.Combine(Application.dataPath, "../ContentsData");
		}
		
		if(!Directory.Exists(path))
			Directory.CreateDirectory(path);
		
		return path;
	}
}
