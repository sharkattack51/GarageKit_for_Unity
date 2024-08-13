//#define USE_ARFOUNDATION

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace GarageKit.ARFoundationExtention
{
#if USE_ARFOUNDATION
    [RequireComponent(typeof(ARSession))]
#endif
    public class ARSessionStartHelper : MonoBehaviour
    {
#if USE_ARFOUNDATION
        public ARSession arSession;

        // AR機能の初期化失敗
        public Action OnFailARSession;

        // ARライブラリのインストールリクエスト(ARCore)
        public Action OnRequestARLibInstall;

        // AR機能の初期化完了
        public Action OnReadyAR;


        void Awake()
        {
            arSession.Reset();
        }

        IEnumerator Start()
        {
            arSession.enabled = false;

            if(ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
                yield return ARSession.CheckAvailability();
            else if(ARSession.state == ARSessionState.Unsupported)
            {
                Debug.Log("ARSessionStartHelper :: ar unsupported");

                this.OnFailARSession?.Invoke();
                yield break;
            }

            if(ARSession.state == ARSessionState.NeedsInstall)
            {
                Debug.Log("ARSessionStartHelper :: ar library need install");

                this.OnRequestARLibInstall?.Invoke();
                yield return ARSession.Install();

                Debug.Log("ARSessionStartHelper :: ar library installed");
            }

            if(ARSession.state == ARSessionState.Ready)
            {
                Debug.Log("ARSessionStartHelper :: ar ready");

                this.OnReadyAR?.Invoke();
                arSession.enabled = true;
            }

            yield return null;
        }

        void Update()
        {

        }

        void OnDisable()
        {
            arSession.Reset();
        }
#endif
    }
}
