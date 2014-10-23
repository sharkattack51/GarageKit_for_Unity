using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

/*
 * シリアル通信を行うクラス
 */

public class SerialPortController : MonoBehaviour
{
	//singleton
	private static SerialPortController instance;
	public static SerialPortController Instance { get{ return instance; } }
	
	/// <summary>
	/// シリアルポートのインスタンス
	/// </summary>
	private SerialPort port = null;
	
	/// <summary>
	/// シリアルポート設定
	/// </summary>
	public string portName = "COM1";
	public int baudRate = 9600;
	public Parity parity = Parity.None;
	public int dataBits = 8;
	public StopBits stopBits = StopBits.One;
	public Encoding encoding = Encoding.ASCII;
	public NewLineCode newLineCode = NewLineCode.LF;
	public Handshake handShake = Handshake.None;
	public bool dtrEnable = false;
	public bool rtsEnable = false;
	public int readTimeout = 50;
	public int writeTimeout = 50;
	
	/// <summary>
	/// 一度に受け取るコマンドの文字バッファサイズ　固定長
	/// </summary>
	const int STR_LENGTH = 1024;
	
	/// <summary>
	/// 改行コード設定
	/// </summary>
	public enum NewLineCode
	{
		LF = 0,
		CR,
		CRLF,
	}
	
	/// <summary>
	/// コマンドを受信したときのデリゲート
	/// </summary>
	public class SerialCommandReceiveArgs : EventArgs
	{
	    public string command;
	}
	public delegate void ReceiveCommand(SerialCommandReceiveArgs e);
	public event ReceiveCommand OnReceive;
	private void InvokeReceive(SerialCommandReceiveArgs e)
	{
		if(OnReceive != null)
			OnReceive(e);
	}
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		portName = ApplicationSetting.Instance.Data["SerialPortName"];
		
		//ポートを開く
        if(port == null)
		{
			port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
	        port.Encoding = encoding;
			switch(newLineCode)
			{
				case NewLineCode.LF: port.NewLine = "\n"; break;
				case NewLineCode.CR: port.NewLine = "\r"; break;
				case NewLineCode.CRLF: port.NewLine = "\r\n"; break;
			}
	        port.Handshake = handShake;
	        port.DtrEnable = dtrEnable;
	        port.RtsEnable = rtsEnable;
	        port.ReadTimeout = readTimeout;
			port.WriteTimeout = writeTimeout;
			
			try
			{
				if(!port.IsOpen)
				{
					port.Open();
					
					//データ受信ルーチンの開始
					StartCoroutine(ReadData());
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message); 
			}
		}
	}
	
	void OnApplicationQuit()
	{
		//ポートを閉じる
		if(port != null)
		{
			port.Close();
			port.Dispose();
		}
	}
	
	
	/// <summary>
    /// データ受信
    /// </summary>
	private IEnumerator ReadData()
	{
		while(port.IsOpen)
		{
			string dataStr = "";
			
			try
			{
				byte data = (byte)port.ReadByte();
				while(data != 255)
				{
					dataStr += (char)data;
					
					//次データの読み取り
					data = (byte)port.ReadByte();
				}
			}
			catch(Exception)
			{
			
			}
			
			if(dataStr != "")
			{
				//指定された関数を実行
				SerialCommandReceiveArgs ce = new SerialCommandReceiveArgs();
	            ce.command = dataStr;

            	InvokeReceive(ce);
			}
			
			yield return null;
		}
	}
	
	/// <summary>
	/// コマンド文字列を送信する
	/// </summary>
	public void SendCommand(string str)
	{
		if(port != null && port.IsOpen)
		{
	        try
	        {
	        	port.WriteLine(str);
	        }
	        catch(Exception e)
	        {
				Debug.LogError(e.Message);
	        }
		}
	}
	
	/// <summary>
	/// コマンド文字列をバイトに変換して送信する
	/// </summary>
	public void SendCommandByte(string str)
	{
		List<byte> bytes = new List<byte>();
		for(int i = 0; i < str.Length; i++)
            bytes.Add((byte)str[i]);
		
		SendByte(bytes.ToArray());
	}
	
	/// <summary>
    /// スペース区切りの数字のコマンド配列を16進数に変換してデータ送信
    /// </summary>
    public void SendCommandArrayHexByte(string str)
    {
		string[] hexs = str.Split(new string[] { " " }, StringSplitOptions.None);

        List<byte> bytes = new List<byte>();
        foreach (string hex in hexs)
            bytes.Add((byte)Convert.ToInt32(hex, 16));

        SendByte(bytes.ToArray());
    }

    /// <summary>
    /// バイトデータを送信する
    /// </summary>
    public void SendByte(byte[] strBytes)
    {
		if(port != null && port.IsOpen)
		{
	        try
	        {
				port.Write(strBytes, 0, strBytes.Length);
	        }
	        catch(Exception e)
	        {
				Debug.LogError(e.Message);
	        }
		}
    }
}