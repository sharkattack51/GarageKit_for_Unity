//#define USE_TMP_Ruby

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
#if USE_TMP_Ruby
using TMP_Ruby;
#endif

namespace GarageKit.Localize
{
    [ExecuteInEditMode]
    public class LocalizeRubyText : MonoBehaviour, ILocalize
    {
        // [TextMeshProRuby](https://github.com/ina-amagami/TextMeshProRuby)

        private static List<LocalizeRubyText> localizeList = new List<LocalizeRubyText>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeRubyText txt in localizeList)
                txt.Localize(lang);
        }

        public TMP_Text uiText;
        public List<TMP_FontAsset> localizeFonts;
        public LocalizeFontScriptable localizeFontScriptable;
        public List<string> localizeStrings;
        public LANGUAGE lang;

        [Header("TextMeshProRuby")]
        public bool fixedLineHeight = true;
        public bool autoMarginTop = false;


        void Awake()
        {
            localizeList.Add(this);

            if(uiText == null)
                uiText = this.gameObject.GetComponent<TMP_Text>();
        }

        void Start()
        {

        }

        void Update()
        {
            if(Application.isEditor && !Application.isPlaying)
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
                if(localizeFonts != null && (int)lang >= 0 && localizeFonts.Count > (int)lang)
                    uiText.font = localizeFonts[(int)lang];
            }
            else
            {
                LocalizeFont locFont = localizeFontScriptable.localizeFonts.Find(f => f.lang == lang);
                if(locFont != null && locFont.font != null)
                    uiText.font = locFont.font;
            }

#if USE_TMP_Ruby
            if(localizeStrings.Count == 1)
                uiText.SetTextAndExpandRuby(localizeStrings[0], fixedLineHeight, autoMarginTop);
            else if((int)lang >= 0 && localizeStrings.Count > (int)lang)
                uiText.SetTextAndExpandRuby(localizeStrings[(int)lang], fixedLineHeight, autoMarginTop);
#endif
        }
    }
}
