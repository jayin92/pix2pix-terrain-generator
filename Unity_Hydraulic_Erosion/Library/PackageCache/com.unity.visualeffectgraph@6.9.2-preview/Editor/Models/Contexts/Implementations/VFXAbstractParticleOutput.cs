using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEditor.Experimental.VFX;
using UnityEngine.Experimental.VFX;

namespace UnityEditor.VFX
{
    abstract class VFXAbstractParticleOutput : VFXAbstractRenderedOutput, IVFXSubRenderer
    {
        public enum ColorMappingMode
        {
            Default,
            GradientMapped
        }

        public enum UVMode
        {
            Simple,
            Flipbook,
            FlipbookBlend,
            ScaleAndBias,
            FlipbookMotionBlend,
        }

        public enum ZWriteMode
        {
            Default,
            Off,
            On
        }
        public enum CullMode
        {
            Default,
            Front,
            Back,
            Off
        }

        public enum ZTestMode
        {
            Default,
            Less,
            Greater,
            LEqual,
            GEqual,
            Equal,
            NotEqual,
            Always
        }

        public enum SortMode
        {
            Auto,
            Off,
            On
        }

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected CullMode cullMode = CullMode.Default;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected ZWriteMode zWriteMode = ZWriteMode.Default;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected ZTestMode zTestMode = ZTestMode.Default;

        [VFXSetting, SerializeField, Tooltip("Determines how the color is handled at pixel shader"), Header("Particle Options")]
        protected ColorMappingMode colorMappingMode;

        [VFXSetting, SerializeField, Tooltip("Determines how the particle UV are handled"), FormerlySerializedAs("flipbookMode")]
        protected UVMode uvMode;

