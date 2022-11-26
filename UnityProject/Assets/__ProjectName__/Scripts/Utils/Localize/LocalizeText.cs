#define USE_TMP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

#if USE_TMP
        public List<TMP_FontAsset> localizeFonts;
#else
        public List<Font> localizeFonts;
#endif
        public List<string> localizeStrings;
        public LANGUAGE lang;

#if USE_TMP
        private TMP_Text uiTxt;
#else
        private Text uiTxt;
#endif

        void Awake()
        {
            localizeList.Add(this);

#if USE_TMP
            uiTxt = this.gameObject.GetComponent<TMP_Text>();
#else
            uiTxt = this.gameObject.GetComponent<Text>();
#endif
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
