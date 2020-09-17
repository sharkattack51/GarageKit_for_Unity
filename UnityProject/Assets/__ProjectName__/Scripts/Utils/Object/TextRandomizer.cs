using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit
{
    [RequireComponent(typeof(Text))]
    public class TextRandomizer : MonoBehaviour
    {
        public string randomChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#$%&()-=^~|@[{;+:*]}<>?";

        private Text uiText;


        void Awake()
        {

        }

        void Start()
        {
            uiText = this.gameObject.GetComponent<Text>();
        }

        void Update()
        {

        }


        public void TextRandomizeIn(string goalText, float delay = 0.0f, int insertRndChrs = 10)
        {
            StartCoroutine(RandomizeCoroutine(goalText, delay, insertRndChrs));
        }

        private IEnumerator RandomizeCoroutine(string goalText, float delay, int insertRndChars)
        {
            List<List<string>> charTable = new List<List<string>>();
            for(int i = 0; i < goalText.Length; i++)
            {
                List<string> charSeq = new List<string>();
                for(int j = 0; j <= i; j++)
                    charSeq.Add("");
                charSeq.Add("-");
                charSeq.Add("-");
                charSeq.Add("-");
                charSeq.Add("+");
                charSeq.Add("+");
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

                yield return new WaitForSeconds(0.03f);

                frame++;
            }
        }

        private string GetRandomChar()
        {
            return randomChars[Random.Range(0, randomChars.Length)].ToString();
        }
    }
}
