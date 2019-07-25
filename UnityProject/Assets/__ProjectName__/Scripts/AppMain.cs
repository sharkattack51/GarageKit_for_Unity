using UnityEngine;
using System.Collections;
using System;

/*
 * Aplplicationのメインクラス
 */
namespace GarageKit
{
	public class AppMain : MonoBehaviour
	{
		// singleton
		private static AppMain instance;
		public static AppMain Instance { get{ return instance; } }
		
		public SceneStateManager sceneStateManager;
		public TimeManager timeManager;
		public SoundManager soundManager;
		public UserInputManager userInputManager;
		public DebugManager debugManager;


		void Awake()
		{
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