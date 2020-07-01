using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/*
 * UDP受信を行いデリゲートを実行するクラス
 */
namespace GarageKit
{
    public class UDPReceiver : MonoBehaviour
    {
        // 受信ポート
        public int port = 8001;

        public bool autoStart = true;

        // 受信データ
        private string receivedDataStr = "";
        public string ReceivedDataStr { get{ return receivedDataStr; } }
        private byte[] receivedDataBytes = new byte[0];
        public byte[] ReceivedDataBytes { get{ return receivedDataBytes; } }

        // スレッド処理でのデータ受信フラグ
        private volatile bool isDataReceived = false;

        // データ受信イベント
        public Action<string> OnReceived;
        public Action<byte[]> OnReceivedBytes;

        private UdpClient udpClient = null;
        private Thread receiveThread = null;
        private volatile bool threadAvairable = false;


        void Awake()
        {

        }

        void Start()
        {
            if(autoStart)
                Open();
        }

        void Update()
        {
            // 受信イベントをメインスレッドで実行
            if(isDataReceived)
            {
                if(OnReceived != null)
                    OnReceived(receivedDataStr);
                if(OnReceivedBytes != null)
                    OnReceivedBytes(receivedDataBytes);

                isDataReceived = false;
            }
        }

        void OnApplicationQuit()
        {
            Close();
        }


        public void Open()
        {
            // 受信スレッドを開始
            if(!threadAvairable)
            {
                receiveThread = new Thread(new ThreadStart(ReceiveData));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
        }

        public void Close()
        {
            threadAvairable = false;

            // 受信スレッドを停止
            if(receiveThread != null)
            {
                receiveThread.Join();
                receiveThread = null;
            }

            // udpクライアントを閉じる
            if(udpClient != null)
                udpClient.Close();
            udpClient = null;
        }

        // 受信スレッド
        private void ReceiveData()
        {
            // udpクライアント設定
            udpClient = new UdpClient(port);

            threadAvairable = true;
            while(threadAvairable)
            {
                try
                {
                    if(udpClient.Available > 0)
                    {
                        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = udpClient.Receive(ref ipEndPoint);

                        receivedDataStr = Encoding.UTF8.GetString(data);
                        receivedDataBytes = data;
                        isDataReceived = true;
                    }
                }
                catch(Exception err)
                {
                    Debug.LogError(err.Message);
                }
            }
        }
    }
}
