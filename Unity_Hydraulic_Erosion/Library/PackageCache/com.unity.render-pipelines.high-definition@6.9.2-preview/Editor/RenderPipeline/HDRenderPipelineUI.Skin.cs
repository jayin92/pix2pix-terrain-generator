using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.HDPipeline.RenderPipelineSettings;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    static partial class HDRenderPipelineUI
    {
        static readonly GUIContent k_GeneralSectionTitle = EditorGUIUtility.TrTextContent("General");
        static readonly GUIContent k_RenderingSectionTitle = EditorGUIUtility.TrTextContent("Rendering");
        static readonly GUIContent k_LightingSectionTitle = EditorGUIUtility.TrTextContent("Lighting");
        static readonly GUIContent k_MaterialSectionTitle = EditorGUIUtility.TrTextContent("Material");
        static readonly GUIContent k_PostProcessSectionTitle = EditorGUIUtility.TrTextContent("Post-processing");
        static readonly GUIContent k_LightLoopSubTitle = EditorGUIUtility.TrTextContent("Lights");

        static readonly GUIContent k_CookiesSubTitle = EditorGUIUtility.TrTextContent("Cookies");
        static readonly GUIContent k_ReflectionsSubTitle = EditorGUIUtility.TrTextContent("Reflections");
        static readonly GUIContent k_SkySubTitle = EditorGUIUtility.TrTextContent("Sky");
        static readonly GUIContent k_DecalsSubTitle = EditorGUIUtility.TrTextContent("Decals");
        static readonly GUIContent k_DecalsMetalAndAOSubTitle = EditorGUIUtility.TrTextContent("Decals Metal And AO");
        static readonly GUIContent k_ShadowSubTitle = EditorGUIUtility.TrTextContent("Shadow");
        static readonly GUIContent k_ShadowPunctualLightAtlasSubTitle = EditorGUIUtility.TrTextContent("Punctual Lights Atlas");
        static readonly GUIContent k_ScreenSpaceShadowsTitle = EditorGUIUtility.TrTextContent("Screen Space Shadows");
        static readonly GUIContent k_ShadowAreaLightAtlasSubTitle = EditorGUIUtility.TrTextContent("Area Lights Atlas");
        static readonly GUIContent k_DynamicResolutionSubTitle = EditorGUIUtility.TrTextContent("Dynamic resolution");
        static readonly GUIContent k_LowResTransparencySubTitle = EditorGUIUtility.TrTextContent("Low res Transparency");

        static readonly GUIContent k_DefaultFrameSettingsContent = EditorGUIUtility.TrTextContent("Default Frame Settings For");

        static readonly GUIContent k_RenderPipelineResourcesContent = EditorGUIUtility.TrTextContent("Render Pipeline Resources", "Set of resources that need to be loaded when creating stand alone");
        static readonly GUIContent k_RenderPipelineRayTracingResourcesContent = EditorGUIUtility.TrTextContent("Render Pipeline Ray Tracing Resources", "Set of resources that need to be loaded when using ray tracing");
        static readonly GUIContent k_RenderPipelineEditorResourcesContent = EditorGUIUtility.TrTextContent("Render Pipeline Editor Resources", "Set of resources that need to be loaded for working in editor");
        static readonly GUIContent k_DiffusionProfileSettingsContent = EditorGUIUtility.TrTextContent("Diffusion Profile List");
        static readonly GUIContent k_SRPBatcher = EditorGUIUtility.TrTextContent("SRP Batcher", "When enabled, the render pipeline uses the SRP batcher.");
        static readonly GUIContent k_ShaderVariantLogLevel = EditorGUIUtility.TrTextContent("Shader Variant Log Level", "Controls the level logging in of shader variants information is outputted when a build is performed. Information appears in the Unity Console when the build finishes.");

        static readonly GUIContent k_SupportShadowMaskContent = EditorGUIUtility.TrTextContent("Shadow Mask", "When enabled, HDRP allocates Shader variants and memory for processing shadow masks. This allows you to use shadow masks in your Unity Project.");
        static readonly GUIContent k_SupportSSRContent = EditorGUIUtility.TrTextContent("Screen Space Reflection", "When enabled, HDRP allocates memory for processing screen space reflection (SSR). This allows you to use SSR in your Unity Project.");
        static readonly GUIContent k_SupportSSAOContent = EditorGUIUtility.TrTextContent("Screen Space Ambient Occlusion", "When enabled, HDRP allocates memory for processing screen space ambient occlusion (SSAO). This allows you to use SSAO in your Unity Project.");
        static readonly GUIContent k_SupportedSSSContent = EditorGUIUtility.TrTextContent("Subsurface Scattering", "When enabled, HDRP allocates memory for processing subsurface scattering (SSS). This allows you to use SSS in your Unity Project.");
        static readonly GUIContent k_SSSSampleCountContent = EditorGUIUtility.TrTextContent("High Quality ", "When enabled, HDRP processes higher quality subsurface scattering effects. Warning: There is a high performance cost, do not enable on consoles.");
        static readonly GUIContent k_SupportVolumetricContent = EditorGUIUtility.TrTextContent("Volumetrics", "When enabled, HDRP allocates Shader variants and memory for volumetric effects. This allows you to use volumetric lighting and fog in your Unity Project.");
        static readonly GUIContent k_VolumetricResolutionContent = EditorGUIUtility.TrTextContent("High Quality ", "When enabled, HDRP increases the resolution of volumetric lighting buffers. Warning: There is a high performance cost, do not enable on consoles.");
        static readonly GUIContent k_SupportLightLayerContent = EditorGUIUtility.TrTextContent("Light Layers", "When enabled, HDRP allocates memory for processing Light Layers. This allows you to use Light Layers in your Unity Project. For deferred rendering, this allocation includes an extra render target in memory and extra cost.");
        static readonly GUIContent k_ColorBufferFormatContent = EditorGUIUtility.TrTextContent("Color Buffer Format", "Specifies the format used by the scene color render target. R11G11B10 is a faster option and should have sufficient precision.");
        static readonly GUIContent k_SupportLitShaderModeContent = EditorGUIUtility.TrTextContent("Lit Shader Mode", "Specifies the rendering modes HDRP supports for Lit Shaders. HDRP removes all allocated memory and Shader variants for modes you do not specify.");
        static readonly GUIContent k_MSAASampleCountContent = EditorGUIUtility.TrTextContent("Multisample Anti-aliasing Quality", "Specifies the maximum quality HDRP supports for MSAA. Set Lit Shader Mode to Forward Only or Both to use this feature.");
        static readonly GUIContent k_SupportDecalContent = EditorGUIUtility.TrTextContent("Enable", "When enabled, HDRP allocates Shader variants and memory to the decals buffer and cluster decal. This allows you to use decals in your Unity Project.");
        static readonly GUIContent k_SupportMotionVectorContent = EditorGUIUtility.TrTextContent("Motion Vectors", "When enabled, HDRP allocates memory for processing motion vectors which it uses for Motion Blur, TAA, and temporal re-projection of various effect like SSR.");
        static readonly GUIContent k_SupportRuntimeDebugDisplayContent = EditorGUIUtility.TrTextContent("Runtime Debug Display", "When disabled, HDRP removes all debug display Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportDitheringCrossFadeContent = EditorGUIUtility.TrTextContent("Dithering Cross-fade", "When disabled, HDRP removes all dithering cross fade Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportDistortion = EditorGUIUtility.TrTextContent("Distortion", "When disabled, HDRP removes all distortion Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportTransparentBackface = EditorGUIUtility.TrTextContent("Transparent Backface", "When disabled, HDRP removes all transparent backface Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportTransparentDepthPrepass = EditorGUIUtility.TrTextContent("Transparent Depth Prepass", "When disabled, HDRP removes all transparent depth prepass Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportTransparentDepthPostpass = EditorGUIUtility.TrTextContent("Transparent Depth Postpass", "When disabled, HDRP removes all transparent depth postpass Shader variants when you build for the Unity Player. This decreases build time.");
        static readonly GUIContent k_SupportRaytracing = EditorGUIUtility.TrTextContent("Realtime Raytracing");
        static readonly GUIContent k_RaytracingTier = EditorGUIUtility.TrTextContent("Raytracing Tier");

        const string k_CacheErrorFormat = "This configuration will lead to more than 2 GB reserved for this cache at runtime! ({0} requested) Only {1} element will be reserved instead.";
        const string k_CacheInfoFormat = "Reserving {0} in memory at runtime.";
        const string k_MultipleDifferenteValueMessage = "Multiple different values";

        static readonly GUIContent k_CoockieSizeContent = EditorGUIUtility.TrTextContent("Cookie Size", "Specifies the maximum size for the individual 2D cookies that HDRP uses for Directional and Spot Lights.");
        static readonly GUIContent k_CookieTextureArraySizeContent = EditorGUIUtility.TrTextContent("Texture Array Size", "Sets the maximum Texture Array size for the 2D cookies HDRP uses for Directional and Spot Lights. Higher values allow HDRP to use more cookies concurrently on screen.");
        static readonly GUIContent k_PointCoockieSizeContent = EditorGUIUtility.TrTextContent("Point Cookie Size", "Specifies the maximum size for the Cube cookes HDRP uses for Point Lights.");
        static readonly GUIContent k_PointCookieTextureArraySizeContent = EditorGUIUtility.TrTextContent("Cubemap Array Size", "Sets the maximum Texture Array size for the Cube cookies HDRP uses for Directional and Spot Lights. Higher values allow HDRP to use more cookies concurrently on screen.");


        static readonly GUIContent k_CompressProbeCacheContent = EditorGUIUtility.TrTextContent("Compress Reflection Probe Cache", "When enabled, HDRP compresses the Reflection Probe cache to save disk space.");
        static readonly GUIContent k_CubemapSizeContent = EditorGUIUtility.TrTextContent("Reflection Cubemap Size", "Specifies the maximum resolution of the individual Reflection Probe cube maps.");
        static readonly GUIContent k_ProbeCacheSizeContent = EditorGUIUtility.TrTextContent("Probe Cache Size", "Sets the maximum size of the Probe Cache.");

        static readonly GUIContent k_CompressPlanarProbeCacheContent = EditorGUIUtility.TrTextContent("Compress Planar Reflection Probe Cache", "When enabled, HDRP compresses the Planar Reflection Probe cache to save disk space.");
        static readonly GUIContent k_PlanarTextureSizeContent = EditorGUIUtility.TrTextContent("Planar Reflection Texture Size", "Specifies the maximum resolution of Planar Reflection Textures.");
        static readonly GUIContent k_PlanarProbeCacheSizeContent = EditorGUIUtility.TrTextContent("Planar Probe Cache Size", "Sets the maximum size of the Planar Probe Cache.");

        static readonly GUIContent k_SupportFabricBSDFConvolutionContent = EditorGUIUtility.TrTextContent("Fabric BSDF Convolution", "When enabled, HDRP calculates a separate version of each Reflection Probe for the Fabric Shader, creating more accurate lighting effects. See the documentation for more information and limitations of this feature.");

        static readonly GUIContent k_SkyReflectionSizeContent = EditorGUIUtility.TrTextContent("Reflection Size", "Specifies the maximum resolution of the cube map HDRP uses to represent the sky.");
        static readonly GUIContent k_SkyLightingOverrideMaskContent = EditorGUIUtility.TrTextContent("Lighting Override Mask", "Specifies the layer mask HDRP uses to override sky lighting.");
        const string k_SkyLightingHelpBoxContent = "Be careful, Sky Lighting Override Mask is set to Everything. This is most likely a mistake as it serves no purpose.";

        static readonly GUIContent k_MaxDirectionalContent = EditorGUIUtility.TrTextContent("Maximum Directional on Screen", "Sets the maximum number of Directional Lights HDRP can handle on screen at once.");
        static readonly GUIContent k_MaxPonctualContent = EditorGUIUtility.TrTextContent("Maximum Punctual on Screen", "Sets the maximum number of Point and Spot Lights HDRP can handle on screen at once.");
        static readonly GUIContent k_MaxAreaContent = EditorGUIUtility.TrTextContent("Maximum Area on Screen", "Sets the maximum number of area Lights HDRP can handle on screen at once.");
        static readonly GUIContent k_MaxEnvContent = EditorGUIUtility.TrTextContent("Maximum Environment Lights on Screen", "Sets the maximum number of environment Lights HDRP can handle on screen at once.");
        static readonly GUIContent k_MaxDecalContent = EditorGUIUtility.TrTextContent("Maximum Decals on Screen", "Sets the maximum number of Decals HDRP can handle on screen at once.");

        static readonly GUIContent k_ResolutionContent = EditorGUIUtility.TrTextContent("Resolution", "Specifies the resolution of the shadow Atlas.");
        static readonly GUIContent k_DirectionalShadowPrecisionContent = EditorGUIUtility.TrTextContent("Directional Shadow Precision", "Select the shadow map bit depth, this forces HDRP to use selected bit depth for shadow maps.");
        static readonly GUIContent k_PrecisionContent = EditorGUIUtility.TrTextContent("Precision", "Select the shadow map bit depth, this forces HDRP to use selected bit depth for shadow maps.");
        static readonly GUIContent k_DynamicRescaleContent = EditorGUIUtility.TrTextContent("Dynamic Rescale", "When enabled, scales the shadow map size using the screen size of the Light to leave more space for other shadows in the atlas.");
        static readonly GUIContent k_MaxRequestContent = EditorGUIUtility.TrTextContent("Maximum Shadows on Screen", "Sets the maximum number of shadows HDRP can handle on screen at once. See the documentation for details on how many shadows each light type casts.");

        static readonly GUIContent k_SupportScreenSpaceShadows = EditorGUIUtility.TrTextContent("Support Screen Space Shadows", "Enables the support of screen space shadows in HDRP.");
        static readonly GUIContent k_MaxScreenSpaceShadows = EditorGUIUtility.TrTextContent("Maximum Screen Space Shadows", "Sets the maximum number of screen space shadows HDRP can handle on screen at once.");

        static readonly GUIContent k_DrawDistanceContent = EditorGUIUtility.TrTextContent("Draw Distance", "Sets the maximum distance from the Camera at which HDRP draws Decals.");
        static readonly GUIContent k_AtlasWidthContent = EditorGUIUtility.TrTextContent("Atlas Width", "Sets the width of the Decal Atlas.");
        static readonly GUIContent k_AtlasHeightContent = EditorGUIUtility.TrTextContent("Atlas Height", "Sets the height of the Decal Atlas.");
        static readonly GUIContent k_MetalAndAOContent = EditorGUIUtility.TrTextContent("Metal and Ambient Occlusion Properties", "When enabled, Decals affect metal and ambient occlusion properties.");
        static readonly GUIContent k_FilteringQuality = EditorGUIUtility.TrTextContent("Filtering Qualities", "Specifies the quality of shadows. See the documentation for details on the algorithm HDRP uses for each preset. (Unsupported in Deferred Only)");

        static readonly GUIContent k_Enabled = EditorGUIUtility.TrTextContent("Enable", "When enabled, HDRP dynamically lowers the resolution of render targets to reduce the workload on the GPU.");
        static readonly GUIContent k_MaxPercentage = EditorGUIUtility.TrTextContent("Maximum Screen Percentage", "Sets the maximum screen percentage that dynamic resolution can reach.");
        static readonly GUIContent k_MinPercentage = EditorGUIUtility.TrTextContent("Minimum Screen Percentage", "Sets the minimum screen percentage that dynamic resolution can reach.");
        static readonly GUIContent k_DynResType = EditorGUIUtility.TrTextContent("Dynamic Resolution Type", "Specifies the type of dynamic resolution that HDRP uses.");
        static readonly GUIContent k_UpsampleFilter = EditorGUIUtility.TrTextContent("Upscale Filter", "Specifies the filter that HDRP uses for upscaling.");
        static readonly GUIContent k_ForceScreenPercentage = EditorGUIUtility.TrTextContent("Force Screen Percentage", "When enabled, HDRP uses the Forced Screen Percentage value as the screen percentage.");
        static readonly GUIContent k_ForcedScreenPercentage = EditorGUIUtility.TrTextContent("Forced Screen Percentage", "Sets a specific screen percentage value. HDRP forces this screen percentage for dynamic resolution.");

        static readonly GUIContent k_LowResTransparentEnabled = EditorGUIUtility.TrTextContent("Enable", "When enabled, materials tagged as Low Res Transparent, will be rendered in a quarter res offscreen buffer and then composited to full res.");
        static readonly GUIContent k_CheckerboardDepthBuffer = EditorGUIUtility.TrTextContent("Checkerboarded depth buffer downsample", "When enabled, the depth buffer used for low res transparency is generated in a min/max checkerboard pattern from original full res buffer.");
        static readonly GUIContent k_LowResTranspUpsample = EditorGUIUtility.TrTextContent("Upsample type", "The type of upsampling filter used to composite the low resolution transparency.");

        static readonly GUIContent k_LutSize = EditorGUIUtility.TrTextContent("Grading LUT Size", "Sets size of the internal and external color grading lookup textures (LUTs).");
        static readonly GUIContent k_LutFormat = EditorGUIUtility.TrTextContent("Grading LUT Format", "Specifies the encoding format for color grading lookup textures. Lower precision formats are faster and use less memory at the expense of color precision.");

        static readonly GUIContent[] k_ShadowBitDepthNames = { new GUIContent("32 bit"),  new GUIContent("16 bit") };
        static readonly int[] k_ShadowBitDepthValues = { (int) DepthBits.Depth32, (int) DepthBits.Depth16};

        const string memoryDrawback = "Adds GPU memory";
        const string shaderVariantDrawback = "Adds Shader Variants";
        const string lotShaderVariantDrawback = "Adds multiple Shader Variants";
        const string gBufferDrawback = "Adds a GBuffer";
        const string lotGBufferDrawback = "Adds GBuffers";
        const string dBufferDrawback = "Adds a DBuffer";
        const string lotDBufferDrawback = "Adds DBuffers";
        static readonly Dictionary<GUIContent, string> k_SupportDrawbacks = new Dictionary<GUIContent, string>
        {
            //k_SupportLitShaderModeContent is special case handled separately
            //k_SupportShadowMaskContent is special case handled separately
            { k_SupportSSRContent                  , memoryDrawback },
            { k_SupportSSAOContent                 , memoryDrawback },
            { k_SupportedSSSContent                , memoryDrawback },
            { k_SupportVolumetricContent           , memoryDrawback },
            //k_SupportLightLayerContent is special case handled separately
            { k_MSAASampleCountContent             , memoryDrawback },
            { k_SupportDecalContent                , string.Format("{0}, {1}", memoryDrawback, lotDBufferDrawback) },
            { k_MetalAndAOContent                  , string.Format("{0}, {1}", memoryDrawback, dBufferDrawback) },
            { k_SupportMotionVectorContent         , memoryDrawback },
            { k_SupportRuntimeDebugDisplayContent  , shaderVariantDrawback },
            { k_SupportDitheringCrossFadeContent   , shaderVariantDrawback },
            { k_SupportDistortion                  , "" },
            { k_SupportTransparentBackface         , shaderVariantDrawback },
            { k_SupportTransparentDepthPrepass     , shaderVariantDrawback },
            { k_SupportTransparentDepthPostpass    , shaderVariantDrawback },
            { k_SupportRaytracing                  , string.Format("{0}, {1}", memoryDrawback, lotShaderVariantDrawback) }
        };

        static Dictionary<SupportedLitShaderMode, string> k_SupportLitShaderModeDrawbacks = new Dictionary<SupportedLitShaderMode, string>
        {
            { SupportedLitShaderMode.ForwardOnly, lotShaderVariantDrawback },
            { SupportedLitShaderMode.DeferredOnly, string.Format("{0}, {1}", shaderVariantDrawback, lotGBufferDrawback) },
            { SupportedLitShaderMode.Both, string.Format("{0}, {1}", lotShaderVariantDrawback, lotGBufferDrawback) }
        };

        static Dictionary<SupportedLitShaderMode, string> k_SupportShadowMaskDrawbacks = new Dictionary<SupportedLitShaderMode, string>
        {
            { SupportedLitShaderMode.ForwardOnly, string.Format("{0}, {1}", shaderVariantDrawback, memoryDrawback) },
            { SupportedLitShaderMode.DeferredOnly, string.Format("{0}, {1}, {2}", shaderVariantDrawback, memoryDrawback, gBufferDrawback) },
            { SupportedLitShaderMode.Both, string.Format("{0}, {1}, {2}", shaderVariantDrawback, memoryDrawback, gBufferDrawback) }
        };

        static Dictionary<SupportedLitShaderMode, string> k_SupportLightLayerDrawbacks = new Dictionary<SupportedLitShaderMode, string>
        {
            { SupportedLitShaderMode.ForwardOnly, memoryDrawback },
            { SupportedLitShaderMode.DeferredOnly, string.Format("{0}, {1}", memoryDrawback, gBufferDrawback) },
            { SupportedLitShaderMode.Both, string.Format("{0}, {1}", memoryDrawback, gBufferDrawback) }
        };
    }
}
