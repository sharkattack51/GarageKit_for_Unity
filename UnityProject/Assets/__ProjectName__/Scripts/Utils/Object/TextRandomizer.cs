using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace GarageKit
{
    public class TextRandomizer : MonoBehaviour
    {
        public string randomChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#$%&()-=^~|@[{;+:*]}<>?";
        public TMP_Text uiText;


        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        void OnDisable()
        {
            StopCoroutine("InternalRandomizeCoroutine");
        }


        public void TextRandomizeIn(string goalText, float delay = 0.0f, int insertRndChrs = 10, bool defaultSalt = true, float tick = 0.03f)
        {
            object[] args = new object[]{ goalText, delay, insertRndChrs, defaultSalt, tick };
            StartCoroutine("InternalRandomizeCoroutine", args);
        }

        private IEnumerator InternalRandomizeCoroutine(object[] args)
        {
            string goalText = (string)args[0];
            float delay = (float)args[1];
            int insertRndChrs = (int)args[2];
            bool defaultSalt = (bool)args[3];
            float tick = (float)args[4];

            yield return RandomizeCoroutine(goalText, delay, insertRndChrs, defaultSalt, tick);
        }

        private IEnumerator RandomizeCoroutine(string goalText, float delay, int insertRndChars, bool defaultSalt, float tick)
        {
            List<List<string>> charTable = new List<List<string>>();
            for(int i = 0; i < goalText.Length; i++)
            {
                List<string> charSeq = new List<string>();
                for(int j = 0; j <= i; j++)
                    charSeq.Add("");
                if(defaultSalt)
                {
                    charSeq.Add("-");
                    charSeq.Add("-");
                    charSeq.Add("-");
                    charSeq.Add("+");
                    charSeq.Add("+");
                }
                else
                {
                    for(int j = 0; j < 5; j++)
                        charSeq.Add(GetRandomChar());
                }
                for(int j = 0; j < insertRndChars; j++)
                    charSeq.Add(GetRandomChar());
                for(int j = 0; j < goalText.Length - i; j++)
                    charSeq.Add(goalText[i].ToString());

                charTable.Add(charSeq);
            }

            uiText.text = "";
            yield return new WaitForSeconds(delay);

            int frame = 0;
            int length = charTable[0].Count;
            while(frame < length)
            {
                uiText.text = "";
                for(int i = 0; i < goalText.Length; i++)
                    uiText.text += charTable[i][frame];

                yield return new WaitForSeconds(tick);

                frame++;
            }
        }

        private string GetRandomChar()
        {
            return randomChars[Random.Range(0, randomChars.Length)].ToString();
        }
    }
}
