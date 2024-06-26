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

        public TMP_Dropdown uiDropdown;
        public List<TMP_FontAsset> localizeFonts;
        public List<LocalizeDropdownStrings> localizeDropdownStrings;


        void Awake()
        {
            localizeList.Add(this);

            if(uiDropdown == null)
                uiDropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        }

        void Start()
        {

        }

        void Update()
        {

        }


        public void Localize(LANGUAGE lang)
        {
            if(uiDropdown == null)
                uiDropdown = this.gameObject.GetComponent<TMP_Dropdown>();

            TMP_Text[] texts = uiDropdown.GetComponentsInChildren<TMP_Text>();
            foreach(TMP_Text text in texts)
                text.font = localizeFonts[(int)lang];

            uiDropdown.options.Clear();
            foreach(string str in localizeDropdownStrings[(int)lang].localizeStrings)
                uiDropdown.options.Add(new TMP_Dropdown.OptionData(str));
            uiDropdown.RefreshShownValue();
        }
    }
}
