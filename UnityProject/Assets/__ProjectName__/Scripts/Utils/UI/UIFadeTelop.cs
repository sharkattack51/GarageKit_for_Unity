using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace GarageKit
{
    public class UIFadeTelop : UIFadeGroupComponent
    {
        public TMP_Text uiText;
        public Image uiTextBg;
        public bool resizeBg = true;


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if(resizeBg && uiTextBg != null)
            {
                uiTextBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiText.preferredWidth + 50.0f);
                uiTextBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiText.preferredHeight + 20.0f);
            }
        }


        public void StartTelop(string message, float fixTime = 5.0f, float tweenTime = 0.5f)
        {
            if(uiText != null)
                uiText.text = message;

            base.StartFade(fixTime, tweenTime);
        }
    }
}
