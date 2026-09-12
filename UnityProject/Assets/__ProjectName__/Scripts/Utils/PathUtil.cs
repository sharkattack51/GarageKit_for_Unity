using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class PathUtil
{
    public static string GetUniqueFilePath(string filePath)
        {
            if(!File.Exists(filePath))
                return filePath;

            string dir = Path.GetDirectoryName(filePath) ?? ".";
            string name = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);

            return Enumerable.Range(1, int.MaxValue)
                .Select(i => Path.Combine(dir, $"{name} ({i}){ext}"))
                .First(p => !File.Exists(p));
        }

        public static string EncodeFileUri(string fileUri)
        {
            if(string.IsNullOrEmpty(fileUri))
                return "";

            return new Uri(fileUri).AbsoluteUri;
        }
}
