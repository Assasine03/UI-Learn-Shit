using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

// Runs on camera color before the Screen Space Overlay canvases are drawn.
public class EscapeMenuBlurFeature : ScriptableRendererFeature
{
    [SerializeField] Material blurMaterial;
    BlurPass pass;

    public override void Create()
    {
        pass = new BlurPass { renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!EscapeMenuBackdrop.BlurActive || blurMaterial == null || !blurMaterial.shader.isSupported ||
            renderingData.cameraData.cameraType != CameraType.Game ||
            !renderingData.cameraData.resolveFinalTarget)
            return;
        pass.material = blurMaterial;
        renderer.EnqueuePass(pass);
    }

    class BlurPass : ScriptableRenderPass
    {
        public Material material;
        public BlurPass() { requiresIntermediateTexture = true; }

        public override void RecordRenderGraph(RenderGraph graph, ContextContainer frameData)
        {
            var resources = frameData.Get<UniversalResourceData>();
            if (resources.isActiveTargetBackBuffer) return;
            var source = resources.activeColorTexture;
            var description = graph.GetTextureDesc(source);
            description.clearBuffer = false;
            description.msaaSamples = MSAASamples.None;
            description.name = "ESC horizontal blur";
            var horizontal = graph.CreateTexture(description);
            description.name = "ESC vertical blur";
            var vertical = graph.CreateTexture(description);
            graph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, horizontal, material, 0),
                passName: "ESC horizontal blur");
            graph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(horizontal, vertical, material, 1),
                passName: "ESC vertical blur");
            resources.cameraColor = vertical;
        }
    }
}
