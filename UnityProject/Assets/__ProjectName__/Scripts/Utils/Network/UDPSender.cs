using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

/*
 * UDP送信を行うクラス
 */

public class UDPSender : MonoBehaviour
{
	//送信先アドレス&ポート
	public string address = "localhost";
	public int port = 8000;
	
	
	void Awake()
	{
		
	}
	
	void Start()
	{
		
	}
	
	
	//データの送信
	public void Send(string dataStr)
	{
		//ソケットの設定
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
		IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
		
		//byte配列に変換
		byte[] data = Encoding.UTF8.GetBytes(dataStr);
		
		//送信
		try
		{
			socket.SendTo(data, 0, data.Length, SocketFlags.None, ipEndPoint);
		}
		catch(Exception e)
		{
			Debug.LogWarning("UDPSender :: " + e.Message);
			DebugConsole.Log("UDPSender :: " + e.Message);
		}
	}
}
