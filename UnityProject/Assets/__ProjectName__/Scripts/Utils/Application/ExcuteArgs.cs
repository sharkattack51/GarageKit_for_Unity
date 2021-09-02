using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * コンテンツ起動時の引数を受け取る
 */
namespace GarageKit
{
    public class ExcuteArgs : MonoBehaviour
    {	
        private string[] args;
        public string[] Args { get{ return args; } }


        void Awake()
        {
            //コマンドライン実行引数を取得する
            if(Application.platform != RuntimePlatform.WindowsEditor)
            {
                args = System.Environment.GetCommandLineArgs();

                for(int i = 0; i < args.Length; i++)
                    Debug.Log("excute args [" + i.ToString() + "] : " + args[i]);
            }
        }
    }
}
