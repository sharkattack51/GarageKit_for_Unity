using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private List<string> receivedDataStrStack = new List<string>();
        private string latestReceivedDataStr = "";
        public string LatestReceivedDataStr { get{ return latestReceivedDataStr; } }
        private List<byte[]> receivedDataBytesStack = new List<byte[]>();
        private byte[] latestReceivedDataBytes = new byte[0];
        public byte[] LatestReceivedDataBytes { get{ return latestReceivedDataBytes; } }

        // データ受信イベント
        public Action<string> OnReceived;
        public Action<byte[]> OnReceivedBytes;

        private UdpClient udpClient = null;
        private bool receivedOne = false;


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
            while(receivedDataStrStack.Count > 0 && receivedDataBytesStack.Count > 0)
            {
                if(OnReceived != null)
                    OnReceived(receivedDataStrStack[0]);
                if(OnReceivedBytes != null)
                    OnReceivedBytes(receivedDataBytesStack[0]);

                receivedDataStrStack.RemoveAt(0);
                receivedDataBytesStack.RemoveAt(0);
            }
        }

        void OnDestroy()
        {
            Close();
        }


        public void Open()
        {
            udpClient = new UdpClient(port);
            udpClient.BeginReceive(ReceiveData, udpClient);
        }

        public void Close()
        {
            // udpクライアントを閉じる
            if(udpClient != null)
            {   
                if(receivedOne)
                    udpClient.Client.Shutdown(SocketShutdown.Receive);
                udpClient.Client.Close();
                udpClient.Close();
                udpClient.Dispose();
            }
            udpClient = null;
        }

        private void ReceiveData(IAsyncResult result)
        {
            UdpClient cli = (UdpClient)result.AsyncState;
            IPEndPoint ipEndPoint = null;

            try
            {
                byte[] bytes = cli.EndReceive(result, ref ipEndPoint);
                receivedDataStrStack.Add(Encoding.UTF8.GetString(bytes));
                receivedDataBytesStack.Add(bytes);

                latestReceivedDataStr = receivedDataStrStack[receivedDataStrStack.Count - 1];
                latestReceivedDataBytes = receivedDataBytesStack[receivedDataBytesStack.Count - 1];
            }
            catch
            {
                // pass
            }

            receivedOne = true;
            cli.BeginReceive(ReceiveData, cli);
        }
    }
}
