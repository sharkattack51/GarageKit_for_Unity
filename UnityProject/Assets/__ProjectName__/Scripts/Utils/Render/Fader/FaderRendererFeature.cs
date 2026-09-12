using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/*
 * Fader.csのフェード色を画面全体に描画するRendererFeature
 * URP(Render Graph)ではOnPostRenderのGL描画が反映されないための代替実装
 */
namespace GarageKit
{
    public class FaderRendererFeature : ScriptableRendererFeature
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        private FaderRenderPass faderPass;

        public override void Create()
        {
            faderPass = new FaderRenderPass(name);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            if(!Fader.UseFade)
                return;

            faderPass.renderPassEvent = renderPassEvent;
            renderer.EnqueuePass(faderPass);
        }

        protected override void Dispose(bool disposing)
        {
            faderPass?.Cleanup();

            base.Dispose(disposing);
        }

        private class FaderRenderPass : ScriptableRenderPass
        {
            private readonly Mesh quadMesh;
            private readonly Color[] quadColors = new Color[4];

            public FaderRenderPass(string passName)
            {
                profilingSampler = new ProfilingSampler(passName);

                quadMesh = new Mesh();
                quadMesh.hideFlags = HideFlags.HideAndDontSave;
                quadMesh.vertices = new Vector3[]
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f, 1.0f, 0.0f),
                    new Vector3(1.0f, 1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f)
                };
                quadMesh.uv = new Vector2[]
                {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f)
                };
                quadMesh.colors = new Color[] { Color.white, Color.white, Color.white, Color.white };
                quadMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            }

            public void Cleanup()
            {
                if(quadMesh != null)
                    Object.DestroyImmediate(quadMesh);
            }

            private static Fader FindFaderForCamera(Camera camera)
            {
                foreach(Fader fader in Fader.Faders)
                {
                    if(fader == null || !fader.isActiveAndEnabled)
                        continue;

                    if(fader.GetComponent<Camera>() == camera)
                        return fader;
                }

                return null;
            }

            private class PassData
            {
                internal Mesh mesh;
                internal Material material;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                Fader fader = FindFaderForCamera(cameraData.camera);
                if(fader == null)
                    return;

                Color color = fader.fadeMaterial.color;
                quadColors[0] = color;
                quadColors[1] = color;
                quadColors[2] = color;
                quadColors[3] = color;
                quadMesh.SetColors(quadColors);

                using(IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Fader", out PassData passData, profilingSampler))
                {
                    passData.mesh = quadMesh;
                    passData.material = fader.fadeMaterial;

                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => {
                        context.cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                        context.cmd.DrawMesh(data.mesh, Matrix4x4.identity, data.material, 0, 0);
                    });
                }
            }
        }
    }
}
