#define USE_TMP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if USE_TMP
using TMPro;
#endif

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
        public TMP_Text uiText;
        public List<TMP_FontAsset> localizeFonts;
#else
        public Text uiTxt;
        public List<Font> localizeFonts;
#endif
        public LocalizeFontScriptable localizeFontScriptable;
        public List<string> localizeStrings;
        public LANGUAGE lang;


        void Awake()
        {
            localizeList.Add(this);

            if(uiText == null)
            {
#if USE_TMP
                uiText = this.gameObject.GetComponent<TMP_Text>();
#else
                uiText = this.gameObject.GetComponent<Text>();
#endif
            }
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

            if(uiText == null)
                return;

            if(localizeFontScriptable == null)
            {
                if(localizeFonts.Count > (int)lang)
                    uiText.font = localizeFonts[(int)lang];
            }
            else
            {
                LocalizeFont locFont = localizeFontScriptable.localizeFonts.Find(f => f.lang == lang);
                if(locFont != null && locFont.font != null)
                    uiText.font = locFont.font;
            }

            if(localizeStrings.Count == 1)
                uiText.text = localizeStrings[0];
            else if(localizeStrings.Count > (int)lang)
                uiText.text = localizeStrings[(int)lang];
        }
    }
}
