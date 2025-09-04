using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;

namespace GarageKit
{
    public class AsyncUtil
    {
        public static async UniTask<Texture2D> LoadTextureAsync(string path, CancellationToken ct = default)
        {
            Texture2D tex = null;

            UnityWebRequest req = UnityWebRequestTexture.GetTexture("file://" + path);
            try
            {
                ct.ThrowIfCancellationRequested();
                await req.SendWebRequest();
            }
            catch(Exception err)
            {
                throw new Exception(string.Format("texture load error: {0} [ {1} ]", path, err.Message));
            }

            if(req.result == UnityWebRequest.Result.Success)
                tex = DownloadHandlerTexture.GetContent(req);

            req.Dispose();

            return tex;
        }

        public static async UniTask<Texture2D[]> LoadTextureAllAsync(string[] paths, CancellationToken ct = default)
        {
            List<UniTask<Texture2D>> tasks = new List<UniTask<Texture2D>>();
            foreach(string path in paths)
                tasks.Add(LoadTextureAsync(path, ct));

            ct.ThrowIfCancellationRequested();
            return await UniTask.WhenAll(tasks);
        }

        public static async UniTask<string> DownloadAsync(string url, CancellationToken ct = default)
        {
            string res = "";

            UnityWebRequest req = UnityWebRequest.Get(url);
            try
            {
                ct.ThrowIfCancellationRequested();
                await req.SendWebRequest();
            }
            catch(Exception err)
            {
                throw new Exception(string.Format("download error: {0} [ {1} ]", url, err.Message));
            }

            if(req.result == UnityWebRequest.Result.Success)
                res = req.downloadHandler.text;

            req.Dispose();

            return res;
        }

        public static async UniTask DownloadFileAsync(string url, string dstFile, CancellationToken ct = default)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            try
            {
                ct.ThrowIfCancellationRequested();
                await req.SendWebRequest();
            }
            catch(Exception err)
            {
                throw new Exception(string.Format("download file error: {0} [ {1} ]", url, err.Message));
            }

            if(req.result == UnityWebRequest.Result.Success)
            {
                if(File.Exists(dstFile))
                    File.Delete(dstFile);
                await File.WriteAllBytesAsync(dstFile, req.downloadHandler.data, ct);
            }

            req.Dispose();
        }

        public static async UniTask DownloadFileAllAsync(string[] urls, string[] dstFiles, CancellationToken ct = default)
        {
            if(urls.Length != dstFiles.Length)
                throw new Exception("download file all error: number of urls and files does not match");

            List<UniTask> tasks = new List<UniTask>();
            for(int i = 0; i < urls.Length; i++)
                tasks.Add(DownloadFileAsync(urls[i], dstFiles[i], ct));

            ct.ThrowIfCancellationRequested();
            await UniTask.WhenAll(tasks);
        }
    }
}
