using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit
{
    public class UIFadeTelop : UIFadeGroupComponent
    {
        public bool resizeBg = true;

        private Text uiText;
        private Image uiTextBg;


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            uiText = this.gameObject.GetComponentInChildren<Text>();
            uiTextBg = this.gameObject.GetComponentInChildren<Image>();
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
