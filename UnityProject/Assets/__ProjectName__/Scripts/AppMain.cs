using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Applicationのメインクラス
 */
namespace GarageKit
{
    public class AppMain : MonoBehaviour
    {
        // singleton
        private static AppMain instance;
        public static AppMain Instance { get{ return instance; } }

        [Header("Managers")]		
        public SceneStateManager sceneStateManager;
        public TimeManager timeManager;
        public SoundManager soundManager;
        public UserInputManager userInputManager;
        public DebugManager debugManager;

        [Header("Multiple Scene")]
        public bool asDontDestroyOnLoad = false;


        void Awake()
        {
            if(asDontDestroyOnLoad)
            {
                if(instance == null)
                {
                    DontDestroyOnLoad(this.gameObject);
                    instance = this;
                }
                else
                    GameObject.DestroyImmediate(this.gameObject);
            }
            else
                instance = this;
        }

        void Start()
        {
            // アプリケーションをスタートする
            sceneStateManager.InitState();
        }

        void Update()
        {

        }
    }
}
