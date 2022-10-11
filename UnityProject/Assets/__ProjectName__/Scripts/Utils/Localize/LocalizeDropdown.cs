using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace GarageKit.Localize
{
    [Serializable]
    public class LocalizeDropdownStrings
    {
        public List<string> localizeStrings;
    }

    public class LocalizeDropdown : MonoBehaviour, ILocalize
    {
        private static List<LocalizeDropdown> localizeList = new List<LocalizeDropdown>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeDropdown dd in localizeList)
                dd.Localize(lang);
        }

        public List<TMP_FontAsset> localizeFonts;
        public List<LocalizeDropdownStrings> localizeDropdownStrings;

        private TMP_Dropdown uiDrpdwn;


        void Awake()
        {
            localizeList.Add(this);

            uiDrpdwn = this.gameObject.GetComponent<TMP_Dropdown>();
        }

        void Start()
        {

        }

        void Update()
        {

        }


        public void Localize(LANGUAGE lang)
        {
            if(uiDrpdwn == null)
                uiDrpdwn = this.gameObject.GetComponent<TMP_Dropdown>();

            TMP_Text[] texts = uiDrpdwn.GetComponentsInChildren<TMP_Text>();
            foreach(TMP_Text text in texts)
                text.font = localizeFonts[(int)lang];

            uiDrpdwn.options.Clear();
            foreach(string str in localizeDropdownStrings[(int)lang].localizeStrings)
                uiDrpdwn.options.Add(new TMP_Dropdown.OptionData(str));
            uiDrpdwn.RefreshShownValue();
        }
    }
}
