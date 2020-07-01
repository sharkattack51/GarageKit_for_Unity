using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Editor内で編集時は非表示にし実行時に表示させる
 */
namespace GarageKit
{
    [RequireComponent(typeof(Renderer))]
    public class EditorHide : MonoBehaviour
    {	
        void Awake()
        {	
            //実行時に表示
            this.GetComponent<Renderer>().enabled = true;
        }
    }
}
