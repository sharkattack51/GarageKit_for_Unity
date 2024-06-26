using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;

namespace GarageKit.Localize
{
    public class LocalizeDynamicRawImage : MonoBehaviour, ILocalize
    {
        private static List<LocalizeDynamicRawImage> localizeList = new List<LocalizeDynamicRawImage>();
        public static async UniTask LocalizeAllAsync(LANGUAGE lang)
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach(LocalizeDynamicRawImage rawImg in localizeList)
                tasks.Add(rawImg.LocalizeAsync(lang));
            await UniTask.WhenAll(tasks);
        }

        public RawImage uiRawImage;
        public List<string> localizeTexturePaths;
        public LANGUAGE lang;

        private string loadedPath = "";


        void Awake()
        {
            localizeList.Add(this);

            if(uiRawImage == null)
                uiRawImage = this.gameObject.GetComponent<RawImage>();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        void OnDestroy()
        {
            localizeList.Remove(this);

            if(uiRawImage.texture != null)
                Texture2D.Destroy(uiRawImage.texture);
            uiRawImage.texture = null;
        }


        public void Localize(LANGUAGE lang)
        {
            Debug.LogWarning("this component requires dynamic loading texture");
        }

        public async UniTask LocalizeAsync(LANGUAGE lang)
        {
            this.lang = lang;

            if(localizeTexturePaths.Count == 1
                || localizeTexturePaths.Count > (int)lang)
            {
                string texPath = "";
                if(localizeTexturePaths.Count == 1)
                    texPath = localizeTexturePaths[0];
                else
                    texPath = localizeTexturePaths[(int)lang];

                if(uiRawImage.texture != null && texPath == loadedPath)
                    return;

                if(uiRawImage.texture != null)
                    Texture2D.Destroy(uiRawImage.texture);

                uiRawImage.texture = await AsyncUtil.LoadTextureAsync(texPath, this.GetCancellationTokenOnDestroy());
                loadedPath = texPath;
            }
        }

        public async UniTask TryCatchLocalizeAsync(LANGUAGE lang, Action<string> onCatch)
        {
            try
            {
                await LocalizeAsync(lang);
            }
            catch(Exception err)
            {
                onCatch?.Invoke(err.Message);
            }
        }
    }
}
