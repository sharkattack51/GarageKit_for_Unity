using UnityEngine;

public class Texture2DUtil
{
    public static void ResizeTexture2D(Texture2D src, ref Texture2D dst)
    {
        if(src == null || dst == null)
            return;

        RenderTexture prevActive = RenderTexture.active;
        RenderTexture rt = RenderTexture.GetTemporary(dst.width, dst.height, 0);
        try
        {
            Graphics.Blit(src, rt);
            RenderTexture.active = rt;
            dst.ReadPixels(new Rect(0, 0, dst.width, dst.height), 0, 0);
            dst.Apply();
        }
        finally
        {
            RenderTexture.active = prevActive;
            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
