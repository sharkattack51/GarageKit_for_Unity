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
        private bool isDataReceived = false;

        // データ受信イベント
        public Action<string> OnReceived;
        public Action<byte[]> OnReceivedBytes;

        private UdpClient udpClient = null;


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
            udpClient = new UdpClient(port);
            udpClient.BeginReceive(ReceiveData, udpClient);
        }

        public void Close()
        {
            // udpクライアントを閉じる
            if(udpClient != null)
                udpClient.Close();
            udpClient = null;
        }

        private void ReceiveData(IAsyncResult result)
        {
            UdpClient cli = (UdpClient)result.AsyncState;
            IPEndPoint ipEndPoint = null;
            byte[] bytes = cli.EndReceive(result, ref ipEndPoint);

            receivedDataStr = Encoding.UTF8.GetString(bytes);
            receivedDataBytes = bytes;
            isDataReceived = true;

            cli.BeginReceive(ReceiveData, cli);
        }
    }
}
