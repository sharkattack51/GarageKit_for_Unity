using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GarageKit
{
    /// <summary>
    /// スクリーンキャプチャ関連のユーティリティ
    /// </summary>
    public class CaptureUtil
    {
        /// <summary>
        /// 範囲指定をしてキャプチャ保存
        /// </summary>
        public static IEnumerator CaptureRect(string fileName, bool withTimestamp)
        {
            yield return CaptureRect("", fileName, new Rect(0, 0, Screen.width, Screen.height), withTimestamp);
        }

        public static IEnumerator CaptureRect(string dirPath, string fileName, Rect range, bool withTimestamp)
        {
            yield return new WaitForEndOfFrame();

            Texture2D capture = ScreenCapture.CaptureScreenshotAsTexture();
            Color[] pixels = capture.GetPixels((int)range.x, (int)range.y, (int)range.width, (int)range.height);

            Texture2D crop = new Texture2D((int)range.width, (int)range.height, TextureFormat.RGB24, false, false);
            crop.SetPixels(pixels);
            crop.Apply();

            // 拡張子の確認
            string ext = Path.GetExtension(fileName);
            byte[] bytes = new byte[0];
            if(ext == "")
            {
                fileName += ".png";
                bytes = crop.EncodeToPNG();
            }
            else if(ext.ToLower() == ".png")
                bytes = crop.EncodeToPNG();
            else if(ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                bytes = crop.EncodeToJPG();

            // 保存先の確認
            if(string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
                dirPath = Path.GetDirectoryName(fileName);
                fileName = Path.GetFileName(fileName);
            }

            if(dirPath == "")
                dirPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); // save to desktop
            else if(!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if(withTimestamp)
                fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;

            // 保存
            File.WriteAllBytes(Path.Combine(dirPath, fileName), bytes);

            Texture2D.Destroy(capture);
            capture = null;
            Texture2D.Destroy(crop);
            crop = null;
            bytes = new byte[0];
        }
    }
}
