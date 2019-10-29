using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// 日本語入力の全角変換中に確定させない状態でInputFieldからフォーカスを外すと変換中の文字が倍加するバグがあり、倍加させない
// original script
// https://qiita.com/monolith8/items/a88b13ecc1121fa10450

namespace GarageKit 
{
    [RequireComponent(typeof(InputField))]
    public class InputfieldZenkakuBSFix : MonoBehaviour
    {
        private InputField uiInputField;

        private string currentStr = "";
        private int currentLength = 0;
        private int selectinAnchorPos = 0;


        void Awake()
        {

        }

        void Start()
        {
            uiInputField = this.gameObject.GetComponent<InputField>();
        } 
        
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(uiInputField.text != "")
                {
                    if(currentStr != uiInputField.text)
                    {
                        if(currentStr == "")
                        {
                            if(uiInputField.text.Length > 1 && uiInputField.text.Length % 2 == 0)
                            {
                                int halfLen = uiInputField.text.Length / 2;

                                if(uiInputField.text.Substring(0, halfLen) == uiInputField.text.Substring(halfLen, halfLen))
                                {
                                    // 未確定時
                                    uiInputField.text = uiInputField.text.Substring(0, halfLen);
                                }
                            }
                        }
                        else
                        {
                            if(uiInputField.text.Length - currentLength > 1)
                            {
                                if(currentLength + (selectinAnchorPos - currentLength) * 2 == uiInputField.text.Length)
                                {
                                    string ck = uiInputField.text.Substring(currentLength);

                                    if(ck.Length > 1 && ck.Length % 2 == 0)
                                    {
                                        int halfLen = ck.Length / 2;
                                        if(ck.Substring(0, halfLen) == ck.Substring(halfLen, halfLen))
                                        {
                                            // 未確定時
                                            string subStr1 = uiInputField.text.Substring(0, currentLength);
                                            string subStr2 = ck.Substring(0, halfLen);
                                            uiInputField.text = subStr1 + subStr2;
                                        }
                                    }
                                }
                                else
                                {
                                    int back = (uiInputField.text.Length - (currentLength + (selectinAnchorPos - currentLength) * 2)) / 2;
                                    int front = currentLength - back;
                                    string ck = uiInputField.text.Remove(uiInputField.text.Length - back).Substring(front);

                                    if(ck.Length > 1 && ck.Length % 2 == 0)
                                    {
                                        int halfLen = ck.Length / 2;
                                        if(ck.Substring(0, halfLen) == ck.Substring(halfLen, halfLen))
                                        {
                                            string subStr1 = uiInputField.text.Substring(0, front);
                                            string subStr2 = ck.Substring(0, halfLen);
                                            string subStr3 = uiInputField.text.Substring(uiInputField.text.Length - back, back);
                                            uiInputField.text = subStr1 + subStr2 + subStr3;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            currentStr = uiInputField.text;
            currentLength = uiInputField.text.Length;
            selectinAnchorPos = uiInputField.selectionAnchorPosition;
        }

        void LateUpdate()
        {
            // 他のinputfieldに変換中の文字が表示されるのを防ぐため選択中のみ
            if(uiInputField.GetComponent<InputField>().isFocused == true)
            {
                // 強制的にラベルを即時更新します。キャレットと表示されている文字列の位置を再計算します。
                uiInputField.ForceLabelUpdate();
            }
        }
    }
}
