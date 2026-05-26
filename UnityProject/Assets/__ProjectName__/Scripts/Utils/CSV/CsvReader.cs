using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;

namespace GarageKit.CSV
{
    public class CsvReader 
    {
        private bool isVaild = false;
        public bool IsVaild { get{ return isVaild; } }

        // csvデータを2次元配列で展開
        private string[,] csvGrid = new string[0, 0];
        public string[,] CsvGrid { get{ return csvGrid; } }

        private string csvString = "";


        public void ReadFileStream(string file)
        {
            csvGrid = new string[0, 0];

            try
            {		
                StreamReader strem = new StreamReader(file, Encoding.UTF8);
                csvString = strem.ReadToEnd();
                strem.Close();

                Parse();

                isVaild = true;
            }
            catch(Exception err)
            {
                Debug.Log(err.Message);
                isVaild = false;
            }
        }

        public async UniTask ReadWebRequestAsync(string file, Encoding enc, CancellationToken tkn = default)
        {
            csvGrid = new string[0, 0];

            try
            {
                UnityWebRequest req = UnityWebRequest.Get(file);
                await req.SendWebRequest().WithCancellation(tkn);

                if(req.result == UnityWebRequest.Result.Success)
                {
                    csvString = enc.GetString(req.downloadHandler.data);

                    Parse();
                    isVaild = true;
                }
                else
                {
                    Debug.LogError(req.error);
                    isVaild = false;
                }

                req.Dispose();
                req = null;
            }
            catch(Exception err)
            {
                Debug.Log(err.Message);
                isVaild = false;
            }
        }

        public void ReadFromString(string csvString)
        {
            csvGrid = new string[0, 0];
            this.csvString = csvString;

            Parse();
            isVaild = true;
        }

        private void Parse()
        {
            csvString = csvString.Replace("\r\n", "\n");
            string[] lines = csvString.Split("\n");

            // 行数設定
            int width = 0; 
            for(int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]); 
                width = Mathf.Max(width, row.Length); 
            }

            // 2次元配列を作成
            csvGrid = new string[lines.Length + 1, width + 1]; 
            for(int row = 0; row < lines.Length; row++)
            {
                string[] cols = SplitCsvLine(lines[row]); 
                for(int col = 0; col < cols.Length; col++) 
                {
                    csvGrid[row, col] = cols[col]; 

                    // 置き換え処理
                    csvGrid[row, col] = csvGrid[row, col].Replace("\"\"", "\"");
                }
            }
        }

        // csvライン分割
        private string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                line,
                @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
                System.Text.RegularExpressions.RegexOptions.ExplicitCapture) select m.Groups[1].Value).ToArray();
        }

        // デバッグ用出力
        static public void DebugLogGrid(string[,] grid)
        {
            string outputText = ""; 
            for(int row = 0; row < grid.GetUpperBound(0); row++)
            {
                for(int col = 0; col < grid.GetUpperBound(1); col++)
                {
                    outputText += grid[row, col];
                    outputText += "|";
                }

                outputText += "\n";
            }

            Debug.Log(outputText);
        }
    }
}
