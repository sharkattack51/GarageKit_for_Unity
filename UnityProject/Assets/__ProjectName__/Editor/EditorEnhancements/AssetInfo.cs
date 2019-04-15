/*
 * Copyright (c) 2016 Tenebrous
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * Latest version: https://bitbucket.org/Tenebrous/unityeditorenhancements/wiki/Home
*/

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tenebrous.EditorEnhancements
{
    internal static class AssetInfo
    {
        public static string GetPreviewInfo(this object pObject)
        {
            string info = "";
            string typename = pObject.GetType().ToString().Replace("UnityEngine.", "");

            if (pObject is AudioClip)
                info = ((AudioClip) pObject).GetPreviewInfo();
            else if (pObject is Texture2D)
                info = ((Texture2D) pObject).GetPreviewInfo();
            else if (pObject is Material)
                info = ((Material) pObject).GetPreviewInfo();
            else if (pObject is Mesh)
                info = ((Mesh) pObject).GetPreviewInfo();
            else if (pObject is MeshFilter)
                info = ((MeshFilter) pObject).GetPreviewInfo();
            else if (pObject is MeshRenderer)
                info = ((MeshRenderer) pObject).GetPreviewInfo();
            else if (pObject is GameObject)
                info = ((GameObject) pObject).GetPreviewInfo();
            else if (pObject is MonoScript)
            {
                typename = "";
                info = ((MonoScript) pObject).GetPreviewInfo();
            }
            else if (pObject is Shader)
                info = ((Shader) pObject).GetPreviewInfo();
            else if (pObject is MonoBehaviour)
            {
                typename = pObject.GetType().BaseType.ToString().Replace("UnityEngine.", "");
                info = ((MonoBehaviour) pObject).GetPreviewInfo();
            }
            else if (pObject is Behaviour)
                info = ((Behaviour) pObject).GetPreviewInfo();

            if (typename != "")
                if (info == "")
                    info += typename;
                else
                    info += " (" + typename + ")";

            return (info);
        }

        public static string GetPreviewInfo(this AudioClip pObject)
        {
            string info = "";

            TimeSpan clipTimespan = TimeSpan.FromSeconds(pObject.length);
            if (clipTimespan.Hours > 0)
                info += clipTimespan.Hours + ":";

            info += clipTimespan.Minutes.ToString("00") + ":" + clipTimespan.Seconds.ToString("00") + "." +
                    clipTimespan.Milliseconds.ToString("000") + " ";

            if (pObject.channels == 1)
                info += "Mono\n";
            else if (pObject.channels == 2)
                info += "Stereo\n";
            else
                info += pObject.channels + " channels\n";

            return (info);
        }

        public static string GetPreviewInfo(this Texture2D pObject)
        {
            if( pObject != null )
                return pObject.format.ToString() + "\n"
                       + pObject.width + "x" + pObject.height;

            return "";
        }

        public static string GetPreviewInfo(this Material pObject)
        {
            string info = "";

            info = pObject.shader.name;

            foreach (Object obj in EditorUtility.CollectDependencies(new Object[] {pObject}))
                if (obj is Texture)
                    info += "\n - " + obj.name;

            return (info);
        }


        public static string GetPreviewInfo(this MeshFilter pObject)
        {
            if( pObject != null && pObject.sharedMesh != null )
                return pObject.sharedMesh.GetPreviewInfo();

            return "";
        }

        public static string GetPreviewInfo(this Mesh pObject)
        {
            if( pObject != null )
                return pObject.vertexCount + " verts " + pObject.triangles.Length + " tris"
                       + "\n" + pObject.name;

            return "";
        }

        public static string GetPreviewInfo(this MeshRenderer pObject)
        {
            return "";
        }

        public static string GetPreviewInfo(this MonoScript pObject)
        {
            if( pObject != null )
            {
                Type assetclass = pObject.GetClass();
                if( assetclass != null )
                    return assetclass.ToString() + "\n(" + assetclass.BaseType.ToString().Replace( "UnityEngine.", "" ) + ")";
                else
                    return "(multiple classes)";
            }

            return "";
        }

        public static string GetPreviewInfo(this Shader pObject)
        {
            if( pObject != null )
                return pObject.renderQueue.ToString();

            return "";
        }

        public static string GetPreviewInfo(this MonoBehaviour pObject)
        {
            if( pObject != null )
            {
                MonoScript ms = MonoScript.FromMonoBehaviour( pObject );
                if( ms != null )
                    return ms.name;
            }

            return "";
        }

        public static string GetPreviewInfo(this GameObject pObject)
        {
            if( pObject != null )
            {
                GameObject parent = PrefabUtility.GetCorrespondingObjectFromSource( pObject ) as GameObject;
                if( parent != null )
                    return AssetDatabase.GetAssetPath( parent );
            }

            return "";
        }


        // components

        public static string GetPreviewInfo(this Behaviour pObject)
        {
            if( pObject == null )
                return "";
            else if (pObject is Light)
                return (pObject as Light).GetPreviewInfo();
            else if (pObject is Camera)
                return (pObject as Camera).GetPreviewInfo();
            else if (pObject is AudioSource)
                return (pObject as AudioSource).GetPreviewInfo();
            else if (pObject is AudioReverbFilter)
                return (pObject as AudioReverbFilter).GetPreviewInfo();

            return "";
        }

        public static string GetPreviewInfo(this Light pObject)
        {
            if( pObject != null )
                return pObject.type.ToString();

            return "";
        }

        public static string GetPreviewInfo(this Camera pObject)
        {
            if( pObject != null )
            {
                if( pObject.orthographic )
                    return "Orthographic";
                else
                    return "Perspective";
            }

            return "";
        }

        public static string GetPreviewInfo(this AudioSource pObject)
        {
            if( pObject != null && pObject.clip != null )
                return pObject.clip.name;

            return "";
        }

        public static string GetPreviewInfo(this AudioReverbFilter pObject)
        {
            if( pObject != null )
                return pObject.reverbPreset.ToString();

            return "";
        }

        // other extensions

        public static bool HasAnyRenderers(this GameObject pObject)
        {
            if( pObject == null )
                return false;

            if (pObject.GetComponent<Renderer>() != null)
                return (true);

            foreach (Transform child in pObject.transform)
                if (child.gameObject.HasAnyRenderers())
                    return (true);

            return false;
        }
    }
}