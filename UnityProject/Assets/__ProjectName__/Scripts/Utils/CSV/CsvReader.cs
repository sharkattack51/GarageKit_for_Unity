using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

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


        public void Read(string file)
        {
            // クリア
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

                isVaild = true;
            }
        }

        private void Parse()
        {
            string[] lines = csvString.Split("\n"[0]);

            // 行数設定
            int width = 0; 
            for(int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]); 
                width = Mathf.Max(width, row.Length); 
            }

            // 2次元配列を作成
            csvGrid = new string[width + 1, lines.Length + 1]; 
            for(int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]); 
                for(int x = 0; x < row.Length; x++) 
                {
                    csvGrid[x,y] = row[x]; 

                    // 置き換え処理
                    csvGrid[x,y] = csvGrid[x,y].Replace("\"\"", "\"");
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
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {
                    outputText += grid[x, y];
                    outputText += "|";
                }

                outputText += "\n";
            }

            Debug.Log(outputText);
        }
    }
}
