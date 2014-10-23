using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

/*
 * UDP受信を行いデリゲートを実行するクラス
 */

public class UDPReciever : MonoBehaviour
{
	//受信ポート
	public int port = 8001;
	
	//受信データ
	private string recievedDataStr = "";
	public string RecievedDataStr { get{ return recievedDataStr; } }
	
	//スレッド処理でのデータ受信フラグ
	private volatile bool isDataRecieved = false;
	
	//データ受信イベント
	public delegate void OnReceivedDelegate(GameObject senderObject, string receivedStr);
	public event OnReceivedDelegate OnReceived;
	private void InvokeOnReceived(string receivedStr)
	{
		if(OnReceived != null)
			OnReceived(this.gameObject, receivedStr);
	}
	
	private UdpClient udpClient = null;
	private Thread receiveThread = null;
	private volatile bool threadAvairable = true;
	
	
	void Awake()
	{
	
	}
	
	void Start()
	{
		//受信スレッドを開始
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}
	
	void Update()
	{
		//受信イベントをメインスレッドで実行
		if(isDataRecieved)
		{
			InvokeOnReceived(recievedDataStr);
			isDataRecieved = false;
		}
	}
	
	void OnApplicationQuit()
	{
		threadAvairable = false;
		
		//受信スレッドを停止
		if(receiveThread != null)
		{
			receiveThread.Join();
			receiveThread = null;
		}
		
		//udpクライアントを閉じる
		udpClient.Close();
		udpClient = null;
	}
	
	
	//受信スレッド
	private void ReceiveData()
	{
		//udpクライアント設定
		udpClient = new UdpClient(port);

		while(threadAvairable)
		{
			try
			{
				if(udpClient.Available > 0)
				{
					IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
					byte[] data = udpClient.Receive(ref ipEndPoint);
					
					recievedDataStr = Encoding.UTF8.GetString(data);
					isDataRecieved = true;
				}
			}
			catch(Exception err)
			{
				Debug.LogError(err.Message);
				DebugConsole.LogError(err.Message);
				break;
			}
		}
	}
}