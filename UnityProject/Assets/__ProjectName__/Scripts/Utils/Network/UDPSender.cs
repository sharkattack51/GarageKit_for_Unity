using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/*
 * UDP送信を行うクラス
 */
namespace GarageKit
{
    public class UDPSender : MonoBehaviour
    {
        // 送信先アドレス&ポート
        public string address = "localhost";
        public int port = 8000;


        void Awake()
        {

        }

        void Start()
        {

        }


#region Send
        public void Send(string dataStr)
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Send(data);
        }

        public void Send(byte[] data)
        {
            if(!this.gameObject.activeSelf)
                return;
            
            // ソケットの設定
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
            IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
            
            // 送信
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
#endregion

#region Broadcast
        public void Broadcast(string dataStr)
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Broadcast(data);
        }

        public void Broadcast(byte[] data)
        {
            if(!this.gameObject.activeSelf)
                return;
            
            // ソケットの設定
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 16);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Broadcast, port);

            // 送信
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
#endregion
    }
}
