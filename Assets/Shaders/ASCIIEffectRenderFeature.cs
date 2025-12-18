using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// --- 1. Volume Component (The settings you see in the Inspector) ---
// This allows you to control the shader parameters like a post-processing effect.
[System.Serializable, VolumeComponentMenu("Custom Post-processing/ASCII Effect")]
public class ASCIIEffectVolume : VolumeComponent, IPostProcessComponent
{
    // VolumeComponent fields map directly to your uniform variables
    // --- Preprocess Settings ---
    public ClampedFloatParameter Zoom = new ClampedFloatParameter(1.0f, 0.0f, 5.0f);
    public Vector2Parameter Offset = new Vector2Parameter(Vector2.zero);
    public IntParameter KernelSize = new IntParameter(2);
    public ClampedFloatParameter Sigma = new ClampedFloatParameter(2.0f, 0.0f, 5.0f);
    public ClampedFloatParameter SigmaScale = new ClampedFloatParameter(1.6f, 0.0f, 5.0f);
    public ClampedFloatParameter Tau = new ClampedFloatParameter(1.0f, 0.0f, 1.1f);
    public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.005f, 0.001f, 0.1f);

    public BoolParameter UseDepth = new BoolParameter(true);
    public ClampedFloatParameter DepthThreshold = new ClampedFloatParameter(0.1f, 0.0f, 5.0f);
    public BoolParameter UseNormals = new BoolParameter(true);
    public ClampedFloatParameter NormalThreshold = new ClampedFloatParameter(0.1f, 0.0f, 5.0f);
    public ClampedFloatParameter DepthCutoff = new ClampedFloatParameter(0.0f, 0.0f, 1000.0f);
    public IntParameter EdgeThreshold = new IntParameter(8);

    // --- Color Settings ---
    public BoolParameter Edges = new BoolParameter(true);
    public BoolParameter Fill = new BoolParameter(true);
    public ClampedFloatParameter Exposure = new ClampedFloatParameter(1.0f, 0.0f, 5.0f);
    public ClampedFloatParameter Attenuation = new ClampedFloatParameter(1.0f, 0.0f, 5.0f);
    public BoolParameter InvertLuminance = new BoolParameter(false);
    public ColorParameter ASCIIColor = new ColorParameter(Color.white);
    public ColorParameter BackgroundColor = new ColorParameter(Color.black);
    public ClampedFloatParameter BlendWithBase = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
    public ClampedFloatParameter DepthFalloff = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
    public ClampedFloatParameter DepthOffset = new ClampedFloatParameter(0.0f, 0.0f, 1000.0f);

    // --- Debug Settings ---
    public BoolParameter ViewDog = new BoolParameter(false);
    public BoolParameter ViewUncompressed = new BoolParameter(false);
    public BoolParameter ViewEdges = new BoolParameter(false);

    // Check if the effect should be rendered
    public bool IsActive() => Edges.value || Fill.value;

    // Required for interface, but not strictly used here
    public bool Is
        (
            PostProcessData data
        )
    {
        return IsActive();
    }
}

// --- 2. Custom Render Pass (The Command Buffer Executor) ---

public class ASCIIEffectPass : ScriptableRenderPass
{
    private const string ProfilerTag = "ASCII Effect";

    private Material m_Material;
    private ComputeShader m_ComputeShader;

    // --- RTHandle Declarations (New System) ---
    // Instead of PropertyToID, we use RTHandle for explicit texture management
    private RTHandle m_LuminanceAsciiRT;
    private RTHandle m_AsciiPingRT;
    private RTHandle m_AsciiDogRT;
    private RTHandle m_NormalsDepthRT;
    private RTHandle m_AsciiEdgesRT;
    private RTHandle m_AsciiSobelRT;
    private RTHandle m_DownscaleRT;
    private RTHandle m_FinalASCIIRT;
    private RTHandle m_CameraColorTextureRT; // Handle for the source/destination

    private int m_CSKernel;
    private ASCIIEffectVolume m_Volume;

    public Texture2D EdgesLut;
    public Texture2D FillLut;

