using System;
using UnityEngine.Rendering;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public partial class RenderPipelineResources : ScriptableObject
    {
        [Serializable, ReloadGroup]
        public sealed class ShaderResources
        {
            // Defaults
            [Reload("Runtime/Material/Lit/Lit.shader")]
            public Shader defaultPS;

            // Debug
            [Reload("Runtime/Debug/DebugDisplayLatlong.Shader")]
            public Shader debugDisplayLatlongPS;
            [Reload("Runtime/Debug/DebugViewMaterialGBuffer.Shader")]
            public Shader debugViewMaterialGBufferPS;
            [Reload("Runtime/Debug/DebugViewTiles.Shader")]
            public Shader debugViewTilesPS;
            [Reload("Runtime/Debug/DebugFullScreen.Shader")]
            public Shader debugFullScreenPS;
            [Reload("Runtime/Debug/DebugColorPicker.Shader")]
            public Shader debugColorPickerPS;
            [Reload("Runtime/Debug/DebugLightVolumes.Shader")]
            public Shader debugLightVolumePS;
            [Reload("Runtime/Debug/DebugLightVolumes.compute")]
            public ComputeShader debugLightVolumeCS;

            // Lighting
            [Reload("Runtime/Lighting/Deferred.Shader")]
            public Shader deferredPS;
            [Reload("Runtime/RenderPipeline/RenderPass/ColorPyramidPS.Shader")]
            public Shader colorPyramidPS;
            [Reload("Runtime/RenderPipeline/RenderPass/DepthPyramid.compute")]
            public ComputeShader depthPyramidCS;
            [Reload("Runtime/Core/CoreResources/GPUCopy.compute")]
            public ComputeShader copyChannelCS;
            [Reload("Runtime/Lighting/ScreenSpaceLighting/ScreenSpaceReflections.compute")]
            public ComputeShader screenSpaceReflectionsCS;
            [Reload("Runtime/RenderPipeline/RenderPass/Distortion/ApplyDistortion.shader")]
            public Shader applyDistortionPS;

            // Lighting tile pass
            [Reload("Runtime/Lighting/LightLoop/cleardispatchindirect.compute")]
            public ComputeShader clearDispatchIndirectCS;
            [Reload("Runtime/Lighting/LightLoop/builddispatchindirect.compute")]
            public ComputeShader buildDispatchIndirectCS;
            [Reload("Runtime/Lighting/LightLoop/scrbound.compute")]
            public ComputeShader buildScreenAABBCS;
            [Reload("Runtime/Lighting/LightLoop/lightlistbuild.compute")]
            public ComputeShader buildPerTileLightListCS;               // FPTL
            [Reload("Runtime/Lighting/LightLoop/lightlistbuild-bigtile.compute")]
            public ComputeShader buildPerBigTileLightListCS;
            [Reload("Runtime/Lighting/LightLoop/lightlistbuild-clustered.compute")]
            public ComputeShader buildPerVoxelLightListCS;              // clustered
            [Reload("Runtime/Lighting/LightLoop/materialflags.compute")]
            public ComputeShader buildMaterialFlagsCS;
            [Reload("Runtime/Lighting/LightLoop/Deferred.compute")]
            public ComputeShader deferredCS;
            [Reload("Runtime/Lighting/Shadow/ContactShadows.compute")]
            public ComputeShader contactShadowCS;
            [Reload("Runtime/Lighting/VolumetricLighting/VolumeVoxelization.compute")]
            public ComputeShader volumeVoxelizationCS;
            [Reload("Runtime/Lighting/VolumetricLighting/VolumetricLighting.compute")]
            public ComputeShader volumetricLightingCS;
            [Reload("Runtime/Lighting/LightLoop/DeferredTile.shader")]
            public Shader deferredTilePS;
            [Reload("Runtime/Lighting/Shadow/ScreenSpaceShadows.shader")]
            public Shader screenSpaceShadowPS;

            [Reload("Runtime/Material/SubsurfaceScattering/SubsurfaceScattering.compute")]
            public ComputeShader subsurfaceScatteringCS;                // Disney SSS
            [Reload("Runtime/Material/SubsurfaceScattering/CombineLighting.shader")]
            public Shader combineLightingPS;

            // General
            [Reload("Runtime/RenderPipeline/RenderPass/MotionVectors/CameraMotionVectors.shader")]
            public Shader cameraMotionVectorsPS;
            [Reload("Runtime/ShaderLibrary/CopyStencilBuffer.shader")]
            public Shader copyStencilBufferPS;
            [Reload("Runtime/ShaderLibrary/CopyDepthBuffer.shader")]
            public Shader copyDepthBufferPS;
            [Reload("Runtime/ShaderLibrary/Blit.shader")]
            public Shader blitPS;

            [Reload("Runtime/ShaderLibrary/DownsampleDepth.shader")]
            public Shader downsampleDepthPS;
            [Reload("Runtime/ShaderLibrary/UpsampleTransparent.shader")]
            public Shader upsampleTransparentPS;

            // Sky
            [Reload("Runtime/Sky/BlitCubemap.shader")]
            public Shader blitCubemapPS;
            [Reload("Runtime/Material/GGXConvolution/BuildProbabilityTables.compute")]
            public ComputeShader buildProbabilityTablesCS;
            [Reload("Runtime/Material/GGXConvolution/ComputeGgxIblSampleData.compute")]
            public ComputeShader computeGgxIblSampleDataCS;
            [Reload("Runtime/Material/GGXConvolution/GGXConvolve.shader")]
            public Shader GGXConvolvePS;
            [Reload("Runtime/Material/Fabric/CharlieConvolve.shader")]
            public Shader charlieConvolvePS;
            [Reload("Runtime/Lighting/AtmosphericScattering/OpaqueAtmosphericScattering.shader")]
            public Shader opaqueAtmosphericScatteringPS;
            [Reload("Runtime/Sky/HDRISky/HDRISky.shader")]
            public Shader hdriSkyPS;
            [Reload("Runtime/Sky/HDRISky/IntegrateHDRISky.shader")]
            public Shader integrateHdriSkyPS;
            [Reload("Runtime/Sky/ProceduralSky/ProceduralSky.shader")]
            public Shader proceduralSkyPS;
            [Reload("Skybox/Cubemap", ReloadAttribute.Package.Builtin)]
            public Shader skyboxCubemapPS;
            [Reload("Runtime/Sky/GradientSky/GradientSky.shader")]
            public Shader gradientSkyPS;
            [Reload("Runtime/Sky/AmbientProbeConvolution.compute")]
            public ComputeShader ambientProbeConvolutionCS;

            // Material
            [Reload("Runtime/Material/PreIntegratedFGD/PreIntegratedFGD_GGXDisneyDiffuse.shader")]
            public Shader preIntegratedFGD_GGXDisneyDiffusePS;
            [Reload("Runtime/Material/PreIntegratedFGD/PreIntegratedFGD_CharlieFabricLambert.shader")]
            public Shader preIntegratedFGD_CharlieFabricLambertPS;
            [Reload("Runtime/Material/AxF/PreIntegratedFGD_Ward.shader")]
            public Shader preIntegratedFGD_WardPS;
            [Reload("Runtime/Material/AxF/PreIntegratedFGD_CookTorrance.shader")]
            public Shader preIntegratedFGD_CookTorrancePS;

            // Utilities / Core
            [Reload("Runtime/Core/CoreResources/EncodeBC6H.compute")]
            public ComputeShader encodeBC6HCS;
            [Reload("Runtime/Core/CoreResources/CubeToPano.shader")]
            public Shader cubeToPanoPS;
            [Reload("Runtime/Core/CoreResources/BlitCubeTextureFace.shader")]
            public Shader blitCubeTextureFacePS;
            [Reload("Runtime/Material/LTCAreaLight/FilterAreaLightCookies.shader")]
            public Shader filterAreaLightCookiesPS;
            [Reload("Runtime/Core/CoreResources/ClearUIntTextureArray.compute")]
            public ComputeShader clearUIntTextureCS;

            // Shadow            
            [Reload("Runtime/Lighting/Shadow/ShadowClear.shader")]
            public Shader shadowClearPS;
            [Reload("Runtime/Lighting/Shadow/EVSMBlur.compute")]
            public ComputeShader evsmBlurCS;
            [Reload("Runtime/Lighting/Shadow/DebugDisplayHDShadowMap.shader")]
            public Shader debugHDShadowMapPS;
            [Reload("Runtime/Lighting/Shadow/MomentShadows.compute")]
            public ComputeShader momentShadowsCS;

            // Decal
            [Reload("Runtime/Material/Decal/DecalNormalBuffer.shader")]
            public Shader decalNormalBufferPS;

            // Ambient occlusion
            [Reload("Runtime/Lighting/ScreenSpaceLighting/GTAO.compute")]
            public ComputeShader GTAOCS;
            [Reload("Runtime/Lighting/ScreenSpaceLighting/GTAODenoise.compute")]
            public ComputeShader GTAODenoiseCS;
            [Reload("Runtime/Lighting/ScreenSpaceLighting/GTAOUpsample.compute")]
            public ComputeShader GTAOUpsampleCS;

            // MSAA Shaders
            [Reload("Runtime/RenderPipeline/RenderPass/MSAA/DepthValues.shader")]
            public Shader depthValuesPS;
            [Reload("Runtime/RenderPipeline/RenderPass/MSAA/ColorResolve.shader")]
            public Shader colorResolvePS;

            // Post-processing
            [Reload("Runtime/PostProcessing/Shaders/NaNKiller.compute")]
            public ComputeShader nanKillerCS;
            [Reload("Runtime/PostProcessing/Shaders/Exposure.compute")]
            public ComputeShader exposureCS;
            [Reload("Runtime/PostProcessing/Shaders/UberPost.compute")]
            public ComputeShader uberPostCS;
            [Reload("Runtime/PostProcessing/Shaders/LutBuilder3D.compute")]
            public ComputeShader lutBuilder3DCS;
            [Reload("Runtime/PostProcessing/Shaders/TemporalAntialiasing.compute")]
            public ComputeShader temporalAntialiasingCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldKernel.compute")]
            public ComputeShader depthOfFieldKernelCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoC.compute")]
            public ComputeShader depthOfFieldCoCCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoCReproject.compute")]
            public ComputeShader depthOfFieldCoCReprojectCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoCDilate.compute")]
            public ComputeShader depthOfFieldDilateCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldMip.compute")]
            public ComputeShader depthOfFieldMipCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldMipSafe.compute")]
            public ComputeShader depthOfFieldMipSafeCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldPrefilter.compute")]
            public ComputeShader depthOfFieldPrefilterCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldTileMax.compute")]
            public ComputeShader depthOfFieldTileMaxCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldGather.compute")]
            public ComputeShader depthOfFieldGatherCS;
            [Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCombine.compute")]
            public ComputeShader depthOfFieldCombineCS;
            [Reload("Runtime/PostProcessing/Shaders/PaniniProjection.compute")]
            public ComputeShader paniniProjectionCS;
            [Reload("Runtime/PostProcessing/Shaders/MotionBlurMotionVecPrep.compute")]
            public ComputeShader motionBlurMotionVecPrepCS;
            [Reload("Runtime/PostProcessing/Shaders/MotionBlurTilePass.compute")]
            public ComputeShader motionBlurTileGenCS;
            [Reload("Runtime/PostProcessing/Shaders/MotionBlur.compute")]
            public ComputeShader motionBlurCS;
            [Reload("Runtime/PostProcessing/Shaders/BloomPrefilter.compute")]
            public ComputeShader bloomPrefilterCS;
            [Reload("Runtime/PostProcessing/Shaders/BloomBlur.compute")]
            public ComputeShader bloomBlurCS;
            [Reload("Runtime/PostProcessing/Shaders/BloomUpsample.compute")]
            public ComputeShader bloomUpsampleCS;
            [Reload("Runtime/PostProcessing/Shaders/FXAA.compute")]
            public ComputeShader FXAACS;
            [Reload("Runtime/PostProcessing/Shaders/FinalPass.shader")]
            public Shader finalPassPS;
            [Reload("Runtime/PostProcessing/Shaders/ClearBlack.shader")]
            public Shader clearBlackPS;
            [Reload("Runtime/PostProcessing/Shaders/SubpixelMorphologicalAntialiasing.shader")]
            public Shader SMAAPS;

            // Iterator to retrieve all compute shaders in reflection so we don't have to keep a list of
            // used compute shaders up to date (prefer editor-only usage)
            public IEnumerable<ComputeShader> GetAllComputeShaders()
            {
                var fields = typeof(ShaderResources).GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.GetValue(this) is ComputeShader computeShader)
                        yield return computeShader;
                }
            }
        }

        [Serializable, ReloadGroup]
        public sealed class MaterialResources
        {
        }

        [Serializable, ReloadGroup]
        public sealed class TextureResources
        {
            // Debug
            [Reload("Runtime/RenderPipelineResources/Texture/DebugFont.tga")]
            public Texture2D debugFontTex;
            [Reload("Runtime/Debug/ColorGradient.png")]
            public Texture2D colorGradient;
            [Reload("Runtime/RenderPipelineResources/Texture/Matcap/DefaultMatcap.png")]
            public Texture2D matcapTex;

            // Pre-baked noise
            [Reload("Runtime/RenderPipelineResources/Texture/BlueNoise16/L/LDR_LLL1_{0}.png", 0, 32)]
            public Texture2D[] blueNoise16LTex;
            [Reload("Runtime/RenderPipelineResources/Texture/BlueNoise16/RGB/LDR_RGB1_{0}.png", 0, 32)]
            public Texture2D[] blueNoise16RGBTex;
            [Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/OwenScrambledNoise.png")]
            public Texture2D owenScrambledTex;
            [Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/ScrambleNoise.png")]
            public Texture2D scramblingTex;

            // Post-processing
            [Reload(new[]
            {
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Thin01.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Thin02.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium01.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium02.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium03.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium04.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium05.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium06.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Large01.png",
                "Runtime/RenderPipelineResources/Texture/FilmGrain/Large02.png"
            })]
            public Texture2D[] filmGrainTex;
            [Reload("Runtime/RenderPipelineResources/Texture/SMAA/SearchTex.tga")]
            public Texture2D   SMAASearchTex;
            [Reload("Runtime/RenderPipelineResources/Texture/SMAA/AreaTex.tga")]
            public Texture2D   SMAAAreaTex;
        }

        [Serializable, ReloadGroup]
        public sealed class ShaderGraphResources
        {
        }

        [Serializable, ReloadGroup]
        public sealed class AssetResources
        {
            [Reload("Runtime/RenderPipelineResources/defaultDiffusionProfile.asset")]
            public DiffusionProfileSettings defaultDiffusionProfile;
        }

        public ShaderResources shaders;
        public MaterialResources materials;
        public TextureResources textures;
        public ShaderGraphResources shaderGraphs;
        public AssetResources assets;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RenderPipelineResources))]
    class RenderPipelineResourcesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Add a "Reload All" button in inspector when we are in developer's mode
            if (UnityEditor.EditorPrefs.GetBool("DeveloperMode")
                && GUILayout.Button("Reload All"))
            {
                var resources = target as RenderPipelineResources;
                resources.materials = null;
                resources.textures = null;
                resources.shaders = null;
                resources.shaderGraphs = null;
                ResourceReloader.ReloadAllNullIn(target, HDUtils.GetHDRenderPipelinePath());
            }
        }
    }
#endif
}
