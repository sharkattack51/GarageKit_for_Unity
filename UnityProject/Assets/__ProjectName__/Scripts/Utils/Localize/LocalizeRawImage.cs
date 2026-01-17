using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit.Localize
{
    [ExecuteInEditMode]
    public class LocalizeRawImage : MonoBehaviour, ILocalize
    {
        private static List<LocalizeRawImage> localizeList = new List<LocalizeRawImage>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeRawImage rawImg in localizeList)
                rawImg.Localize(lang);
        }

        public RawImage uiRawImage;
        public List<Texture2D> localizeTexs;
        public LANGUAGE lang;


        void Awake()
        {
            localizeList.Add(this);

            if(uiRawImage == null)
                uiRawImage = this.gameObject.GetComponent<RawImage>();
        }

        void Start()
        {

        }

        void Update()
        {
            if(Application.isEditor)
                Localize(this.lang);
        }

        void OnDestroy()
        {
            localizeList.Remove(this);
        }


        public void Localize(LANGUAGE lang)
        {
            this.lang = lang;

            if(localizeTexs.Count == 1)
                uiRawImage.texture = localizeTexs[0];
            else if(localizeTexs.Count > (int)lang)
                uiRawImage.texture = localizeTexs[(int)lang];
        }
    }
}