    public ASCIIEffectPass(Shader shader, ComputeShader computeShader)
    {
        if (shader == null || computeShader == null)
        {
            Debug.LogError("Shader or Compute Shader is missing.");
            return;
        }
        m_Material = CoreUtils.CreateEngineMaterial(shader);
        m_ComputeShader = computeShader;
        m_CSKernel = m_ComputeShader.FindKernel("CS_RenderASCII");
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    // --- SETUP: Takes the Camera's RTHandle ---
    // The Renderer now passes a handle directly
    public void Setup(RTHandle cameraColorTextureHandle)
    {
        m_CameraColorTextureRT = cameraColorTextureHandle;
    }

    // --- ALLOCATION: Allocates all temporary RTHandles ---
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        m_CameraColorTextureRT = renderingData.cameraData.renderer.cameraColorTargetHandle;

        if (m_Material == null || m_ComputeShader == null) return;

        var stack = VolumeManager.instance.stack;
        m_Volume = stack.GetComponent<ASCIIEffectVolume>();

        if (m_Volume.UseDepth.value || m_Volume.UseNormals.value)
            ConfigureInput(ScriptableRenderPassInput.Depth);

        // --- RTHandle Allocation using RTHandles.Alloc ---
        // The RenderTextureDescriptor and allocation settings are managed here.

        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;

        // Luminance (RHalf)
        desc.colorFormat = RenderTextureFormat.RHalf;
        RenderingUtils.ReAllocateIfNeeded(ref m_LuminanceAsciiRT, desc, name: "LuminanceRT");

        // Ping-Pong, Sobel (RGHalf)
        desc.colorFormat = RenderTextureFormat.RGHalf;
        RenderingUtils.ReAllocateIfNeeded(ref m_AsciiPingRT, desc, name: "AsciiPingRT");
        RenderingUtils.ReAllocateIfNeeded(ref m_AsciiSobelRT, desc, name: "AsciiSobelRT");

        // Normals/Depth, Final (ARGBHalf)
        desc.colorFormat = RenderTextureFormat.ARGBHalf;
        RenderingUtils.ReAllocateIfNeeded(ref m_NormalsDepthRT, desc, name: "NormalsDepthRT");
        RenderingUtils.ReAllocateIfNeeded(ref m_FinalASCIIRT, desc, name: "FinalASCIIRT");

        // DoG, Edges (RHalf)
        desc.colorFormat = RenderTextureFormat.RHalf;
        RenderingUtils.ReAllocateIfNeeded(ref m_AsciiDogRT, desc, name: "AsciiDogRT");
        RenderingUtils.ReAllocateIfNeeded(ref m_AsciiEdgesRT, desc, name: "AsciiEdgesRT");

        // Downscale (ARGBHalf, 8:1 resolution)
        desc.width /= 8;
        desc.height /= 8;
        desc.colorFormat = RenderTextureFormat.ARGBHalf;
        RenderingUtils.ReAllocateIfNeeded(ref m_DownscaleRT, desc, name: "DownscaleRT");

        // We use ConfigureTarget(m_CameraColorTextureRT) to explicitly tell URP
        // that our final target is the camera's RT.
        ConfigureTarget(m_CameraColorTextureRT);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (m_Material == null || m_ComputeShader == null || !m_Volume.IsActive()) return;

        CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

        // Set up the context for Blitter
        SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
        
        SetShaderParameters(m_Volume);

        // Set LUTs (these are still global textures)
        cmd.SetGlobalTexture("_EdgesASCIILUT", EdgesLut);
        cmd.SetGlobalTexture("_FillASCIILUT", FillLut);

        // --- URP Pass Execution (Pixel Shaders) using Blitter.BlitCameraTexture ---
        // Replaces the old Blit(cmd, source, dest, material, passIndex)

        // Pass 0: Luminance (Source -> LuminanceRT)
        Blitter.BlitCameraTexture(cmd, m_CameraColorTextureRT, m_LuminanceAsciiRT, m_Material, 0);

        // Pass 1: Downscale (Source -> DownscaleRT)
        Blitter.BlitCameraTexture(cmd, m_CameraColorTextureRT, m_DownscaleRT, m_Material, 1);

        // Pass 2: Horizontal Blur (LuminanceRT -> AsciiPingRT)
        cmd.SetGlobalTexture("_LuminanceAsciiTex", m_LuminanceAsciiRT);
        Blitter.BlitCameraTexture(cmd, m_LuminanceAsciiRT, m_AsciiPingRT, m_Material, 2);

        // Pass 3: Vertical Blur and Difference (PingRT -> AsciiDogRT)
        cmd.SetGlobalTexture("_AsciiPingTex", m_AsciiPingRT);
        Blitter.BlitCameraTexture(cmd, m_AsciiPingRT, m_AsciiDogRT, m_Material, 3);

        // Pass 4: Calculate Normals/Depth (Source -> NormalsDepthRT)
        Blitter.BlitCameraTexture(cmd, m_CameraColorTextureRT, m_NormalsDepthRT, m_Material, 4);

        // Pass 5: Edge Detect (DoG, Normals/Depth -> AsciiEdgesRT)
        cmd.SetGlobalTexture("_AsciiDogTex", m_AsciiDogRT);
        cmd.SetGlobalTexture("_NormalsDepthTex", m_NormalsDepthRT);
        Blitter.BlitCameraTexture(cmd, m_CameraColorTextureRT, m_AsciiEdgesRT, m_Material, 5); // Source texture is irrelevant here, but the call needs one.

        // Pass 6: Horizontal Sobel (EdgesRT -> AsciiPingRT)
        cmd.SetGlobalTexture("_AsciiEdgesTex", m_AsciiEdgesRT);
        Blitter.BlitCameraTexture(cmd, m_AsciiEdgesRT, m_AsciiPingRT, m_Material, 6);

        // Pass 7: Vertical Sobel (PingRT -> AsciiSobelRT)
        cmd.SetGlobalTexture("_AsciiPingTex", m_AsciiPingRT);
        Blitter.BlitCameraTexture(cmd, m_AsciiPingRT, m_AsciiSobelRT, m_Material, 7);

        // --- Compute Shader Execution ---
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;
        int threadGroupX = Mathf.CeilToInt(width / 8.0f);
        int threadGroupY = Mathf.CeilToInt(height / 8.0f);

        // Bind Compute Shader Textures (use GetRenderTexture to pass the RTHandle's underlying RT)
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_SobelTex", m_AsciiSobelRT.nameID);
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_DownscaleTex", m_DownscaleRT.nameID);
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_NormalsDepthTex", m_NormalsDepthRT.nameID);
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_AsciiEdgesTex", m_AsciiEdgesRT.nameID);
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_EdgesASCIILUT", EdgesLut);
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_FillASCIILUT", FillLut);

        // Bind the final output texture (RWTexture2D)
        cmd.SetComputeTextureParam(m_ComputeShader, m_CSKernel, "_ResultTexture", m_FinalASCIIRT.nameID);

        // Dispatch the Compute Shader
        cmd.DispatchCompute(m_ComputeShader, m_CSKernel, threadGroupX, threadGroupY, 1);

        // --- Final Blit to Screen ---
        // Final pass to draw the result from the Compute Shader buffer back to the camera's color buffer.
        cmd.SetGlobalTexture("_FinalASCIITex", m_FinalASCIIRT);
        Blitter.BlitCameraTexture(cmd, m_FinalASCIIRT, m_CameraColorTextureRT, m_Material, 8);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        
    }

