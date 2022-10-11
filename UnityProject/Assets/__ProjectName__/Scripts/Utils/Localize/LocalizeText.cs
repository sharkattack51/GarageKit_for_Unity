using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace GarageKit.Localize
{
    [ExecuteInEditMode]
    public class LocalizeText : MonoBehaviour, ILocalize
    {
        private static List<LocalizeText> localizeList = new List<LocalizeText>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeText txt in localizeList)
                txt.Localize(lang);
        }

        public List<TMP_FontAsset> localizeFonts;
        public List<string> localizeStrings;
        public LANGUAGE lang;

        private TMP_Text uiTxt;


        void Awake()
        {
            localizeList.Add(this);

            uiTxt = this.gameObject.GetComponent<TMP_Text>();
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

            if(localizeFonts.Count > (int)lang)
                uiTxt.font = localizeFonts[(int)lang];
            if(localizeStrings.Count > (int)lang)
                uiTxt.text = localizeStrings[(int)lang];
        }
    }
}