        [VFXSetting, SerializeField]
        protected bool useSoftParticle = false;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.None), SerializeField, Header("Rendering Options")]
        protected int sortPriority = 0;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected SortMode sort = SortMode.Auto;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected bool indirectDraw = false;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected bool castShadows = false;

        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        protected bool useExposureWeight = false;

        // IVFXSubRenderer interface
        public virtual bool hasShadowCasting { get { return castShadows; } }

        protected virtual bool needsExposureWeight { get { return true; } }

        private bool hasExposure { get { return needsExposureWeight && subOutput.supportsExposure; } }

        public bool HasIndirectDraw()   { return indirectDraw || HasSorting(); }
        public bool HasSorting()        { return sort == SortMode.On || (sort == SortMode.Auto && (blendMode == BlendMode.Alpha || blendMode == BlendMode.AlphaPremultiplied)); }
        int IVFXSubRenderer.sortPriority
        {
            get {
                return sortPriority;
            }
            set {
                if(sortPriority != value)
                {
                    sortPriority = value;
                    Invalidate(InvalidationCause.kSettingChanged);
                }
            }
        }
        public bool NeedsDeadListCount() { return HasIndirectDraw() && (taskType == VFXTaskType.ParticleQuadOutput || taskType == VFXTaskType.ParticleHexahedronOutput); } // Should take the capacity into account to avoid false positive

        protected VFXAbstractParticleOutput() : base(VFXDataType.Particle) {}

        public override bool codeGeneratorCompute { get { return false; } }

        public virtual bool supportsUV { get { return false; } }

        public virtual CullMode defaultCullMode { get { return CullMode.Off; } }
        public virtual ZTestMode defaultZTestMode { get { return ZTestMode.LEqual; } }

        public virtual bool supportSoftParticles { get { return useSoftParticle && !isBlendModeOpaque; } }

        protected bool usesFlipbook { get { return supportsUV && (uvMode == UVMode.Flipbook || uvMode == UVMode.FlipbookBlend || uvMode == UVMode.FlipbookMotionBlend); } }

        protected virtual IEnumerable<VFXNamedExpression> CollectGPUExpressions(IEnumerable<VFXNamedExpression> slotExpressions)
        {
            if (blendMode == BlendMode.Masked)
                yield return slotExpressions.First(o => o.name == "alphaThreshold");

            if (colorMappingMode == ColorMappingMode.GradientMapped)
            {
                yield return slotExpressions.First(o => o.name == "gradient");
            }

            if (supportSoftParticles)
            {
                var softParticleFade = slotExpressions.First(o => o.name == "softParticlesFadeDistance");
                var invSoftParticleFade = (VFXValue.Constant(1.0f) / softParticleFade.exp);
                yield return new VFXNamedExpression(invSoftParticleFade, "invSoftParticlesFadeDistance");
            }

            if (supportsUV && uvMode != UVMode.Simple)
            {
                VFXNamedExpression flipBookSizeExp;
                switch (uvMode)
                {
                    case UVMode.Flipbook:
                    case UVMode.FlipbookBlend:
                    case UVMode.FlipbookMotionBlend:
                        flipBookSizeExp = slotExpressions.First(o => o.name == "flipBookSize");
                        yield return flipBookSizeExp;
                        yield return new VFXNamedExpression(VFXValue.Constant(Vector2.one) / flipBookSizeExp.exp, "invFlipBookSize");
                        if (uvMode == UVMode.FlipbookMotionBlend)
                        {
                            yield return slotExpressions.First(o => o.name == "motionVectorMap");
                            yield return slotExpressions.First(o => o.name == "motionVectorScale");
                        }
                        break;
                    case UVMode.ScaleAndBias:
                        yield return slotExpressions.First(o => o.name == "uvScale");
                        yield return slotExpressions.First(o => o.name == "uvBias");
                        break;
                    default: throw new NotImplementedException("Unimplemented UVMode: " + uvMode);
                }
            }

            if (hasExposure && useExposureWeight)
                yield return slotExpressions.First(o => o.name == "exposureWeight");
        }

        public override VFXExpressionMapper GetExpressionMapper(VFXDeviceTarget target)
        {
            if (target == VFXDeviceTarget.GPU)
            {
                var gpuMapper = VFXExpressionMapper.FromBlocks(activeFlattenedChildrenWithImplicit);
                gpuMapper.AddExpressions(CollectGPUExpressions(GetExpressionsFromSlots(this)), -1);
                return gpuMapper;
            }
            return new VFXExpressionMapper();
        }

        public class InputPropertiesGradientMapped
        {
            [Tooltip("The gradient used to sample color")]
            public Gradient gradient = VFXResources.defaultResources.gradientMapRamp;
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get
            {
                foreach (var property in PropertiesFromType(GetInputPropertiesTypeName()))
                    yield return property;

                if(colorMappingMode == ColorMappingMode.GradientMapped)
                {
                    foreach(var property in PropertiesFromType("InputPropertiesGradientMapped"))
                        yield return property;
                }

                if (supportsUV && uvMode != UVMode.Simple)
                {
                    switch (uvMode)
                    {
                        case UVMode.Flipbook:
                        case UVMode.FlipbookBlend:
                        case UVMode.FlipbookMotionBlend:
                            yield return new VFXPropertyWithValue(new VFXProperty(typeof(Vector2), "flipBookSize"), new Vector2(4, 4));
                            if(uvMode == UVMode.FlipbookMotionBlend)
                            {
                                yield return new VFXPropertyWithValue(new VFXProperty(typeof(Texture2D), "motionVectorMap"));
                                yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "motionVectorScale"), 1.0f);
                            }
                            break;
                        case UVMode.ScaleAndBias:
                            yield return new VFXPropertyWithValue(new VFXProperty(typeof(Vector2), "uvScale"), Vector2.one);
                            yield return new VFXPropertyWithValue(new VFXProperty(typeof(Vector2), "uvBias"), Vector2.zero);
                            break;
                        default: throw new NotImplementedException("Unimplemented UVMode: " + uvMode);
                    }
                }

                if (blendMode == BlendMode.Masked)
                    yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "alphaThreshold", VFXPropertyAttribute.Create(new RangeAttribute(0.0f, 1.0f))), 0.5f);
                if (supportSoftParticles)
                    yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "softParticlesFadeDistance", VFXPropertyAttribute.Create(new MinAttribute(0.001f))), 1.0f);

                if (hasExposure && useExposureWeight)
                    yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "exposureWeight", VFXPropertyAttribute.Create(new RangeAttribute(0.0f, 1.0f))), 1.0f);
            }
        }

        public override IEnumerable<string> additionalDefines
        {
            get
            {
                switch(colorMappingMode)
                {
                    case ColorMappingMode.Default:
                        yield return "VFX_COLORMAPPING_DEFAULT";
                        break;
                    case ColorMappingMode.GradientMapped:
                        yield return "VFX_COLORMAPPING_GRADIENTMAPPED";
                        break;
                }

                if (isBlendModeOpaque)
                    yield return "IS_OPAQUE_PARTICLE";
                else
                    yield return "IS_TRANSPARENT_PARTICLE";

                if (blendMode == BlendMode.Masked)
                    yield return "USE_ALPHA_TEST";
                if (supportSoftParticles)
                    yield return "USE_SOFT_PARTICLE";

                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        yield return "VFX_BLENDMODE_ALPHA";
                        break;
                    case BlendMode.Additive:
                        yield return "VFX_BLENDMODE_ADD";
                        break;
                    case BlendMode.AlphaPremultiplied:
                        yield return "VFX_BLENDMODE_PREMULTIPLY";
                        break;
                }

                VisualEffectResource asset = GetResource();
                if (asset != null)
                {
                    var settings = asset.rendererSettings;
                    if (settings.motionVectorGenerationMode == MotionVectorGenerationMode.Object)
                        yield return "USE_MOTION_VECTORS_PASS";
                    if (hasShadowCasting)
                        yield return "USE_CAST_SHADOWS_PASS";
                }

                if (HasIndirectDraw())
                    yield return "VFX_HAS_INDIRECT_DRAW";

                if (supportsUV && uvMode != UVMode.Simple)
                {
                    switch (uvMode)
                    {
                        case UVMode.Flipbook:
                            yield return "USE_FLIPBOOK";
                            break;
                        case UVMode.FlipbookBlend:
                            yield return "USE_FLIPBOOK";
                            yield return "USE_FLIPBOOK_INTERPOLATION";
                            break;
                        case UVMode.FlipbookMotionBlend:
                            yield return "USE_FLIPBOOK";
                            yield return "USE_FLIPBOOK_INTERPOLATION";
                            yield return "USE_FLIPBOOK_MOTIONVECTORS";
                            break;
                        case UVMode.ScaleAndBias:
                            yield return "USE_UV_SCALE_BIAS";
                            break;
                        default: throw new NotImplementedException("Unimplemented UVMode: " + uvMode);
                    }
                }

                if (hasExposure && useExposureWeight)
                    yield return "USE_EXPOSURE_WEIGHT";

                if (NeedsDeadListCount() && GetData().IsAttributeStored(VFXAttribute.Alive)) //Actually, there are still corner cases, e.g.: particles spawning immortal particles through GPU Event
                    yield return "USE_DEAD_LIST_COUNT";
            }
        }

        protected override IEnumerable<string> filteredOutSettings
        {
            get
            {
                if (!supportsUV)
                    yield return "uvMode";

                if (isBlendModeOpaque)
                {
                    yield return "useSoftParticle";
                }

                if (!hasExposure)
                    yield return "useExposureWeight";
            }
        }

        public override IEnumerable<KeyValuePair<string, VFXShaderWriter>> additionalReplacements
        {
            get
            {
                yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputRenderState}", renderState);

                var shaderTags = new VFXShaderWriter();
                var renderQueueStr = subOutput.GetRenderQueueStr();
                var renderTypeStr = isBlendModeOpaque ? "Opaque" : "Transparent";

                shaderTags.Write(string.Format("Tags {{ \"Queue\"=\"{0}\" \"IgnoreProjector\"=\"{1}\" \"RenderType\"=\"{2}\" }}", renderQueueStr, !isBlendModeOpaque, renderTypeStr));
                yield return new KeyValuePair<string, VFXShaderWriter>("${VFXShaderTags}", shaderTags);
            }
        }

        protected virtual VFXShaderWriter renderState
        {
            get
            {
                var rs = new VFXShaderWriter();

                WriteBlendMode(rs);

                var zTest = zTestMode;
                if (zTest == ZTestMode.Default)
                    zTest = defaultZTestMode;

                switch (zTest)
                {
                    case ZTestMode.Default: rs.WriteLine("ZTest LEqual"); break;
                    case ZTestMode.Always: rs.WriteLine("ZTest Always"); break;
                    case ZTestMode.Equal: rs.WriteLine("ZTest Equal"); break;
                    case ZTestMode.GEqual: rs.WriteLine("ZTest GEqual"); break;
                    case ZTestMode.Greater: rs.WriteLine("ZTest Greater"); break;
                    case ZTestMode.LEqual: rs.WriteLine("ZTest LEqual"); break;
                    case ZTestMode.Less: rs.WriteLine("ZTest Less"); break;
                    case ZTestMode.NotEqual: rs.WriteLine("ZTest NotEqual"); break;
                }

                switch (zWriteMode)
                {
                    case ZWriteMode.Default:
                        if (isBlendModeOpaque)
                            rs.WriteLine("ZWrite On");
                        else
                            rs.WriteLine("ZWrite Off");
                        break;
                    case ZWriteMode.On: rs.WriteLine("ZWrite On"); break;
                    case ZWriteMode.Off: rs.WriteLine("ZWrite Off"); break;
                }

                var cull = cullMode;
                if (cull == CullMode.Default)
                    cull = defaultCullMode;

                switch (cull)
                {
                    case CullMode.Default: rs.WriteLine("Cull Off"); break;
                    case CullMode.Front: rs.WriteLine("Cull Front"); break;
                    case CullMode.Back: rs.WriteLine("Cull Back"); break;
                    case CullMode.Off: rs.WriteLine("Cull Off"); break;
                }

                return rs;
            }
        }

        public override IEnumerable<VFXMapping> additionalMappings
        {
            get
            {
                yield return new VFXMapping("sortPriority", sortPriority);
                if (HasIndirectDraw())
                    yield return new VFXMapping("indirectDraw", 1);
            }
        }
    }
}
