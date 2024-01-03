using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

using Cysharp.Threading.Tasks;

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
        public void Send(string dataStr, string address = null, int? port = null)
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Send(data, address, port);
        }

        public void Send(byte[] data, string address = null, int? port = null)
        {
            if(!this.gameObject.activeSelf)
                return;

            if(address == null)
                address = this.address;
            if(port == null)
                port = this.port;

            UniTask.RunOnThreadPool(() => {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                    IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostAddresses(address)[0], (int)port);
                    socket.SendTo(data, 0, data.Length, SocketFlags.None, ipEndPoint);
                    socket.Close();
                    socket.Dispose();
                }
                catch(Exception e)
                {
                    Debug.LogWarning("UDPSender :: " + e.Message);
                }
            }).Forget();
        }

        public void TryContinuousSend(string dataStr, int tryCount, float span = 0.1f, string address = null, int? port = null)
        {
            StartCoroutine(ContinuousSendCoroutine(dataStr, tryCount, span, address, port));
        }

        private IEnumerator ContinuousSendCoroutine(string dataStr, int tryCount, float span, string address, int? port)
        {
            for(int i = 0; i < tryCount; i++)
            {
                Send(dataStr);
                yield return new WaitForSeconds(span);
            }
        }
#endregion

#region Broadcast
        public void Broadcast(string dataStr, int? port = null)
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Broadcast(data, port);
        }

        public void Broadcast(byte[] data, int? port = null)
        {
            if(!this.gameObject.activeSelf)
                return;

            if(port == null)
                port = this.port;

            UniTask.RunOnThreadPool(() => {
                try
                {
                    Socket broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    broadcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 16);
                    broadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Broadcast, (int)port);
                    broadcastSocket.SendTo(data, 0, data.Length, SocketFlags.None, ipEndPoint);
                    broadcastSocket.Close();
                    broadcastSocket.Dispose();
                }
                catch(Exception e)
                {
                    Debug.LogWarning("UDPSender :: " + e.Message);
                }
            }).Forget();
        }

        public void TryContinuousBroadcast(string dataStr, int tryCount, float span = 0.1f, int? port = null)
        {
            StartCoroutine(ContinuousBroadcastCoroutine(dataStr, tryCount, span, port));
        }

        private IEnumerator ContinuousBroadcastCoroutine(string dataStr, int tryCount, float span, int? port)
        {
            for(int i = 0; i < tryCount; i++)
            {
                Broadcast(dataStr);
                yield return new WaitForSeconds(span);
            }
        }
#endregion
    }
}
