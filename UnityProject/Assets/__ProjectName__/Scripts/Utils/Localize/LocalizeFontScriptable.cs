#define USE_TMP

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_TMP
using TMPro;
#endif

namespace GarageKit.Localize
{
    [CreateAssetMenu(menuName = "GarageKit/Localize Font Asset", fileName = "LocalizeFontAsset")]
    public class LocalizeFontScriptable : ScriptableObject
    {
        public List<LocalizeFont> localizeFonts;
    }

    [Serializable]
    public class LocalizeFont
    {
        public LANGUAGE lang;
#if USE_TMP
        public TMP_FontAsset font;
#else
        public Font font;
#endif
    }
}