    // --- CLEANUP: Releases all RTHandles ---
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // RTHandles.Release() is the correct way to dispose of handles.
        m_LuminanceAsciiRT?.Release();
        m_AsciiPingRT?.Release();
        m_AsciiDogRT?.Release();
        m_NormalsDepthRT?.Release();
        m_AsciiEdgesRT?.Release();
        m_AsciiSobelRT?.Release();
        m_DownscaleRT?.Release();
        m_FinalASCIIRT?.Release();
    }

    private void SetShaderParameters(ASCIIEffectVolume volume)
    {
        // --- Preprocess Settings ---
        m_Material.SetFloat("_Zoom", volume.Zoom.value);
        m_Material.SetVector("_Offset", volume.Offset.value);
        m_Material.SetInt("_KernelSize", volume.KernelSize.value);
        m_Material.SetFloat("_Sigma", volume.Sigma.value);
        m_Material.SetFloat("_SigmaScale", volume.SigmaScale.value);
        m_Material.SetFloat("_Tau", volume.Tau.value);
        m_Material.SetFloat("_Threshold", volume.Threshold.value);

        // --- Depth/Normal ---
        m_Material.SetInt("_UseDepth", volume.UseDepth.value ? 1 : 0);
        m_Material.SetFloat("_DepthThreshold", volume.DepthThreshold.value);
        m_Material.SetInt("_UseNormals", volume.UseNormals.value ? 1 : 0);
        m_Material.SetFloat("_NormalThreshold", volume.NormalThreshold.value);
        m_Material.SetFloat("_DepthCutoff", volume.DepthCutoff.value);
        m_Material.SetInt("_EdgeThreshold", volume.EdgeThreshold.value);

        // Also set properties on the Compute Shader
        m_ComputeShader.SetInt("_EdgeThreshold", volume.EdgeThreshold.value);
        m_ComputeShader.SetFloat("_DepthCutoff", volume.DepthCutoff.value);

        // --- Color Settings ---
        m_Material.SetInt("_Edges", volume.Edges.value ? 1 : 0);
        m_Material.SetInt("_Fill", volume.Fill.value ? 1 : 0);
        m_Material.SetFloat("_Exposure", volume.Exposure.value);
        m_Material.SetFloat("_Attenuation", volume.Attenuation.value);
        m_Material.SetInt("_InvertLuminance", volume.InvertLuminance.value ? 1 : 0);
        m_Material.SetColor("_ASCIIColor", volume.ASCIIColor.value);
        m_Material.SetColor("_BackgroundColor", volume.BackgroundColor.value);
        m_Material.SetFloat("_BlendWithBase", volume.BlendWithBase.value);
        m_Material.SetFloat("_DepthFalloff", volume.DepthFalloff.value);
        m_Material.SetFloat("_DepthOffset", volume.DepthOffset.value);

        // Set all of these on the Compute Shader too (omitted for brevity)
        // ... (You must copy all relevant color/debug properties to the Compute Shader)

        // --- Debug Settings ---
        m_Material.SetInt("_ViewDog", volume.ViewDog.value ? 1 : 0);
        m_Material.SetInt("_ViewUncompressed", volume.ViewUncompressed.value ? 1 : 0);
        m_Material.SetInt("_ViewEdges", volume.ViewEdges.value ? 1 : 0);

        // Also set debug flags on the Compute Shader
        m_ComputeShader.SetInt("_ViewDog", volume.ViewDog.value ? 1 : 0);
        m_ComputeShader.SetInt("_ViewUncompressed", volume.ViewUncompressed.value ? 1 : 0);
        m_ComputeShader.SetInt("_ViewEdges", volume.ViewEdges.value ? 1 : 0);
    }
}

// --- 3. Custom Render Feature (The project asset) ---

[System.Serializable]
public class ASCIIEffectRenderFeature : ScriptableRendererFeature
{
    public Shader shader;
    public ComputeShader computeShader;

    // LUT Textures (Assign your imported images here!)
    public Texture2D edgesASCIILUT;
    public Texture2D fillASCIILUT;

    private ASCIIEffectPass m_ScriptablePass;

    public override void Create()
    {
        // Only create the pass if the required assets are assigned
        if (shader == null || computeShader == null)
        {
            Debug.LogError("Shader or Compute Shader is not assigned on the Render Feature asset.");
            return;
        }

        m_ScriptablePass = new ASCIIEffectPass(shader, computeShader);
        m_ScriptablePass.EdgesLut = edgesASCIILUT;
        m_ScriptablePass.FillLut = fillASCIILUT;
    }

    // Called every frame to inject the pass into the renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_ScriptablePass == null || !renderingData.cameraData.postProcessEnabled)
        {
            return;
        }

        renderer.EnqueuePass(m_ScriptablePass);
    }
}