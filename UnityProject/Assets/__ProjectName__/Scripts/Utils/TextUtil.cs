using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GarageKit
{
    /// <summary>
    /// テキスト整形処理関連のユーティリティ
    /// </summary>
    public class TextUtil
    {	
        /// <summary>
        /// テキストの文字数制限
        /// </summary>
        public static string CutoutText(string text, int maxCount)
        {
            // 行端の不要文字を削除
            text = text.Trim();

            string cutOutText;
            if(text.Length > maxCount)
                cutOutText = text.Substring(0, maxCount);
            else
                cutOutText = text;

            return cutOutText;
        }

        /// <summary>
        /// テキストの行揃え
        /// </summary>
        public static string AlignText(string text, int lineCount)
        {
            // 行端の不要文字を削除
            text = text.Trim();

            string alignText = "";

            for(int i = 0; i < text.Length; i++)
            {
                // 改行を挿入
                if((i > 0) && ((i % lineCount) == 0))
                    alignText += "\n";

                alignText += text.Substring(i, 1);
            }

            return alignText;
        }

        /// <summary>
        /// 改行入りテキストを行文字数及び行数指定で整形
        /// </summary>
        public static string CutoutLineAndLfText(string lfText, int maxLineCount, int maxLineStringCount)
        {
            string[] splitLines = lfText.Split('\n');

            string alignedText = "";
            for(int i = 0; i < splitLines.Length; i++)
            {
                string aligned = AlignText(splitLines[i], maxLineStringCount);
                alignedText += aligned;

                if(i < (splitLines.Length - 1))
                    alignedText += "\n";
            }

            string[] alignedSplitLines = alignedText.Split('\n');

            string cutOutLineText = "";
            for(int j = 0; j < maxLineCount; j++)
            {
                cutOutLineText += alignedSplitLines[j];

                if(j < (maxLineCount - 1))
                    cutOutLineText += "\n";
            }

            return cutOutLineText;
        }

        /// <summary>
        /// 改行入りテキストを行数指定で整形
        /// </summary>
        public static string CutoutLine(string lfText, int maxLineCount)
        {	
            string[] splitLines = lfText.Split('\n');

            string cutOutLineText = "";
            for(int i = 0; i < maxLineCount; i++)
            {
                cutOutLineText += splitLines[i];

                if(i < (maxLineCount - 1))
                    cutOutLineText += "\n";
            }

            return cutOutLineText;
        }

        /// <summary>
        /// 改行を消す
        /// </summary>
        public static string RemoveEOL(string text)
        {
            return text.Replace("\\n", "").Replace("\n", "").Replace("\\r", "").Replace("\r", "");
        }

        /// <summary>
        /// WWWクラスを使用時の半角#のエラーを回避する
        /// </summary>
        public static string EscapeSingleByteSharpForWWW(string text)
        {
            return text.Replace("#","%23");
        }

        /// <summary>
        /// テキストのエンコード変換
        /// </summary>
        public static string ConvertEncoding(string srcString, Encoding destEncording)
        {
            byte[] srcBytes = Encoding.ASCII.GetBytes(srcString);
            byte[] destBytes = Encoding.Convert(Encoding.ASCII, destEncording, srcBytes);

            return destEncording.GetString(destBytes);
        }

        /// <summary>
        /// バイト数を考慮してSubstringする
        /// </summary>
        public static string SubstringDoubleByte(string srcString, int startIndex, int endIndex, Encoding encoding)
        {
            char[] charArry = srcString.ToCharArray();
            List<List<char>> byteCheckedChars = new List<List<char>>(); 

            for(int i = 0; i < charArry.Length; i++)
            {
                List<char> doubleByteChar = new List<char>(); 
                
                if(IsMultiByteChar(charArry[i], encoding)) //全角2バイト文字
                {
                    doubleByteChar.Add(charArry[i]);
                }
                else // 半角1バイト文字
                {
                    doubleByteChar.Add(charArry[i]);
                    
                    if((i + 1) < charArry.Length)
                    {
                        if(!IsMultiByteChar(charArry[i + 1], encoding)) //半角1バイト文字
                        {
                            doubleByteChar.Add(charArry[i + 1]);
                            i++;
                        }
                    }
                }
                
                byteCheckedChars.Add(doubleByteChar);
            }

            string subString = "";
            for(int j = 0; j < byteCheckedChars.Count; j++)
            {
                if(j < startIndex)
                    continue;

                if(j >= endIndex)
                    break;

                for(int k = 0; k < byteCheckedChars[j].Count; k++)
                    subString += byteCheckedChars[j][k];
            }

            return subString;
        }

        /// <summary>
        /// バイト数を考慮して文字数をカウント
        /// </summary>
        public static int MultiByteLength(string srcString, Encoding encoding)
        {
            char[] charArry = srcString.ToCharArray();
            float ct = 0.0f;

            for(int i = 0; i < charArry.Length; i++)
            {
                if(IsMultiByteChar(charArry[i], encoding))
                    ct += 1.0f;
                else
                    ct += 0.5f;
            }

            return Mathf.CeilToInt(ct);
        }

        /// <summary>
        /// 2バイト文字のチェック
        /// </summary>
        public static bool IsMultiByteChar(char srcChar, Encoding encoding)
        {
            if(encoding.GetByteCount(srcChar.ToString()) > 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 文字列が指定した幅に収まるかチェックし収まらない場合は改行する
        /// </summary>
        public static void CalcTextBox(TextMesh textMesh, float width = 1000.0f)
        {
            // 既に収まっている場合
            if(textMesh.GetComponent<Renderer>().bounds.size.x / textMesh.transform.lossyScale.x <= width)
                return;

            // スケール0は無効
            if(textMesh.transform.lossyScale.x <= 0.0f || textMesh.transform.lossyScale.y <= 0.0f || textMesh.transform.lossyScale.z <= 0.0f)
                return;

            // サイズが0の場合
            if(width <= 0.0f)
                return;

            string processedText = "";
            string tempText = textMesh.text;

            //幅設定
            int start = 0;
            int mid = 0;
            int min = 0;
            int max = tempText.Length;
            while(mid < tempText.Length)
            {
                if(min != 0)
                    processedText += "\n";

                start = mid;
                min = mid;
                max = tempText.Length;

                while(min <= max)
                {
                    mid = (min + max) / 2;
                    
                    //midで文字幅を比較
                    textMesh.text = tempText.Substring(start, mid - start);

                    float size = textMesh.GetComponent<Renderer>().bounds.size.x / textMesh.transform.lossyScale.x;
                    if(size < width)
                    {
                        min = mid + 1;
                    }
                    else if(size >= width)
                    {
                        max = mid - 1;
                        mid--; //はみ出して終了を防ぐ
                    }
                    else
                        break;
                }

                // 改行後の先頭文字をチェック
                int length = start + (mid - start + 1);
                if(length <= tempText.Length)
                {
                    string tmpChar = tempText.Substring(mid, 1);
                    string[] notKaigyouArry = new string[]{",", ".", ";", ":", "、", "。"};
                    
                    foreach(string checkChar in notKaigyouArry)
                    {
                        if(tmpChar == checkChar)
                        {
                            mid++;
                            break;
                        }
                    }
                }

                processedText += tempText.Substring(start, mid - start);
            }

            textMesh.text = processedText;
        }

        /// <summary>
        /// IPアドレスとポートに分割して文字列パースする
        /// </summary>
        public static bool ParseIpAndPort(string ipStr, out string ip, out int port)
        {
            string[] ipStrs = ipStr.Split(new string[]{ ":" }, StringSplitOptions.None);
            if(ipStrs.Length == 2)
            {
                ip = ipStrs[0];
                return int.TryParse(ipStrs[1], out port);
            }

            ip = "";
            port = 0;
            return false;
        }
    }
}
