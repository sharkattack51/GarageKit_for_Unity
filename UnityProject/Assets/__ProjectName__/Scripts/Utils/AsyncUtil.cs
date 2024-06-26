using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}
