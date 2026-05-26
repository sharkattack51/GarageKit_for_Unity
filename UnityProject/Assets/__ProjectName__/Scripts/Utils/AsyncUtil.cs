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

            if(!path.Contains("file://"))
                path = "file://" + path;
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(path);
            try
            {
                await req.SendWebRequest().WithCancellation(ct);

                if(req.result == UnityWebRequest.Result.Success)
                    tex = DownloadHandlerTexture.GetContent(req);
                else
                    Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, req.error);
            }
            catch(OperationCanceledException)
            {
                Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, "operation canceled");
                throw;
            }
            catch(Exception err)
            {
                Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, err.Message);
                throw;
            }
            finally
            {
                req.Dispose();
                req = null;
            }

            return tex;
        }

        public static async UniTask<Texture2D> LoadTextureWithOptionAsync(string path, TextureFormat texFormat, bool mipChain, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, FilterMode filterMode, CancellationToken ct = default)
        {
            Texture2D tex = null;

            if(!path.Contains("file://"))
                path = "file://" + path;
            UnityWebRequest req = UnityWebRequest.Get(path);
            try
            {
                await req.SendWebRequest().WithCancellation(ct);

                if(req.result == UnityWebRequest.Result.Success)
                {
                    tex = new Texture2D(2, 2, texFormat, mipChain);
                    tex.LoadImage(req.downloadHandler.data);
                    tex.wrapModeU = wrapModeU;
                    tex.wrapModeV = wrapModeV;
                    tex.filterMode = filterMode;
                    tex.Apply();
                }
                else
                    Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, req.error);
            }
            catch(OperationCanceledException)
            {
                Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, "operation canceled");
                throw;
            }
            catch(Exception err)
            {
                Debug.LogWarningFormat("texture load error: {0} [ {1} ]", path, err.Message);
                throw;
            }
            finally
            {
                req.Dispose();
                req = null;
            }

            return tex;
        }

        public static async UniTask<Texture2D[]> LoadTextureAllAsync(string[] paths, CancellationToken ct = default)
        {
            List<UniTask<Texture2D>> tasks = new List<UniTask<Texture2D>>();
            foreach(string path in paths)
                tasks.Add(LoadTextureAsync(path, ct));

            return await UniTask.WhenAll(tasks);
        }

        public static async UniTask<string> DownloadAsync(string url, CancellationToken ct = default)
        {
            string res = "";

            UnityWebRequest req = UnityWebRequest.Get(url);
            try
            {
                await req.SendWebRequest().WithCancellation(ct);

                if(req.result == UnityWebRequest.Result.Success)
                    res = req.downloadHandler.text;
                else
                    Debug.LogWarningFormat("download error: {0} [ {1} ]", url, req.error);
            }
            catch(OperationCanceledException)
            {
                Debug.LogWarningFormat("download error: {0} [ {1} ]", url, "operation canceled");
                throw;
            }
            catch(Exception err)
            {
                Debug.LogWarningFormat("download error: {0} [ {1} ]", url, err.Message);
                throw;
            }
            finally
            {
                req.Dispose();
                req = null;
            }

            return res;
        }

        public static async UniTask DownloadFileAsync(string url, string dstFile, CancellationToken ct = default)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            try
            {
                await req.SendWebRequest().WithCancellation(ct);

                if(req.result == UnityWebRequest.Result.Success)
                {
                    if(File.Exists(dstFile))
                        File.Delete(dstFile);
                    await File.WriteAllBytesAsync(dstFile, req.downloadHandler.data, ct);
                }
                else
                    Debug.LogWarningFormat("download error: {0} [ {1} ]", url, req.error);
            }
            catch(OperationCanceledException)
            {
                if(File.Exists(dstFile))
                    File.Delete(dstFile);

                Debug.LogWarningFormat("download error: {0} [ {1} ]", url, "operation canceled");
                throw;
            }
            catch(Exception err)
            {
                if(File.Exists(dstFile))
                    File.Delete(dstFile);

                Debug.LogWarningFormat("download error: {0} [ {1} ]", url, err.Message);
                throw;
            }
            finally
            {
                req.Dispose();
                req = null;
            }
        }

        public static async UniTask DownloadFileAllAsync(string[] urls, string[] dstFiles, CancellationToken ct = default)
        {
            if(urls.Length != dstFiles.Length)
                throw new Exception("download file all error: number of urls and files does not match");

            List<UniTask> tasks = new List<UniTask>();
            for(int i = 0; i < urls.Length; i++)
                tasks.Add(DownloadFileAsync(urls[i], dstFiles[i], ct));

            await UniTask.WhenAll(tasks);
        }
    }
}
