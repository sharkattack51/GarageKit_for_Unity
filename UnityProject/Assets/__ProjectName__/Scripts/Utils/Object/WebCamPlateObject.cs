using UnityEngine;
using System.Collections;
using System;

/*
 * WebCameraの映像をテクスチャに設定する
 * 使用する前にWebカメラのドライバをインストールすること!
 */

public class WebCamPlateObject : MonoBehaviour
{
	//デバイス設定
	public string deviceName = "";
	public int deviceIndex = 0;
	public int requestedWidth = 1280;
	public int requestedHeight = 720;
	public int anisoLevel = 9;
	public FilterMode filteMode = FilterMode.Bilinear;
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	public bool isAutoAspect = true;
	public bool isMirror = false;
	public OBJECTAXIS_Y vertical = OBJECTAXIS_Y.Z;
	
	private WebCamDevice[] devices;
	private WebCamTexture webCamTexture;
	public WebCamTexture GetWebCamTexture() { return webCamTexture; }
	
	private Vector3 defaultAspect;
	
	public enum OBJECTAXIS_Y
	{
		Y = 0,
		Z
	}
	
	
	void Awake()
	{
	
	}
	
	void Start()
	{
		//Webカメラを開く
		try
		{
			devices = WebCamTexture.devices;
			
			if(deviceName != null)
				webCamTexture = new WebCamTexture(deviceName, requestedWidth, requestedHeight);
			else
				webCamTexture = new WebCamTexture(devices[deviceIndex].name, requestedWidth, requestedHeight);
			
			webCamTexture.anisoLevel = anisoLevel;
			webCamTexture.filterMode = filteMode;
			webCamTexture.wrapMode = wrapMode;
			webCamTexture.mipMapBias = -1.0f;
			
			Debug.Log("usb camera opened : " + webCamTexture.deviceName + " : " + webCamTexture.requestedWidth.ToString() + " : " + webCamTexture.requestedHeight.ToString());
		}
		catch(Exception e)
		{
			Debug.Log("usb camera open error : " + e.Message);
			return;
		}
		
		//アスペクト調整用の初期値を保存
		defaultAspect = this.gameObject.transform.localScale;
		
		//アスペクトを自動調整
		if(isAutoAspect)
		{
			float textureAspect = (float)requestedWidth / (float)requestedHeight;
			float objectAspect = 1.0f;
			if(vertical == OBJECTAXIS_Y.Z)
				objectAspect = defaultAspect.x / defaultAspect.z;
			else
				objectAspect = defaultAspect.x / defaultAspect.y;
			float scaleRatio = textureAspect / objectAspect;
			
			this.gameObject.transform.localScale = new Vector3(defaultAspect.x * scaleRatio, defaultAspect.y, defaultAspect.z);
		}
		
		//反転設定
		if(isMirror)
		{
			this.gameObject.transform.localScale = new Vector3(
				-this.gameObject.transform.localScale.x,
				this.gameObject.transform.localScale.y,
				this.gameObject.transform.localScale.z);
		}
		
		//テクスチャを適用
		this.renderer.material.mainTexture = webCamTexture;
		
		//映像の更新を開始
		webCamTexture.Play();
	}
	
	/// <summary>
	/// 現在使用しているWebカメラ映像を取得
	/// </summary>
	public WebCamTexture GetTexture()
	{
		if(webCamTexture != null)
			return webCamTexture;
		else
			return null;
	}
}
