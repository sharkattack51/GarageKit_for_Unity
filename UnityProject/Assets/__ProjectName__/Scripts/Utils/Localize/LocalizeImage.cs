using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit.Localize
{
    [ExecuteInEditMode]
    public class LocalizeImage : MonoBehaviour, ILocalize
    {
        private static List<LocalizeImage> localizeList = new List<LocalizeImage>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeImage img in localizeList)
                img.Localize(lang);
        }

        public Image uiImage;
        public List<Sprite> localizeSprites;
        public LANGUAGE lang;


        void Awake()
        {
            localizeList.Add(this);

            if(uiImage == null)
                uiImage = this.gameObject.GetComponent<Image>();
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

            if(localizeSprites.Count == 1)
                uiImage.sprite = localizeSprites[0];
            else if(localizeSprites.Count > (int)lang)
                uiImage.sprite = localizeSprites[(int)lang];
        }
    }
}
