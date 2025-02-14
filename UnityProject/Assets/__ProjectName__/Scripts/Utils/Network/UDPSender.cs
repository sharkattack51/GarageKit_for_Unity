using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public void Send(string dataStr, string address = null, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Send(data, address, port, ct);
        }

        public void Send(byte[] data, string address = null, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            if(!this.gameObject.activeSelf)
                return;

            if(address == null)
                address = this.address;
            if(port == null)
                port = this.port;

            UniTask.RunOnThreadPool(() => {
                if(ct.IsCancellationRequested)
                    return;

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
            }, cancellationToken: ct).Forget();
        }

        public void TryContinuousSend(string dataStr, int tryCount, float span = 0.1f, string address = null, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            StartCoroutine(ContinuousSendCoroutine(dataStr, tryCount, span, address, port, ct));
        }

        private IEnumerator ContinuousSendCoroutine(string dataStr, int tryCount, float span, string address, int? port, CancellationToken ct = default(CancellationToken))
        {
            for(int i = 0; i < tryCount; i++)
            {
                if(ct.IsCancellationRequested)
                    break;
                else
                    Send(dataStr, address, port);

                yield return new WaitForSeconds(span);
            }
        }
#endregion

#region RangeSend
        private List<string> MakeAddressRange(string startAddress, int addressNum)
        {
            List<string> addresses = new List<string>();

            string[] ips = startAddress.Split(new string[]{ "." }, StringSplitOptions.None);
            if(ips.Length != 4)
            {
                Debug.LogWarning("UDPSender :: address range error");
                return addresses;
            }

            int[] ipNums = new int[4];
            bool err = false;
            for(int i = 0; i < 4; i++)
            {
                if(!int.TryParse(ips[i], out ipNums[i]))
                {
                    err = true;
                    break;
                }

                if(ipNums[i] < 0 || ipNums[i] > 255)
                {
                    err = true;
                    break;
                }
            }
            if(err)
            {
                Debug.LogWarning("UDPSender :: make address range error");
                return addresses;
            }

            for(int i = ipNums[3]; i < ipNums[3] + addressNum; i++)
            {
                if(i < 0 || i > 255)
                {
                    Debug.LogWarning("UDPSender :: make address range error");
                    break;
                }

                addresses.Add(string.Format("{0}.{1}.{2}.{3}", ipNums[0], ipNums[1], ipNums[2], i));
            }
            return addresses;
        }

        public void RangeSend(string dataStr, List<string> addresses, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            foreach(string address in addresses)
                Send(dataStr, address, port, ct);
        }

        public void RangeSend(string dataStr, string startAddress, int addressNum, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            RangeSend(dataStr, MakeAddressRange(startAddress, addressNum), port, ct);
        }

        public void TryContinuousRangeSend(string dataStr, List<string> addresses, int tryCount, float span = 0.1f, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            foreach(string address in addresses)
                TryContinuousSend(dataStr, tryCount, span, address, port, ct);
        }

        public void TryContinuousRangeSend(string dataStr, string startAddress, int addressNum, int tryCount, float span = 0.1f, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            TryContinuousRangeSend(dataStr, MakeAddressRange(startAddress, addressNum), tryCount, span, port, ct);
        }
#endregion

#region Broadcast
        public void Broadcast(string dataStr, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            // byte配列に変換して送信
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            Broadcast(data, port, ct);
        }

        public void Broadcast(byte[] data, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            if(!this.gameObject.activeSelf)
                return;

            if(port == null)
                port = this.port;

            UniTask.RunOnThreadPool(() => {
                if(ct.IsCancellationRequested)
                    return;

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
            }, cancellationToken: ct).Forget();
        }

        public void TryContinuousBroadcast(string dataStr, int tryCount, float span = 0.1f, int? port = null, CancellationToken ct = default(CancellationToken))
        {
            StartCoroutine(ContinuousBroadcastCoroutine(dataStr, tryCount, span, port, ct));
        }

        private IEnumerator ContinuousBroadcastCoroutine(string dataStr, int tryCount, float span, int? port, CancellationToken ct = default(CancellationToken))
        {
            for(int i = 0; i < tryCount; i++)
            {
                if(ct.IsCancellationRequested)
                    break;
                else
                    Broadcast(dataStr, port);

                yield return new WaitForSeconds(span);
            }
        }
#endregion
    }
}
