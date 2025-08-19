using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace GarageKit
{
    public class CommonUtil
    {
        public static string GetPlatformResourceRootDirectory(string appRootDir = "")
        {
            string dir;
            if(Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            {
                dir = Path.Combine(Application.persistentDataPath, appRootDir);

                // no backup flag
                IOSUtil.NoBackupDocumentsFolder();
            }
            else if(Application.platform == RuntimePlatform.Android && !Application.isEditor)
            {
                dir = Path.Combine(AndroidUtil.ExternalStorageDir(), appRootDir);

                // full file access permission
                AndroidUtil.RequestAllFilesAccessPermission();
            }
            else
                dir = Path.GetFullPath(Path.Combine(".", appRootDir));

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public static void OpenFolder(string path)
        {
            Process.Start(new ProcessStartInfo() {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public static async UniTask CopyDirectoryAsync(string src, string dest, bool overwriteAsLatest = true, CancellationToken ct = default)
        {
            bool copied = false;

            try{
                await UniTask.RunOnThreadPool(() => {
                    CopyDirectory(src, dest, overwriteAsLatest);
                }, true, ct);
                copied = true;
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                copied = true;
            }

            await UniTask.WaitUntil(() => copied);
        }

        public static void CopyDirectory(string src, string dest, bool overwriteAsLatest = true)
        {
            DirectoryInfo srcDirInfo = new DirectoryInfo(src);
            if(!srcDirInfo.Exists)
                return;
            if(srcDirInfo.Name.Substring(0, 1) == ".")
                return;

            DirectoryInfo destDirInfo = new DirectoryInfo(dest);
            if(destDirInfo.Exists == false)
            {
                destDirInfo.Create();
                File.SetAttributes(destDirInfo.FullName, File.GetAttributes(srcDirInfo.FullName));
                UnityEngine.Debug.LogFormat("create dir: {0}", destDirInfo.FullName);
            }

            // proc files
            foreach(FileInfo srcFileInfo in srcDirInfo.GetFiles()) 
            {
                string[] ignores = new string[]{ ".DS_Store", "Thumb.db" };
                if(ignores.Contains(srcFileInfo.Name))
                    continue;
                if(srcFileInfo.Name.Substring(0, 1) == ".")
                    continue;

                string dstFile = Path.Join(destDirInfo.FullName, srcFileInfo.Name);
                FileInfo dstFileInfo = new FileInfo(dstFile);

                if(dstFileInfo.Exists)
                {
                    if(overwriteAsLatest && (srcFileInfo.LastWriteTime != dstFileInfo.LastWriteTime))
                    {
                        srcFileInfo.CopyTo(dstFile, true);
                        File.SetAttributes(dstFileInfo.FullName, File.GetAttributes(srcFileInfo.FullName));
                        UnityEngine.Debug.LogFormat("copy file: {0}", dstFileInfo.FullName);
                    }
                }
                else
                {
                    srcFileInfo.CopyTo(dstFile);
                    File.SetAttributes(dstFileInfo.FullName, File.GetAttributes(srcFileInfo.FullName));
                    UnityEngine.Debug.LogFormat("copy file: {0}", dstFileInfo.FullName);
                }
            }

            // proc dirs
            foreach(DirectoryInfo srcSubDirInfo in srcDirInfo.GetDirectories())
                CopyDirectory(srcSubDirInfo.FullName, Path.Join(destDirInfo.FullName, srcSubDirInfo.Name));
        }
    }
}
