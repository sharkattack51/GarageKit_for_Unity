using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GarageKit.CSV
{
    public class CsvWriter
    {
        private string outputDir = "./CSV";


        public CsvWriter(string outputDir)
        {
            this.outputDir =  outputDir;

            // 出力先ディレクトリの確認
            if(!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
        }

        public string Write(string file, ICsvWritable data)
        {
            return Write(file, new List<ICsvWritable>(){ data });
        }

        public string Write(string file, List<ICsvWritable> datas)
        {
            if(datas.Count == 0)
                return "";

            string rows = "";

            // 同名ファイルの確認
            string path = Path.Join(outputDir, file);
            if(!File.Exists(path))
                rows += datas[0].GetCsvHeader() + "\n";
            else
                rows += File.ReadAllText(path, Encoding.GetEncoding("shift_jis")).Trim() + "\n";

            foreach(ICsvWritable data in datas)
                rows += data.ToCsvRowString() + "\n";

            // 書き出し
            if(File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, rows, Encoding.GetEncoding("shift_jis"));

            return path;
        }
    }
}
