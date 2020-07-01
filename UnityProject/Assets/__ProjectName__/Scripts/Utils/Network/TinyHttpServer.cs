using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace GarageKit
{
    public class TinyHttpServer : MonoBehaviour
    {
        public bool listenOnStart = true;
        public int port = 8080;

        private HttpListener listener;

        public delegate string HttpResponce(HttpListenerContext context);
        public HttpResponce OnHttpRequest;


        void Awake()
        {

        }

        void Start()
        {
            if(listenOnStart)
                StartServer();
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            StopServer();
        }


        // httpサーバーを開始
        public void StartServer()
        {
            if(listener == null)
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://*:" + port.ToString() + "/");
                listener.Start();
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);

                string hostName = Dns.GetHostName();
                string hostAddress = "";
                foreach(IPAddress address in Dns.GetHostAddresses(hostName))
                {
                    hostAddress = address.ToString();
                    break;
                }

                Debug.Log(string.Format(
                    "Start TinyHttpServer ... http://localhost:{0} or http://{1}:{2}",
                    port.ToString(), hostAddress, port.ToString()));
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListenerContext context = ((HttpListener)result.AsyncState).EndGetContext(result);

            string responce = "";
            if(OnHttpRequest != null)
                responce = OnHttpRequest(context);

            byte[] data = Encoding.UTF8.GetBytes(responce);
            context.Response.StatusCode = 200;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.Close();

            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        // サーバーを停止
        public void StopServer()
        {
            if(listener != null)
                listener.Stop();
            listener = null;
        }
    }
}
