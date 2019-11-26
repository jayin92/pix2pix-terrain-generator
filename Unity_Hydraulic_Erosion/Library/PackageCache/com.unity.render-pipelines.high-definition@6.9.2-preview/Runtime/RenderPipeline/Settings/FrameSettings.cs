using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    /// <summary>Helper to handle Deferred or Forward but not both</summary>
    public enum LitShaderMode
    {
        Forward,
        Deferred
    }

    public enum LODBiasMode
    {
        /// <summary>Use the current quality settings value.</summary>
        FromQualitySettings,
        /// <summary>Scale the current quality settings value.</summary>
        ScaleQualitySettings,
        /// <summary>Set the current quality settings value.</summary>
        Fixed,
    }
    public enum MaximumLODLevelMode
    {
        /// <summary>Use the current quality settings value.</summary>
        FromQualitySettings,
        /// <summary>Offset the current quality settings value.</summary>
        OffsetQualitySettings,
        /// <summary>Set the current quality settings value.</summary>
        Fixed,
    }

    public static class LODBiasModeExtensions
    {
        public static float ComputeValue(this LODBiasMode mode, float qualitySettingValue, float inputValue)
        {
            switch (mode)
            {
                case LODBiasMode.FromQualitySettings: return qualitySettingValue;
                case LODBiasMode.Fixed: return inputValue;
                case LODBiasMode.ScaleQualitySettings: return inputValue * qualitySettingValue;
                default: throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }
    }

    public static class MaximumLODLevelModeExtensions
    {
        public static int ComputeValue(this MaximumLODLevelMode mode, int qualitySettingsValue, int inputValue)
        {
            switch (mode)
            {
                case MaximumLODLevelMode.FromQualitySettings: return qualitySettingsValue;
                case MaximumLODLevelMode.OffsetQualitySettings: return qualitySettingsValue + inputValue;
                case MaximumLODLevelMode.Fixed: return inputValue;
                default: throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }
    }

    // To add a new element to FrameSettings, add en entry in this enum using the FrameSettingsFieldAttribute.
    // Inspector UI and DebugMenu are generated from this.
    // If you have very specific display requirement, you could add them in FrameSettingsUI.Drawer.cs with a AmmendInfo command.
    public enum FrameSettingsField
    {
        None = -1,

        //rendering settings from 0 to 19
        [FrameSettingsField(0, autoName: LitShaderMode, type: FrameSettingsFieldAttribute.DisplayType.BoolAsEnumPopup, targetType: typeof(LitShaderMode), customOrderInGroup: 0)]
        LitShaderMode = 0,
        [FrameSettingsField(0, displayedName: "Depth Prepass within Deferred", positiveDependencies: new[] { LitShaderMode })]
        DepthPrepassWithDeferredRendering = 1,
        [FrameSettingsField(0, displayedName: "Clear GBuffers", positiveDependencies: new[] { LitShaderMode }, customOrderInGroup: 2)]
        ClearGBuffers = 5,
        [FrameSettingsField(0, displayedName: "MSAA within Forward", negativeDependencies: new[] { LitShaderMode }, customOrderInGroup: 2)]
        MSAA = 31,
        [FrameSettingsField(0, autoName: OpaqueObjects, customOrderInGroup: 3)]
        OpaqueObjects = 2,
        [FrameSettingsField(0, autoName: TransparentObjects)]
        TransparentObjects = 3,
        [FrameSettingsField(0, autoName: RealtimePlanarReflection)]
        RealtimePlanarReflection = 4,

        [FrameSettingsField(0, autoName: TransparentPrepass)]
        TransparentPrepass = 8,
        [FrameSettingsField(0, autoName: TransparentPostpass)]
        TransparentPostpass = 9,
        [FrameSettingsField(0, displayedName: "Transparent Write Motion Vectors", customOrderInGroup: 7)]
        TransparentsWriteMotionVector = 16,
        [FrameSettingsField(0, autoName: MotionVectors)]
        MotionVectors = 10,
        [FrameSettingsField(0, autoName: ObjectMotionVectors, positiveDependencies: new[] { MotionVectors })]
        ObjectMotionVectors = 11,
        [FrameSettingsField(0, autoName: Decals)]
        Decals = 12,
        [FrameSettingsField(0, autoName: RoughRefraction)]
        RoughRefraction = 13,
        [FrameSettingsField(0, autoName: Distortion)]
        Distortion = 14,
        [FrameSettingsField(0, autoName: Postprocess)]
        Postprocess = 15,
        [FrameSettingsField(0, autoName: AfterPostprocess, customOrderInGroup: 15)]
        AfterPostprocess = 17,
        [FrameSettingsField(0, autoName: LowResTransparent)]
        LowResTransparent = 18,
        [FrameSettingsField(0, displayedName: "ZTest For After PostProcess", tooltip: "When enabled, Cameras that don't use TAA process a depth test for Materials in the AfterPostProcess rendering pass.")]
        ZTestAfterPostProcessTAA = 19,

        //lighting settings from 20 to 39
        [FrameSettingsField(1, autoName: Shadow)]
        Shadow = 20,
        [FrameSettingsField(1, autoName: ContactShadows)]
        ContactShadows = 21,
        [FrameSettingsField(1, autoName: ScreenSpaceShadows, customOrderInGroup: 22)]
        ScreenSpaceShadows = 34,
        [FrameSettingsField(1, autoName: ShadowMask, customOrderInGroup: 23)]
        ShadowMask = 22,
        [FrameSettingsField(1, autoName: SSR)]
        SSR = 23,
        [FrameSettingsField(1, autoName: SSAO)]
        SSAO = 24,
        [FrameSettingsField(1, autoName: SubsurfaceScattering)]
        SubsurfaceScattering = 25,
        [FrameSettingsField(1, autoName: Transmission)]
        Transmission = 26,
        [FrameSettingsField(1, displayedName: "Fog")]
        AtmosphericScattering = 27,
        [FrameSettingsField(1, autoName: Volumetrics, positiveDependencies: new[] { AtmosphericScattering })]
        Volumetrics = 28,
        [FrameSettingsField(1, autoName: ReprojectionForVolumetrics, positiveDependencies: new[] { AtmosphericScattering, Volumetrics })]
        ReprojectionForVolumetrics = 29,
        [FrameSettingsField(1, autoName: LightLayers)]
        LightLayers = 30,
        [FrameSettingsField(1, autoName: ExposureControl, customOrderInGroup: 32)]
        ExposureControl = 32,
        [FrameSettingsField(1, autoName: EnableReflectionProbe, customOrderInGroup: 33)]
        EnableReflectionProbe = 33,
        [FrameSettingsField(1, autoName: EnablePlanarProbe, customOrderInGroup: 34)]
        EnablePlanarProbe = 35,
        [FrameSettingsField(1, autoName: ReplaceDiffuseForIndirect, customOrderInGroup: 35)]
        ReplaceDiffuseForIndirect = 36,
        [FrameSettingsField(1, autoName: EnableSkyLighting, customOrderInGroup: 36)]
        EnableSkyLighting = 37,

        //async settings from 40 to 59
        [FrameSettingsField(2, autoName: AsyncCompute)]
        AsyncCompute = 40,
        [FrameSettingsField(2, autoName: LightListAsync, positiveDependencies: new[] { AsyncCompute })]
        LightListAsync = 41,
        [FrameSettingsField(2, autoName: SSRAsync, positiveDependencies: new[] { AsyncCompute })]
        SSRAsync = 42,
        [FrameSettingsField(2, autoName: SSAOAsync, positiveDependencies: new[] { AsyncCompute })]
        SSAOAsync = 43,
        [FrameSettingsField(2, autoName: ContactShadowsAsync, positiveDependencies: new[] { AsyncCompute })]
        ContactShadowsAsync = 44,
        [FrameSettingsField(2, autoName: VolumeVoxelizationsAsync, positiveDependencies: new[] { AsyncCompute })]
        VolumeVoxelizationsAsync = 45,

        //from 60 to 119 : space for new scopes
        // true <=> Fixed, false <=> FromQualitySettings (default)
        [FrameSettingsField(0, autoName: LODBiasMode, type: FrameSettingsFieldAttribute.DisplayType.Others, targetType: typeof(LODBiasMode))]
        LODBiasMode = 60,
        /// <summary>Set the LOD Bias with the value in <see cref="FrameSettings.lodBias"/>.</summary>
        [FrameSettingsField(0, autoName: LODBias, type: FrameSettingsFieldAttribute.DisplayType.Others, positiveDependencies: new[]{ LODBiasMode })]
        LODBias = 61,
        // true <=> Fixed, false <=> FromQualitySettings (default)
        [FrameSettingsField(0, autoName: MaximumLODLevelMode, type: FrameSettingsFieldAttribute.DisplayType.Others, targetType: typeof(MaximumLODLevelMode))]
        MaximumLODLevelMode = 62,
        /// <summary>Set the LOD Bias with the value in <see cref="FrameSettings.maximumLODLevel"/>.</summary>
        [FrameSettingsField(0, autoName: MaximumLODLevel, type: FrameSettingsFieldAttribute.DisplayType.Others, positiveDependencies: new[]{ MaximumLODLevelMode })]
        MaximumLODLevel = 63,

        //lightLoop settings from 120 to 127
        [FrameSettingsField(3, autoName: FPTLForForwardOpaque)]
        FPTLForForwardOpaque = 120,
        [FrameSettingsField(3, autoName: BigTilePrepass)]
        BigTilePrepass = 121,
        [FrameSettingsField(3, autoName: DeferredTile)]
        DeferredTile = 122,
        [FrameSettingsField(3, autoName: ComputeLightEvaluation, positiveDependencies: new[] { DeferredTile })]
        ComputeLightEvaluation = 123,
        [FrameSettingsField(3, autoName: ComputeLightVariants, positiveDependencies: new[] { DeferredTile })]
        ComputeLightVariants = 124,
        [FrameSettingsField(3, autoName: ComputeMaterialVariants, positiveDependencies: new[] { DeferredTile })]
        ComputeMaterialVariants = 125,
        Reflection = 126, //set by engine, not for DebugMenu/Inspector

        //only 128 booleans saved. For more, change the BitArray used
    }

    /// <summary>BitField that state which element is overridden.</summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{mask.humanizedData}")]
    public struct FrameSettingsOverrideMask
    {
        [SerializeField]
        public BitArray128 mask;
    }

    /// <summary>Per renderer and per frame settings.</summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{bitDatas.humanizedData}")]
    public partial struct FrameSettings
    {
        /// <summary>Default FrameSettings for Camera renderer.</summary>
        public static readonly FrameSettings defaultCamera = new FrameSettings()
        {
            bitDatas = new BitArray128(new uint[] {
                (uint)FrameSettingsField.Shadow,
                (uint)FrameSettingsField.ContactShadows,
                (uint)FrameSettingsField.ShadowMask,
                (uint)FrameSettingsField.SSR,
                (uint)FrameSettingsField.SSAO,
                (uint)FrameSettingsField.SubsurfaceScattering,
                (uint)FrameSettingsField.Transmission,   // Caution: this is only for debug, it doesn't save the cost of Transmission execution
                (uint)FrameSettingsField.AtmosphericScattering,
                (uint)FrameSettingsField.Volumetrics,
                (uint)FrameSettingsField.ReprojectionForVolumetrics,
                (uint)FrameSettingsField.LightLayers,
                (uint)FrameSettingsField.ExposureControl,
                (uint)FrameSettingsField.LitShaderMode, //deffered ; enum with only two value saved as a bool
                (uint)FrameSettingsField.TransparentPrepass,
                (uint)FrameSettingsField.TransparentPostpass,
                (uint)FrameSettingsField.MotionVectors, // Enable/disable whole motion vectors pass (Camera + Object).
                (uint)FrameSettingsField.ObjectMotionVectors,
                (uint)FrameSettingsField.Decals,
                (uint)FrameSettingsField.RoughRefraction, // Depends on DepthPyramid - If not enable, just do a copy of the scene color (?) - how to disable rough refraction ?
                (uint)FrameSettingsField.Distortion,
                (uint)FrameSettingsField.Postprocess,
                (uint)FrameSettingsField.AfterPostprocess,
                (uint)FrameSettingsField.LowResTransparent,
                (uint)FrameSettingsField.ZTestAfterPostProcessTAA,
                (uint)FrameSettingsField.OpaqueObjects,
                (uint)FrameSettingsField.TransparentObjects,
                (uint)FrameSettingsField.RealtimePlanarReflection,
                (uint)FrameSettingsField.AsyncCompute,
                (uint)FrameSettingsField.LightListAsync,
                (uint)FrameSettingsField.SSRAsync,
                (uint)FrameSettingsField.SSRAsync,
                (uint)FrameSettingsField.SSAOAsync,
                (uint)FrameSettingsField.ContactShadowsAsync,
                (uint)FrameSettingsField.VolumeVoxelizationsAsync,
                (uint)FrameSettingsField.DeferredTile,
                (uint)FrameSettingsField.ComputeLightEvaluation,
                (uint)FrameSettingsField.ComputeLightVariants,
                (uint)FrameSettingsField.ComputeMaterialVariants,
                (uint)FrameSettingsField.FPTLForForwardOpaque,
                (uint)FrameSettingsField.BigTilePrepass,
                (uint)FrameSettingsField.TransparentsWriteMotionVector,
                (uint)FrameSettingsField.EnableReflectionProbe,
                (uint)FrameSettingsField.EnablePlanarProbe,
                (uint)FrameSettingsField.EnableSkyLighting,
            }),
            lodBias = 1,
        };
        /// <summary>Default FrameSettings for realtime ReflectionProbe/PlanarReflectionProbe renderer.</summary>
        public static readonly FrameSettings defaultRealtimeReflectionProbe = new FrameSettings()
        {
            bitDatas = new BitArray128(new uint[] {
                (uint)FrameSettingsField.Shadow,
                //(uint)FrameSettingsField.ContactShadow,
                //(uint)FrameSettingsField.ShadowMask,
                //(uint)FrameSettingsField.SSR,
                //(uint)FrameSettingsField.SSAO,
                (uint)FrameSettingsField.SubsurfaceScattering,
                (uint)FrameSettingsField.Transmission,   // Caution: this is only for debug, it doesn't save the cost of Transmission execution
                //(uint)FrameSettingsField.AtmosphericScaterring,
                (uint)FrameSettingsField.Volumetrics,
                (uint)FrameSettingsField.ReprojectionForVolumetrics,
                (uint)FrameSettingsField.LightLayers,
                //(uint)FrameSettingsField.ExposureControl,
                (uint)FrameSettingsField.LitShaderMode, //deffered ; enum with only two value saved as a bool
                (uint)FrameSettingsField.TransparentPrepass,
                (uint)FrameSettingsField.TransparentPostpass,
                (uint)FrameSettingsField.MotionVectors, // Enable/disable whole motion vectors pass (Camera + Object).
                (uint)FrameSettingsField.ObjectMotionVectors,
                (uint)FrameSettingsField.Decals,
                //(uint)FrameSettingsField.RoughRefraction, // Depends on DepthPyramid - If not enable, just do a copy of the scene color (?) - how to disable rough refraction ?
                //(uint)FrameSettingsField.Distortion,
                //(uint)FrameSettingsField.Postprocess,
                //(uint)FrameSettingsField.AfterPostprocess,
                (uint)FrameSettingsField.OpaqueObjects,
                (uint)FrameSettingsField.TransparentObjects,
                (uint)FrameSettingsField.RealtimePlanarReflection,
                (uint)FrameSettingsField.AsyncCompute,
                (uint)FrameSettingsField.LightListAsync,
                (uint)FrameSettingsField.SSRAsync,
                (uint)FrameSettingsField.SSRAsync,
                (uint)FrameSettingsField.SSAOAsync,
                (uint)FrameSettingsField.ContactShadowsAsync,
                (uint)FrameSettingsField.VolumeVoxelizationsAsync,
                (uint)FrameSettingsField.DeferredTile,
                (uint)FrameSettingsField.ComputeLightEvaluation,
                (uint)FrameSettingsField.ComputeLightVariants,
                (uint)FrameSettingsField.ComputeMaterialVariants,
                (uint)FrameSettingsField.FPTLForForwardOpaque,
                (uint)FrameSettingsField.BigTilePrepass,
                (uint)FrameSettingsField.EnableReflectionProbe,
                (uint)FrameSettingsField.EnableSkyLighting,
            }),
            lodBias = 1,
        };
        /// <summary>Default FrameSettings for baked or custom ReflectionProbe renderer.</summary>
        public static readonly FrameSettings defaultCustomOrBakeReflectionProbe = new FrameSettings()
        {
            bitDatas = new BitArray128(new uint[] {
                (uint)FrameSettingsField.Shadow,
                (uint)FrameSettingsField.ContactShadows,
                (uint)FrameSettingsField.ShadowMask,
                //(uint)FrameSettingsField.SSR,
                (uint)FrameSettingsField.SSAO,
                (uint)FrameSettingsField.SubsurfaceScattering,
                (uint)FrameSettingsField.Transmission,   // Caution: this is only for debug, it doesn't save the cost of Transmission execution
                (uint)FrameSettingsField.AtmosphericScattering,
                (uint)FrameSettingsField.Volumetrics,
                (uint)FrameSettingsField.ReprojectionForVolumetrics,
                (uint)FrameSettingsField.LightLayers,
                //(uint)FrameSettingsField.ExposureControl,
                (uint)FrameSettingsField.LitShaderMode, //deffered ; enum with only two value saved as a bool
                (uint)FrameSettingsField.TransparentPrepass,
                (uint)FrameSettingsField.TransparentPostpass,
                //(uint)FrameSettingsField.MotionVectors, // Enable/disable whole motion vectors pass (Camera + Object).
                //(uint)FrameSettingsField.ObjectMotionVectors,
                (uint)FrameSettingsField.Decals,
                (uint)FrameSettingsField.RoughRefraction, // Depends on DepthPyramid - If not enable, just do a copy of the scene color (?) - how to disable rough refraction ?
                (uint)FrameSettingsField.Distortion,
                //(uint)FrameSettingsField.Postprocess,
                //(uint)FrameSettingsField.AfterPostprocess,
                (uint)FrameSettingsField.OpaqueObjects,
                (uint)FrameSettingsField.TransparentObjects,
                (uint)FrameSettingsField.RealtimePlanarReflection,
                (uint)FrameSettingsField.AsyncCompute,
                (uint)FrameSettingsField.LightListAsync,
                //(uint)FrameSettingsField.SSRAsync,
                (uint)FrameSettingsField.SSAOAsync,
                (uint)FrameSettingsField.ContactShadowsAsync,
                (uint)FrameSettingsField.VolumeVoxelizationsAsync,
                (uint)FrameSettingsField.DeferredTile,
                (uint)FrameSettingsField.ComputeLightEvaluation,
                (uint)FrameSettingsField.ComputeLightVariants,
                (uint)FrameSettingsField.ComputeMaterialVariants,
                (uint)FrameSettingsField.FPTLForForwardOpaque,
                (uint)FrameSettingsField.BigTilePrepass,
                (uint)FrameSettingsField.ReplaceDiffuseForIndirect,
            }),
            lodBias = 1,
        };

        // Each time you add data in the framesettings. Attempt to add boolean one only if possible.
        // BitArray is quick in computation and take not a lot of space. It can contains only boolean value.
        // If anyone wants more than 128 bit, the BitArray256 already exist. Just replace this one with it should be enough.
        // For more, you should write one using previous as exemple.
        [SerializeField]
        BitArray128 bitDatas;

        /// <summary>
        /// if <c>lodBiasMode == LODBiasMode.Fixed</c>, then this value will overwrite <c>QualitySettings.lodBias</c>
        /// if <c>lodBiasMode == LODBiasMode.ScaleQualitySettings</c>, then this value will scale <c>QualitySettings.lodBias</c>
        /// </summary>
        public float lodBias;
        /// <summary>Define how the <c>QualitySettings.lodBias</c> value is set.</summary>
        public LODBiasMode lodBiasMode;
        /// <summary>
        /// if <c>maximumLODLevelMode == MaximumLODLevelMode.FromQualitySettings</c>, then this value will overwrite <c>QualitySettings.maximumLODLevel</c>
        /// if <c>maximumLODLevelMode == MaximumLODLevelMode.OffsetQualitySettings</c>, then this value will offset <c>QualitySettings.maximumLODLevel</c>
        /// </summary>
        public int maximumLODLevel;
        /// <summary>Define how the <c>QualitySettings.maximumLODLevel</c> value is set.</summary>
        public MaximumLODLevelMode maximumLODLevelMode;

        /// <summary>Helper to see binary saved data on LitShaderMode as a LitShaderMode enum.</summary>
        public LitShaderMode litShaderMode
        {
            get => bitDatas[(uint)FrameSettingsField.LitShaderMode] ? LitShaderMode.Deferred : LitShaderMode.Forward;
            set => bitDatas[(uint)FrameSettingsField.LitShaderMode] = value == LitShaderMode.Deferred;
        }

        /// <summary>Get stored data for this field.</summary>
        public bool IsEnabled(FrameSettingsField field) => bitDatas[(uint)field];
        /// <summary>Set stored data for this field.</summary>
        public void SetEnabled(FrameSettingsField field, bool value) => bitDatas[(uint)field] = value;

        // followings are helper for engine.
        internal bool fptl => litShaderMode == LitShaderMode.Deferred || bitDatas[(int)FrameSettingsField.FPTLForForwardOpaque];
        internal float specularGlobalDimmer => bitDatas[(int)FrameSettingsField.Reflection] ? 1f : 0f;

        internal bool BuildLightListRunsAsync() => SystemInfo.supportsAsyncCompute && bitDatas[(int)FrameSettingsField.AsyncCompute] && bitDatas[(int)FrameSettingsField.LightListAsync];
        internal bool SSRRunsAsync() => SystemInfo.supportsAsyncCompute && bitDatas[(int)FrameSettingsField.AsyncCompute] && bitDatas[(int)FrameSettingsField.SSRAsync];
        internal bool SSAORunsAsync() => SystemInfo.supportsAsyncCompute && bitDatas[(int)FrameSettingsField.AsyncCompute] && bitDatas[(int)FrameSettingsField.SSAOAsync];
        internal bool ContactShadowsRunAsync() => SystemInfo.supportsAsyncCompute && bitDatas[(int)FrameSettingsField.AsyncCompute] && bitDatas[(int)FrameSettingsField.ContactShadowsAsync];
        internal bool VolumeVoxelizationRunsAsync() => SystemInfo.supportsAsyncCompute && bitDatas[(int)FrameSettingsField.AsyncCompute] && bitDatas[(int)FrameSettingsField.VolumeVoxelizationsAsync];

        /// <summary>Override a frameSettings according to a mask.</summary>
        /// <param name="overriddenFrameSettings">Overrided FrameSettings. Must contains default data before attempting the override.</param>
        /// <param name="overridingFrameSettings">The FrameSettings data we will use for overriding.</param>
        /// <param name="frameSettingsOverideMask">The mask to use for overriding (1 means override this field).</param>
        public static void Override(ref FrameSettings overriddenFrameSettings, FrameSettings overridingFrameSettings, FrameSettingsOverrideMask frameSettingsOverideMask)
        {
            //quick override of all booleans
            overriddenFrameSettings.bitDatas = (overridingFrameSettings.bitDatas & frameSettingsOverideMask.mask) | (~frameSettingsOverideMask.mask & overriddenFrameSettings.bitDatas);
            if (frameSettingsOverideMask.mask[(uint) FrameSettingsField.LODBias])
                overriddenFrameSettings.lodBias = overridingFrameSettings.lodBias;
            if (frameSettingsOverideMask.mask[(uint) FrameSettingsField.LODBiasMode])
                overriddenFrameSettings.lodBiasMode = overridingFrameSettings.lodBiasMode;
            if (frameSettingsOverideMask.mask[(uint) FrameSettingsField.MaximumLODLevel])
                overriddenFrameSettings.maximumLODLevel = overridingFrameSettings.maximumLODLevel;
            if (frameSettingsOverideMask.mask[(uint) FrameSettingsField.MaximumLODLevelMode])
                overriddenFrameSettings.maximumLODLevelMode = overridingFrameSettings.maximumLODLevelMode;

            //override remaining values here if needed
        }

        /// <summary>Check FrameSettings with what is supported in RenderPipelineSettings and change value in order to be compatible.</summary>
        /// <param name="sanitazedFrameSettings">The FrameSettings being cleaned.</param>
        /// <param name="camera">Camera contais some necessary information to check how to sanitize.</param>
        /// <param name="renderPipelineSettings">Contains what is supported by the engine.</param>
        public static void Sanitize(ref FrameSettings sanitazedFrameSettings, Camera camera, RenderPipelineSettings renderPipelineSettings)
        {
            bool reflection = camera.cameraType == CameraType.Reflection;
            bool preview = HDUtils.IsRegularPreviewCamera(camera);
            bool sceneViewFog = CoreUtils.IsSceneViewFogEnabled(camera);

            // XRTODO: fix it
            bool stereoInstancing = camera.stereoEnabled && (XRGraphics.stereoRenderingMode == XRGraphics.StereoRenderingMode.SinglePassInstanced);

            // When rendering reflection probe we disable specular as it is view dependent
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Reflection] = !reflection;

            // We have to fall back to forward-only rendering when scene view is using wireframe rendering mode
            // as rendering everything in wireframe + deferred do not play well together
            if (GL.wireframe)
            {
                sanitazedFrameSettings.litShaderMode = LitShaderMode.Forward;
            }
            else
            {
                switch (renderPipelineSettings.supportedLitShaderMode)
                {
                    case RenderPipelineSettings.SupportedLitShaderMode.ForwardOnly:
                        sanitazedFrameSettings.litShaderMode = LitShaderMode.Forward;
                        break;
                    case RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly:
                        sanitazedFrameSettings.litShaderMode = LitShaderMode.Deferred;
                        break;
                    case RenderPipelineSettings.SupportedLitShaderMode.Both:
                        //nothing to do: keep previous value
                        break;
                }
            }

            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Shadow] &= !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ShadowMask] &= renderPipelineSettings.supportShadowMask && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ContactShadows] &= !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ScreenSpaceShadows] &= renderPipelineSettings.hdShadowInitParams.supportScreenSpaceShadows;

            //MSAA only supported in forward
            // TODO: The work will be implemented piecemeal to support all passes
            bool msaa = sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.MSAA] &= renderPipelineSettings.supportMSAA && sanitazedFrameSettings.litShaderMode == LitShaderMode.Forward;

            // No recursive reflections
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.SSR] &= !reflection && renderPipelineSettings.supportSSR && !msaa && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.SSAO] &= renderPipelineSettings.supportSSAO && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.SubsurfaceScattering] &= !reflection && renderPipelineSettings.supportSubsurfaceScattering;

            // We must take care of the scene view fog flags in the editor
            bool atmosphericScattering = sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.AtmosphericScattering] &= sceneViewFog && !preview;

            // Volumetric are disabled if there is no atmospheric scattering
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Volumetrics] &= renderPipelineSettings.supportVolumetrics && atmosphericScattering; //&& !preview induced by atmospheric scattering
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ReprojectionForVolumetrics] &= !preview;

            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.LightLayers] &= renderPipelineSettings.supportLightLayers && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ExposureControl] &= !reflection && !preview;

            // Planar and real time cubemap doesn't need post process and render in FP16
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Postprocess] &= !reflection && !preview;

            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.TransparentPrepass] &= renderPipelineSettings.supportTransparentDepthPrepass && !preview;

            bool motionVector = sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.MotionVectors] &= !reflection && renderPipelineSettings.supportMotionVectors && !preview;

            // Object motion vector are disabled if motion vector are disabled
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ObjectMotionVectors] &= motionVector && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Decals] &= renderPipelineSettings.supportDecals && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.TransparentPostpass] &= renderPipelineSettings.supportTransparentDepthPostpass && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.Distortion] &= !reflection && renderPipelineSettings.supportDistortion && !msaa && !preview;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.LowResTransparent] &= renderPipelineSettings.lowresTransparentSettings.enabled;

            bool async = sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.AsyncCompute] &= SystemInfo.supportsAsyncCompute;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.LightListAsync] &= async;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.SSRAsync] &= async;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.SSAOAsync] &= async;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ContactShadowsAsync] &= async;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.VolumeVoxelizationsAsync] &= async;

            // XRTODO: fix indirect deferred pass with instancing
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.DeferredTile] &= !stereoInstancing;
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.ComputeLightEvaluation] &= !stereoInstancing;

            // Deferred opaque are always using Fptl. Forward opaque can use Fptl or Cluster, transparent use cluster.
            // When MSAA is enabled we disable Fptl as it become expensive compare to cluster
            // In HD, MSAA is only supported for forward only rendering, no MSAA in deferred mode (for code complexity reasons)
            sanitazedFrameSettings.bitDatas[(int)FrameSettingsField.FPTLForForwardOpaque] &= !msaa;
        }

        /// <summary>Aggregation is default with override of the renderer then sanitazed depending on supported features of hdrpasset.</summary>
        /// <param name="aggregatedFrameSettings">The aggregated FrameSettings result.</param>
        /// <param name="camera">The camera rendering.</param>
        /// <param name="additionalData">Additional data of the camera rendering.</param>
        /// <param name="hdrpAsset">HDRenderPipelineAsset contening default FrameSettings.</param>
        public static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, HDAdditionalCameraData additionalData, HDRenderPipelineAsset hdrpAsset)
            => AggregateFrameSettings(
                ref aggregatedFrameSettings,
                camera,
                additionalData,
                ref hdrpAsset.GetDefaultFrameSettings(additionalData?.defaultFrameSettings ?? FrameSettingsRenderType.Camera), //fallback on Camera for SceneCamera and PreviewCamera
                hdrpAsset.currentPlatformRenderPipelineSettings
                );

        // Note: this version is the one tested as there is issue getting HDRenderPipelineAsset in batchmode in unit test framework currently.
        /// <summary>Aggregation is default with override of the renderer then sanitazed depending on supported features of hdrpasset.</summary>
        /// <param name="aggregatedFrameSettings">The aggregated FrameSettings result.</param>
        /// <param name="camera">The camera rendering.</param>
        /// <param name="additionalData">Additional data of the camera rendering.</param>
        /// <param name="defaultFrameSettings">Base framesettings to copy prior any override.</param>
        /// <param name="supportedFeatures">Currently supported feature for the sanitazation pass.</param>
        public static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, HDAdditionalCameraData additionalData, ref FrameSettings defaultFrameSettings, RenderPipelineSettings supportedFeatures)
        {
            aggregatedFrameSettings = defaultFrameSettings; //fallback on Camera for SceneCamera and PreviewCamera
            if (additionalData && additionalData.customRenderingSettings)
                Override(ref aggregatedFrameSettings, additionalData.renderingPathCustomFrameSettings, additionalData.renderingPathCustomFrameSettingsOverrideMask);
            Sanitize(ref aggregatedFrameSettings, camera, supportedFeatures);
        }

        public static bool operator ==(FrameSettings a, FrameSettings b) => a.bitDatas == b.bitDatas;
        public static bool operator !=(FrameSettings a, FrameSettings b) => a.bitDatas != b.bitDatas;
        public override bool Equals(object obj) => (obj is FrameSettings) && bitDatas.Equals(((FrameSettings)obj).bitDatas);
        public override int GetHashCode() => -1690259335 + bitDatas.GetHashCode();
    }
}
