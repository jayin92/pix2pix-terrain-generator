using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using CED = CoreEditorDrawer<SerializedHDLight>;

    static partial class HDLightUI
    {
        // LightType + LightTypeExtent combined
        internal enum LightShape
        {
            Spot,
            Directional,
            Point,
            //Area, <= offline base type not displayed in our case but used for GI of our area light
            Rectangle,
            Tube,
            //Sphere,
            //Disc,
        }

        internal enum DirectionalLightUnit
        {
            Lux = LightUnit.Lux,
        }

        internal enum AreaLightUnit
        {
            Lumen = LightUnit.Lumen,
            Luminance = LightUnit.Luminance,
            Ev100 = LightUnit.Ev100,
        }

        internal enum PunctualLightUnit
        {
            Lumen = LightUnit.Lumen,
            Candela = LightUnit.Candela,
            Lux = LightUnit.Lux,
            Ev100 = LightUnit.Ev100
        }

        enum ShadowmaskMode
        {
            ShadowMask,
            DistanceShadowmask
        }

        enum Expandable
        {
            General = 1 << 0,
            Shape = 1 << 1,
            Emission = 1 << 2,
            Volumetric = 1 << 3,
            Shadows = 1 << 4,
            ShadowMap = 1 << 5,
            ContactShadow = 1 << 6,
            BakedShadow = 1 << 7,
            ShadowQuality = 1 << 8
        }

        enum Advanceable
        {
            General = 1 << 0,
            //Shape = 1 << 1, //not used anymore
            Emission = 1 << 2,
            Shadow = 1 << 3,
        }

        const float k_MinLightSize = 0.01f; // Provide a small size of 1cm for line light

        readonly static ExpandedState<Expandable, Light> k_ExpandedState = new ExpandedState<Expandable, Light>(~(-1), "HDRP");

        public static readonly CED.IDrawer Inspector;

        static bool GetAdvanced(Advanceable mask, SerializedHDLight serialized, Editor owner)
        {
            return (serialized.serializedLightData.showAdditionalSettings.intValue & (int)mask) != 0;
        }

        static void SetAdvanced(Advanceable mask, bool value, SerializedHDLight serialized, Editor owner)
        {
            if (value)
            {
                serialized.serializedLightData.showAdditionalSettings.intValue |= (int)mask;
            }
            else
            {
                serialized.serializedLightData.showAdditionalSettings.intValue &= ~(int)mask;
            }
        }

        static void SwitchAdvanced(Advanceable mask, SerializedHDLight serialized, Editor owner)
        {
            if ((serialized.serializedLightData.showAdditionalSettings.intValue & (int)mask) != 0)
            {
                serialized.serializedLightData.showAdditionalSettings.intValue &= ~(int)mask;
            }
            else
            {
                serialized.serializedLightData.showAdditionalSettings.intValue |= (int)mask;
            }
        }

        static Action<GUIContent, SerializedProperty, LightEditor.Settings> SliderWithTexture;

        static HDLightUI()
        {
            Inspector = CED.Group(
                CED.AdvancedFoldoutGroup(s_Styles.generalHeader, Expandable.General, k_ExpandedState,
                    (serialized, owner) => GetAdvanced(Advanceable.General, serialized, owner),
                    (serialized, owner) => SwitchAdvanced(Advanceable.General, serialized, owner),
                    DrawGeneralContent,
                    DrawGeneralAdvancedContent
                    ),
                CED.FoldoutGroup(s_Styles.shapeHeader, Expandable.Shape, k_ExpandedState, DrawShapeContent),
                CED.AdvancedFoldoutGroup(s_Styles.emissionHeader, Expandable.Emission, k_ExpandedState,
                    (serialized, owner) => GetAdvanced(Advanceable.Emission, serialized, owner),
                    (serialized, owner) => SwitchAdvanced(Advanceable.Emission, serialized, owner),
                    DrawEmissionContent,
                    DrawEmissionAdvancedContent
                    ),
                CED.FoldoutGroup(s_Styles.volumetricHeader, Expandable.Volumetric, k_ExpandedState, DrawVolumetric),
                CED.Conditional((serialized, owner) => serialized.editorLightShape != LightShape.Tube,
                    CED.AdvancedFoldoutGroup(s_Styles.shadowHeader, Expandable.Shadows, k_ExpandedState,
                        (serialized, owner) => GetAdvanced(Advanceable.Shadow, serialized, owner),
                        (serialized, owner) => SwitchAdvanced(Advanceable.Shadow, serialized, owner),
                        CED.Group(
                            CED.FoldoutGroup(s_Styles.shadowMapSubHeader, Expandable.ShadowMap, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent | FoldoutOption.NoSpaceAtEnd, DrawShadowMapContent),
                            CED.Conditional((serialized, owner) => GetAdvanced(Advanceable.Shadow, serialized, owner) && k_ExpandedState[Expandable.ShadowMap],
                                CED.Group(GroupOption.Indent, DrawShadowMapAdvancedContent)),
                            CED.space,
                            // Very High setting
                            CED.Conditional((serialized, owner) => HasShadowQualitySettingsUI(HDShadowQuality.VeryHigh, serialized, owner),
                                CED.FoldoutGroup(s_Styles.veryHighShadowQualitySubHeader, Expandable.ShadowQuality, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent, DrawVeryHighShadowSettingsContent)),
                            // High setting
                            CED.Conditional((serialized, owner) => HasShadowQualitySettingsUI(HDShadowQuality.High, serialized, owner),
                                CED.FoldoutGroup(s_Styles.highShadowQualitySubHeader, Expandable.ShadowQuality, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent, DrawHighShadowSettingsContent)),
                            CED.Conditional((serialized, owner) => HasShadowQualitySettingsUI(HDShadowQuality.Medium, serialized, owner),
                                CED.FoldoutGroup(s_Styles.mediumShadowQualitySubHeader, Expandable.ShadowQuality, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent, DrawMediumShadowSettingsContent)),
                            CED.Conditional((serialized, owner) => HasShadowQualitySettingsUI(HDShadowQuality.Low, serialized, owner),
                                CED.FoldoutGroup(s_Styles.lowShadowQualitySubHeader, Expandable.ShadowQuality, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent, DrawLowShadowSettingsContent)),

                            CED.Conditional((serialized, owner) => serialized.editorLightShape != LightShape.Rectangle && serialized.editorLightShape != LightShape.Tube,
                                CED.FoldoutGroup(s_Styles.contactShadowsSubHeader, Expandable.ContactShadow, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent | FoldoutOption.NoSpaceAtEnd, DrawContactShadowsContent)
                            ),
                            CED.Conditional((serialized, owner) => (serialized.settings.isBakedOrMixed || serialized.settings.isCompletelyBaked) && serialized.editorLightShape != LightShape.Rectangle,
                                CED.space,
                                CED.FoldoutGroup(s_Styles.bakedShadowsSubHeader, Expandable.BakedShadow, k_ExpandedState, FoldoutOption.SubFoldout | FoldoutOption.Indent | FoldoutOption.NoSpaceAtEnd, DrawBakedShadowsContent))
                            ),
                        CED.noop //will only add parameter in first sub header
                        )
                    )
            );

            //quicker than standard reflection as it is compiled
            var paramLabel = Expression.Parameter(typeof(GUIContent), "label");
            var paramProperty = Expression.Parameter(typeof(SerializedProperty), "property");
            var paramSettings = Expression.Parameter(typeof(LightEditor.Settings), "settings");
            System.Reflection.MethodInfo sliderWithTextureInfo = typeof(EditorGUILayout)
                .GetMethod(
                    "SliderWithTexture",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                    null,
                    System.Reflection.CallingConventions.Any,
                    new[] { typeof(GUIContent), typeof(SerializedProperty), typeof(float), typeof(float), typeof(float), typeof(Texture2D), typeof(GUILayoutOption[]) },
                    null);
            var sliderWithTextureCall = Expression.Call(
                sliderWithTextureInfo,
                paramLabel,
                paramProperty,
                Expression.Constant((float)typeof(LightEditor.Settings).GetField("kMinKelvin", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetRawConstantValue()),
                Expression.Constant((float)typeof(LightEditor.Settings).GetField("kMaxKelvin", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetRawConstantValue()),
                Expression.Constant((float)typeof(LightEditor.Settings).GetField("kSliderPower", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetRawConstantValue()),
                Expression.Field(paramSettings, typeof(LightEditor.Settings).GetField("m_KelvinGradientTexture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)),
                Expression.Constant(null, typeof(GUILayoutOption[])));
            var lambda = Expression.Lambda<Action<GUIContent, SerializedProperty, LightEditor.Settings>>(sliderWithTextureCall, paramLabel, paramProperty, paramSettings);
            SliderWithTexture = lambda.Compile();
        }

        static void DrawGeneralContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUI.BeginChangeCheck();
            serialized.editorLightShape = (LightShape)EditorGUILayout.Popup(s_Styles.shape, (int)serialized.editorLightShape, s_Styles.shapeNames);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyEditorLightShape(serialized, owner);
                UpdateLightIntensityUnit(serialized, owner);

                // For GI we need to detect any change on additional data and call SetLightDirty + For intensity we need to detect light shape change
                serialized.needUpdateAreaLightEmissiveMeshComponents = true;
                ((Light)owner.target).SetLightDirty(); // Should be apply only to parameter that's affect GI, but make the code cleaner
            }

            serialized.settings.DrawLightmapping();
        }

        static void DrawGeneralAdvancedContent(SerializedHDLight serialized, Editor owner)
        {
            using (new EditorGUI.DisabledScope(!HDUtils.hdrpSettings.supportLightLayers))
            {
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    HDEditorUtils.LightLayerMaskPropertyDrawer(s_Styles.lightLayer, serialized.serializedLightData.lightlayersMask);

                    // If we're not in decoupled mode for light layers, we sync light with shadow layers:
                    if (serialized.serializedLightData.linkLightLayers.boolValue && change.changed)
                        SyncLightAndShadowLayers(serialized, owner);
                }
            }
        }

        static void DrawShapeContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUI.BeginChangeCheck(); // For GI we need to detect any change on additional data and call SetLightDirty + For intensity we need to detect light shape change

            // LightShape is HD specific, it need to drive LightType from the original LightType
            // when it make sense, so the GI is still in sync with the light shape
            switch (serialized.editorLightShape)
            {
                case LightShape.Directional:
                    EditorGUILayout.PropertyField(serialized.serializedLightData.maxSmoothness, s_Styles.maxSmoothness);
                    break;

                case LightShape.Point:
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeRadius, s_Styles.lightRadius);
                    EditorGUILayout.PropertyField(serialized.serializedLightData.maxSmoothness, s_Styles.maxSmoothness);
                    break;

                case LightShape.Spot:
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serialized.serializedLightData.spotLightShape, s_Styles.spotLightShape);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateLightIntensityUnit(serialized, owner);
                    }

                    if (serialized.serializedLightData.spotLightShape.hasMultipleDifferentValues)
                    {
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.LabelField("Multiple different spot Shapes in selection");
                    }
                    else
                    {
                        var spotLightShape = (SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex;
                        switch (spotLightShape)
                        {
                            case SpotLightShape.Box:
                                // Box directional light.
                                EditorGUILayout.PropertyField(serialized.serializedLightData.shapeWidth, s_Styles.shapeWidthBox);
                                EditorGUILayout.PropertyField(serialized.serializedLightData.shapeHeight, s_Styles.shapeHeightBox);
                                break;
                            case SpotLightShape.Cone:
                                // Cone spot projector
                                EditorGUILayout.Slider(serialized.settings.spotAngle, 1f, 179f, s_Styles.outterAngle);
                                EditorGUILayout.Slider(serialized.serializedLightData.spotInnerPercent, 0f, 100f, s_Styles.spotInnerPercent);
                                EditorGUILayout.PropertyField(serialized.serializedLightData.shapeRadius, s_Styles.lightRadius);
                                EditorGUILayout.PropertyField(serialized.serializedLightData.maxSmoothness, s_Styles.maxSmoothness);
                                break;
                            case SpotLightShape.Pyramid:
                                // pyramid spot projector
                                serialized.settings.DrawSpotAngle();
                                EditorGUILayout.Slider(serialized.serializedLightData.aspectRatio, 0.05f, 20.0f, s_Styles.aspectRatioPyramid);
                                EditorGUILayout.PropertyField(serialized.serializedLightData.shapeRadius, s_Styles.lightRadius);
                                EditorGUILayout.PropertyField(serialized.serializedLightData.maxSmoothness, s_Styles.maxSmoothness);
                                break;
                            default:
                                Debug.Assert(false, "Not implemented light type");
                                break;
                        }
                    }
                    break;

                case LightShape.Rectangle:
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeWidth, s_Styles.shapeWidthRect);
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeHeight, s_Styles.shapeHeightRect);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serialized.settings.areaSizeX.floatValue = serialized.serializedLightData.shapeWidth.floatValue;
                        serialized.settings.areaSizeY.floatValue = serialized.serializedLightData.shapeHeight.floatValue;
                    }
                    break;

                case LightShape.Tube:
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeWidth, s_Styles.shapeWidthTube);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // Fake line with a small rectangle in vanilla unity for GI
                        serialized.settings.areaSizeX.floatValue = serialized.serializedLightData.shapeWidth.floatValue;
                        serialized.settings.areaSizeY.floatValue = k_MinLightSize;
                    }
                    break;

                case (LightShape)(-1):
                    // don't do anything, this is just to handle multi selection
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.LabelField("Multiple different Types in selection");
                    break;

                default:
                    Debug.Assert(false, "Not implemented light type");
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                // Light size must be non-zero, else we get NaNs.
                serialized.serializedLightData.shapeWidth.floatValue = Mathf.Max(serialized.serializedLightData.shapeWidth.floatValue, k_MinLightSize);
                serialized.serializedLightData.shapeHeight.floatValue = Mathf.Max(serialized.serializedLightData.shapeHeight.floatValue, k_MinLightSize);
                serialized.serializedLightData.shapeRadius.floatValue = Mathf.Max(serialized.serializedLightData.shapeRadius.floatValue, 0.0f);
                serialized.needUpdateAreaLightEmissiveMeshComponents = true;
                ((Light)owner.target).SetLightDirty(); // Should be apply only to parameter that's affect GI, but make the code cleaner
            }
        }

        static void UpdateLightIntensityUnit(SerializedHDLight serialized, Editor owner)
        {
            // Box are local directional light
            if (serialized.editorLightShape == LightShape.Directional ||
                (serialized.editorLightShape == LightShape.Spot && ((SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex == SpotLightShape.Box)))
            {
                serialized.serializedLightData.lightUnit.enumValueIndex = (int)DirectionalLightUnit.Lux;
                // We need to reset luxAtDistance to neutral when changing to (local) directional light, otherwise first display value ins't correct
                serialized.serializedLightData.luxAtDistance.floatValue = 1.0f;
            }
            else
                serialized.serializedLightData.lightUnit.enumValueIndex = (int)LightUnit.Lumen;
        }

        static void DrawLightIntensityUnitPopup(SerializedHDLight serialized, Editor owner)
        {
            LightShape shape = serialized.editorLightShape;
            LightUnit selectedLightUnit;
            LightUnit oldLigthUnit = (LightUnit)serialized.serializedLightData.lightUnit.enumValueIndex;

            EditorGUI.showMixedValue = serialized.serializedLightData.lightUnit.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            switch (shape)
            {
                case LightShape.Directional:
                    selectedLightUnit = (LightUnit)EditorGUILayout.EnumPopup((DirectionalLightUnit)serialized.serializedLightData.lightUnit.enumValueIndex);
                    break;
                case LightShape.Point:
                    selectedLightUnit = (LightUnit)EditorGUILayout.EnumPopup((PunctualLightUnit)serialized.serializedLightData.lightUnit.enumValueIndex);
                    break;
                case LightShape.Spot:
                    if ((SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex == SpotLightShape.Box)
                        selectedLightUnit = (LightUnit)EditorGUILayout.EnumPopup((DirectionalLightUnit)serialized.serializedLightData.lightUnit.enumValueIndex);
                    else
                        selectedLightUnit = (LightUnit)EditorGUILayout.EnumPopup((PunctualLightUnit)serialized.serializedLightData.lightUnit.enumValueIndex);
                    break;
                default:
                    selectedLightUnit = (LightUnit)EditorGUILayout.EnumPopup((AreaLightUnit)serialized.serializedLightData.lightUnit.enumValueIndex);
                    break;
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                ConvertLightIntensity(oldLigthUnit, selectedLightUnit, serialized, owner);
                serialized.serializedLightData.lightUnit.enumValueIndex = (int)selectedLightUnit;
            }
        }

        static void ConvertLightIntensity(LightUnit oldLightUnit, LightUnit newLightUnit, SerializedHDLight serialized, Editor owner)
        {
            float intensity = serialized.serializedLightData.intensity.floatValue;
            Light light = (Light)owner.target;

            // For punctual lights
            if ((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex == LightTypeExtent.Punctual)
            {
                // Lumen ->
                if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Candela)
                    intensity = LightUtils.ConvertPunctualLightLumenToCandela(light.type, intensity, light.intensity, serialized.serializedLightData.enableSpotReflector.boolValue);
                else if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Lux)
                    intensity = LightUtils.ConvertPunctualLightLumenToLux(light.type, intensity, light.intensity, serialized.serializedLightData.enableSpotReflector.boolValue,
                                                                            serialized.serializedLightData.luxAtDistance.floatValue);
                else if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Ev100)
                    intensity = LightUtils.ConvertPunctualLightLumenToEv(light.type, intensity, light.intensity, serialized.serializedLightData.enableSpotReflector.boolValue);
                // Candela ->
                else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Lumen)
                    intensity = LightUtils.ConvertPunctualLightCandelaToLumen(  light.type, (SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex, intensity, serialized.serializedLightData.enableSpotReflector.boolValue,
                                                                                light.spotAngle, serialized.serializedLightData.aspectRatio.floatValue);
                else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Lux)
                    intensity = LightUtils.ConvertCandelaToLux(intensity, serialized.serializedLightData.luxAtDistance.floatValue);
                else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Ev100)
                    intensity = LightUtils.ConvertCandelaToEv(intensity);
                // Lux ->
                else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Lumen)
                    intensity = LightUtils.ConvertPunctualLightLuxToLumen(light.type, (SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex, intensity, serialized.serializedLightData.enableSpotReflector.boolValue,
                                                                          light.spotAngle, serialized.serializedLightData.aspectRatio.floatValue, serialized.serializedLightData.luxAtDistance.floatValue);
                else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Candela)
                    intensity = LightUtils.ConvertLuxToCandela(intensity, serialized.serializedLightData.luxAtDistance.floatValue);
                else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Ev100)
                    intensity = LightUtils.ConvertLuxToEv(intensity, serialized.serializedLightData.luxAtDistance.floatValue);
                // EV100 ->
                else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lumen)
                    intensity = LightUtils.ConvertPunctualLightEvToLumen(light.type, (SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex, intensity, serialized.serializedLightData.enableSpotReflector.boolValue,
                                                                            light.spotAngle, serialized.serializedLightData.aspectRatio.floatValue);
                else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Candela)
                    intensity = LightUtils.ConvertEvToCandela(intensity);
                else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lux)
                    intensity = LightUtils.ConvertEvToLux(intensity, serialized.serializedLightData.luxAtDistance.floatValue);
            }
            else  // For area lights
            {               
                if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Luminance)
                    intensity = LightUtils.ConvertAreaLightLumenToLuminance((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex, intensity, serialized.serializedLightData.shapeWidth.floatValue, serialized.serializedLightData.shapeHeight.floatValue);
                if (oldLightUnit == LightUnit.Luminance && newLightUnit == LightUnit.Lumen)
                    intensity = LightUtils.ConvertAreaLightLuminanceToLumen((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex, intensity, serialized.serializedLightData.shapeWidth.floatValue, serialized.serializedLightData.shapeHeight.floatValue);
                if (oldLightUnit == LightUnit.Luminance && newLightUnit == LightUnit.Ev100)
                    intensity = LightUtils.ConvertLuminanceToEv(intensity);
                if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Luminance)
                    intensity = LightUtils.ConvertEvToLuminance(intensity);
                if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lumen)
                    intensity = LightUtils.ConvertAreaLightEvToLumen((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex, intensity, serialized.serializedLightData.shapeWidth.floatValue, serialized.serializedLightData.shapeHeight.floatValue);
                if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Ev100)
                    intensity = LightUtils.ConvertAreaLightLumenToEv((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex, intensity, serialized.serializedLightData.shapeWidth.floatValue, serialized.serializedLightData.shapeHeight.floatValue);
            }

            serialized.serializedLightData.intensity.floatValue = intensity;
        }

        static void DrawEmissionContent(SerializedHDLight serialized, Editor owner)
        {
            using (var changes = new EditorGUI.ChangeCheckScope())
            {
                if (GraphicsSettings.lightsUseLinearIntensity && GraphicsSettings.lightsUseColorTemperature)
                {
                    EditorGUILayout.PropertyField(serialized.settings.useColorTemperature, s_Styles.useColorTemperature);
                    if (serialized.settings.useColorTemperature.boolValue)
                    {
                        EditorGUI.indentLevel += 1;
                        EditorGUILayout.PropertyField(serialized.settings.color, s_Styles.colorFilter);
                        SliderWithTexture(s_Styles.colorTemperature, serialized.settings.colorTemperature, serialized.settings);
                        EditorGUI.indentLevel -= 1;
                    }
                    else
                        EditorGUILayout.PropertyField(serialized.settings.color, s_Styles.color);
                }
                else
                    EditorGUILayout.PropertyField(serialized.settings.color, s_Styles.color);
                
                if (changes.changed && HDRenderPipelinePreferences.lightColorNormalization)
                    serialized.settings.color.colorValue = HDUtils.NormalizeColor(serialized.settings.color.colorValue);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serialized.serializedLightData.intensity, s_Styles.lightIntensity);
            DrawLightIntensityUnitPopup(serialized, owner);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                serialized.serializedLightData.intensity.floatValue = Mathf.Max(serialized.serializedLightData.intensity.floatValue, 0.0f);
            }

            if (serialized.editorLightShape != LightShape.Directional && serialized.serializedLightData.lightUnit.enumValueIndex == (int)PunctualLightUnit.Lux)
            {
                // Box are local directional light and shouldn't display the Lux At widget. It use only lux
                if (!(serialized.editorLightShape == LightShape.Spot && ((SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex == SpotLightShape.Box)))
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serialized.serializedLightData.luxAtDistance, s_Styles.luxAtDistance);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serialized.serializedLightData.luxAtDistance.floatValue = Mathf.Max(serialized.serializedLightData.luxAtDistance.floatValue, 0.01f);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            if (serialized.editorLightShape == LightShape.Spot)
            {
                var spotLightShape = (SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex;
                if (spotLightShape == SpotLightShape.Cone || spotLightShape == SpotLightShape.Pyramid)
                {
                    // Display reflector only in advance mode
                    if (serialized.serializedLightData.lightUnit.enumValueIndex == (int)PunctualLightUnit.Lumen && GetAdvanced(Advanceable.Emission, serialized, owner))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialized.serializedLightData.enableSpotReflector, s_Styles.enableSpotReflector);
                        EditorGUI.indentLevel--;
                    }
                }
            }

            if (serialized.editorLightShape != LightShape.Directional)
            {
                EditorGUI.BeginChangeCheck();
                serialized.settings.DrawRange(false);
                if (EditorGUI.EndChangeCheck())
                {
                    // For GI we need to detect any change on additional data and call SetLightDirty + For intensity we need to detect light shape change
                    serialized.needUpdateAreaLightEmissiveMeshComponents = true;
                    ((Light)owner.target).SetLightDirty(); // Should be apply only to parameter that's affect GI, but make the code cleaner
                }
            }

            serialized.settings.DrawBounceIntensity();

            EditorGUI.BeginChangeCheck(); // For GI we need to detect any change on additional data and call SetLightDirty

            // No cookie with area light (maybe in future textured area light ?)
            if (!HDAdditionalLightData.IsAreaLight(serialized.serializedLightData.lightTypeExtent))
            {
                serialized.settings.DrawCookie();

                // When directional light use a cookie, it can control the size
                if (serialized.settings.cookie != null && serialized.editorLightShape == LightShape.Directional)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeWidth, s_Styles.cookieSizeX);
                    EditorGUILayout.PropertyField(serialized.serializedLightData.shapeHeight, s_Styles.cookieSizeY);
                    EditorGUI.indentLevel--;
                }
            }
            else if ((LightTypeExtent)serialized.serializedLightData.lightTypeExtent.enumValueIndex == LightTypeExtent.Rectangle)
            {
                EditorGUILayout.ObjectField( serialized.serializedLightData.areaLightCookie, s_Styles.areaLightCookie );
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serialized.needUpdateAreaLightEmissiveMeshComponents = true;
                ((Light)owner.target).SetLightDirty(); // Should be apply only to parameter that's affect GI, but make the code cleaner
            }
        }

        static void DrawEmissionAdvancedContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUI.BeginChangeCheck(); // For GI we need to detect any change on additional data and call SetLightDirty

            EditorGUILayout.PropertyField(serialized.serializedLightData.affectDiffuse, s_Styles.affectDiffuse);
            EditorGUILayout.PropertyField(serialized.serializedLightData.affectSpecular, s_Styles.affectSpecular);
            if (serialized.editorLightShape != LightShape.Directional)
            {
                if ((SpotLightShape)serialized.serializedLightData.spotLightShape.enumValueIndex != SpotLightShape.Box)
                    EditorGUILayout.PropertyField(serialized.serializedLightData.applyRangeAttenuation, s_Styles.applyRangeAttenuation);
                EditorGUILayout.PropertyField(serialized.serializedLightData.fadeDistance, s_Styles.fadeDistance);
            }
            EditorGUILayout.PropertyField(serialized.serializedLightData.lightDimmer, s_Styles.lightDimmer);

            // Emissive mesh for area light only
            if (HDAdditionalLightData.IsAreaLight(serialized.serializedLightData.lightTypeExtent))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serialized.serializedLightData.displayAreaLightEmissiveMesh, s_Styles.displayAreaLightEmissiveMesh);
                if (EditorGUI.EndChangeCheck())
                    serialized.needUpdateAreaLightEmissiveMeshComponents = true;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serialized.needUpdateAreaLightEmissiveMeshComponents = true;
                serialized.serializedLightData.fadeDistance.floatValue = Mathf.Max(serialized.serializedLightData.fadeDistance.floatValue, 0.01f);
                ((Light)owner.target).SetLightDirty(); // Should be apply only to parameter that's affect GI, but make the code cleaner
            }
        }

        static void DrawVolumetric(SerializedHDLight serialized, Editor owner)
        {
            EditorGUILayout.PropertyField(serialized.serializedLightData.useVolumetric, s_Styles.volumetricEnable);
            using (new EditorGUI.DisabledScope(!serialized.serializedLightData.useVolumetric.boolValue))
            {
                EditorGUILayout.PropertyField(serialized.serializedLightData.volumetricDimmer, s_Styles.volumetricDimmer);
                EditorGUILayout.Slider(serialized.serializedShadowData.volumetricShadowDimmer, 0.0f, 1.0f, s_Styles.volumetricShadowDimmer);
            }
        }

        static void DrawShadowMapContent(SerializedHDLight serialized, Editor owner)
        {
            bool oldShadowEnabled = serialized.settings.shadowsType.enumValueIndex != 0;
            bool newShadowsEnabled = EditorGUILayout.Toggle(s_Styles.enableShadowMap, oldShadowEnabled);
            if (oldShadowEnabled ^ newShadowsEnabled)
            {
                serialized.settings.shadowsType.enumValueIndex = newShadowsEnabled ? (int)LightShadows.Hard : (int)LightShadows.None;
            }

            using (new EditorGUI.DisabledScope(!newShadowsEnabled))
            {
                {
#if ENABLE_RAYTRACING
                    // For the moment, we only support screen space rasterized shadows for directional lights
                    if (serialized.settings.lightType.enumValueIndex == (int)LightType.Directional)
                    {
                        EditorGUILayout.PropertyField(serialized.serializedLightData.useScreenSpaceShadows, s_Styles.useScreenSpaceShadows);
                    }
#endif
                    using (var change = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUILayout.DelayedIntField(serialized.serializedShadowData.resolution, s_Styles.shadowResolution);
                        if (change.changed)
                            serialized.serializedShadowData.resolution.intValue = Mathf.Max(HDShadowManager.k_MinShadowMapResolution, serialized.serializedShadowData.resolution.intValue);
                    }
                    EditorGUILayout.Slider(serialized.serializedLightData.shadowNearPlane, HDShadowUtils.k_MinShadowNearPlane, 10f, s_Styles.shadowNearPlane);

                    if (serialized.settings.isMixed)
                    {
                        using (new EditorGUI.DisabledScope(!(GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).currentPlatformRenderPipelineSettings.supportShadowMask))
                        {
                            EditorGUI.showMixedValue = serialized.serializedLightData.nonLightmappedOnly.hasMultipleDifferentValues;
                            EditorGUI.BeginChangeCheck();
                            ShadowmaskMode shadowmask = serialized.serializedLightData.nonLightmappedOnly.boolValue ? ShadowmaskMode.ShadowMask : ShadowmaskMode.DistanceShadowmask;
                            shadowmask = (ShadowmaskMode)EditorGUILayout.EnumPopup(s_Styles.nonLightmappedOnly, shadowmask);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObjects(owner.targets, "Light Update Shadow Mask Mode");
                                serialized.serializedLightData.nonLightmappedOnly.boolValue = shadowmask == ShadowmaskMode.ShadowMask;
                                foreach (Light target in owner.targets)
                                    target.lightShadowCasterMode = shadowmask == ShadowmaskMode.ShadowMask ? LightShadowCasterMode.NonLightmappedOnly : LightShadowCasterMode.Everything;
                            }
                            EditorGUI.showMixedValue = false;
                        }
                    }
                    if (serialized.editorLightShape == LightShape.Rectangle)
                    {
                        EditorGUILayout.Slider(serialized.serializedLightData.areaLightShadowCone, 10.0f, 179.0f, s_Styles.areaLightShadowCone);
                    }
                    else
                    {
                        EditorGUILayout.Slider(serialized.serializedShadowData.viewBiasScale, 0.0f, 15.0f, s_Styles.viewBiasScale);
                    }
                }
#if ENABLE_RAYTRACING
                if(LightShape.Rectangle == serialized.editorLightShape)
                {
                    EditorGUILayout.PropertyField(serialized.serializedLightData.useRayTracedShadows, s_Styles.useRayTracedShadows);
                    if(serialized.serializedLightData.useRayTracedShadows.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialized.serializedLightData.numRayTracingSamples, s_Styles.numRayTracingSamples);
                        EditorGUILayout.PropertyField(serialized.serializedLightData.filterTracedShadow, s_Styles.filterTracedShadow);
                        EditorGUILayout.PropertyField(serialized.serializedLightData.filterSizeTraced, s_Styles.filterSizeTraced);
                        EditorGUI.indentLevel--;
                    }
                }

                if (serialized.settings.lightType.enumValueIndex == (int)LightType.Directional)
                {
                    EditorGUILayout.PropertyField(serialized.serializedLightData.useRayTracedShadows, s_Styles.useRayTracedShadows);
                    if(serialized.serializedLightData.useRayTracedShadows.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialized.serializedLightData.numRayTracingSamples, s_Styles.numRayTracingSamples);
                        EditorGUILayout.PropertyField(serialized.serializedLightData.filterTracedShadow, s_Styles.filterTracedShadow);
                        EditorGUILayout.PropertyField(serialized.serializedLightData.filterSizeTraced, s_Styles.filterSizeTraced);
                        EditorGUILayout.PropertyField(serialized.serializedLightData.sunLightConeAngle, s_Styles.sunLightConeAngle);
                        EditorGUI.indentLevel--;
                    }
                }
#endif
            }
        }


        static void DrawShadowMapAdvancedContent(SerializedHDLight serialized, Editor owner)
        {
            using (new EditorGUI.DisabledScope(serialized.settings.shadowsType.enumValueIndex == 0))
            {

                if(serialized.editorLightShape == LightShape.Rectangle)
                {
                    EditorGUILayout.Slider(serialized.serializedLightData.evsmExponent, 5.0f, 42.0f, s_Styles.evsmExponent);
                    EditorGUILayout.Slider(serialized.serializedLightData.evsmLightLeakBias, 0.0f, 1.0f, s_Styles.evsmLightLeakBias);
                    EditorGUILayout.Slider(serialized.serializedLightData.evsmVarianceBias, 0.0f, 0.001f, s_Styles.evsmVarianceBias);
                    EditorGUILayout.IntSlider(serialized.serializedLightData.evsmBlurPasses, 0, 8, s_Styles.evsmAdditionalBlurPasses);
                }
                else
                {
                    EditorGUILayout.Slider(serialized.serializedShadowData.viewBiasMin, 0.0f, 5.0f, s_Styles.viewBiasMin);
                    //EditorGUILayout.PropertyField(serialized.serializedShadowData.viewBiasMax, s_Styles.viewBiasMax);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Slider(serialized.serializedShadowData.normalBiasMin, 0.0f, 5.0f, s_Styles.normalBiasMin);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // Link min to max and don't expose normalBiasScale (useless when min == max)
                        serialized.serializedShadowData.normalBiasMax.floatValue = serialized.serializedShadowData.normalBiasMin.floatValue;
                    }
                    //EditorGUILayout.PropertyField(serialized.serializedShadowData.normalBiasMax, s_Styles.normalBiasMax);
                    //EditorGUILayout.PropertyField(serialized.serializedShadowData.normalBiasScale, s_Styles.normalBiasScale);
                    //EditorGUILayout.PropertyField(serialized.serializedShadowData.sampleBiasScale, s_Styles.sampleBiasScale);
                    EditorGUILayout.PropertyField(serialized.serializedShadowData.edgeLeakFixup, s_Styles.edgeLeakFixup);
                    if (serialized.serializedShadowData.edgeLeakFixup.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialized.serializedShadowData.edgeToleranceNormal, s_Styles.edgeToleranceNormal);
                        EditorGUILayout.Slider(serialized.serializedShadowData.edgeTolerance, 0.0f, 1.0f, s_Styles.edgeTolerance);
                        EditorGUI.indentLevel--;
                    }

                }

                // Dimmer and Tint don't have effect on baked shadow
                if (!serialized.settings.isCompletelyBaked)
                {
                    EditorGUILayout.Slider(serialized.serializedShadowData.shadowDimmer, 0.0f, 1.0f, s_Styles.shadowDimmer);
                    EditorGUILayout.PropertyField(serialized.serializedShadowData.shadowTint, s_Styles.shadowTint);
                }                

                if (serialized.settings.lightType.enumValueIndex != (int)LightType.Directional)
                {
                    EditorGUILayout.PropertyField(serialized.serializedShadowData.fadeDistance, s_Styles.shadowFadeDistance);
                }

                // Shadow Layers
                using (new EditorGUI.DisabledScope(!HDUtils.hdrpSettings.supportLightLayers))
                {
                    using (var change = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUILayout.PropertyField(serialized.serializedLightData.linkLightLayers, s_Styles.linkLightAndShadowLayersText);

                        // Undo the changes in the light component because the SyncLightAndShadowLayers will change the value automatically when link is ticked
                        if (change.changed)
                            Undo.RecordObjects(owner.targets, "Undo Light Layers Changed");
                    }
                    if (!serialized.serializedLightData.linkLightLayers.hasMultipleDifferentValues)
                    {
                        using (new EditorGUI.DisabledGroupScope(serialized.serializedLightData.linkLightLayers.boolValue))
                        {
                            HDEditorUtils.LightLayerMaskPropertyDrawer(s_Styles.shadowLayerMaskText, serialized.settings.renderingLayerMask);
                        }
                        if (serialized.serializedLightData.linkLightLayers.boolValue)
                            SyncLightAndShadowLayers(serialized, owner);
                    }
                }
            }
        }
        
        static void SyncLightAndShadowLayers(SerializedHDLight serialized, Editor owner)
        {
            // If we're not in decoupled mode for light layers, we sync light with shadow layers:
            foreach (Light target in owner.targets)
                target.renderingLayerMask = serialized.serializedLightData.lightlayersMask.intValue;
        }

        static void DrawContactShadowsContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUILayout.PropertyField(serialized.serializedShadowData.contactShadows, s_Styles.contactShadows);
        }

        static void DrawBakedShadowsContent(SerializedHDLight serialized, Editor owner)
        {
            switch ((LightType)serialized.settings.lightType.enumValueIndex)
            {
                case LightType.Directional:
                    EditorGUILayout.Slider(serialized.settings.bakedShadowAngleProp, 0f, 90f, s_Styles.bakedShadowAngle);
                    break;
                case LightType.Spot:
                case LightType.Point:
                    EditorGUILayout.PropertyField(serialized.settings.bakedShadowRadiusProp, s_Styles.bakedShadowRadius);
                    break;
            }
        }


        static bool HasShadowQualitySettingsUI(HDShadowQuality quality, SerializedHDLight serialized, Editor owner)
        {
            // Handle quality where there is nothing to draw directly here
            // No PCSS for now with directional light
            if (quality == HDShadowQuality.Medium || quality == HDShadowQuality.Low)
                return false;

            // Draw shadow settings using the current shadow algorithm
            HDShadowInitParameters hdShadowInitParameters = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).currentPlatformRenderPipelineSettings.hdShadowInitParams;
            LightType lightType = (LightType)serialized.settings.lightType.enumValueIndex;

            if (hdShadowInitParameters.shadowQuality == HDShadowQuality.VeryHigh)
            {
                // For very high settings shadow punctial lights we do not use the Very high settings but rather the High
                if (quality == HDShadowQuality.High && lightType != LightType.Directional)
                    return true;
                // Only the directional can access the very high shadow settings
                else if (quality == HDShadowQuality.VeryHigh && lightType == LightType.Directional)
                    return true;
                return false;
            }

            return hdShadowInitParameters.shadowQuality == quality;
        }

        static void ApplyEditorLightShape(SerializedHDLight serialized, Editor owner)
        {
            switch (serialized.editorLightShape)
            {
                case LightShape.Directional:
                    serialized.settings.lightType.enumValueIndex = (int)LightType.Directional;
                    serialized.serializedLightData.lightTypeExtent.enumValueIndex = (int)LightTypeExtent.Punctual;
                    break;
                case LightShape.Point:
                    serialized.settings.lightType.enumValueIndex = (int)LightType.Point;
                    serialized.serializedLightData.lightTypeExtent.enumValueIndex = (int)LightTypeExtent.Punctual;
                    break;
                case LightShape.Spot:
                    serialized.settings.lightType.enumValueIndex = (int)LightType.Spot;
                    serialized.serializedLightData.lightTypeExtent.enumValueIndex = (int)LightTypeExtent.Punctual;
                    break;
                case LightShape.Rectangle:
                    // TODO: Currently if we use Area type as it is offline light in legacy, the light will not exist at runtime
                    //m_BaseData.type.enumValueIndex = (int)LightType.Rectangle;
                    // In case of change, think to update InitDefaultHDAdditionalLightData()
                    serialized.settings.lightType.enumValueIndex = (int)LightType.Point;
                    serialized.serializedLightData.lightTypeExtent.enumValueIndex = (int)LightTypeExtent.Rectangle;
                    if (serialized.settings.isRealtime)
                        serialized.settings.shadowsType.enumValueIndex = (int)LightShadows.None;
                    break;
                case LightShape.Tube:
                    // TODO: Currently if we use Area type as it is offline light in legacy, the light will not exist at runtime
                    //m_BaseData.type.enumValueIndex = (int)LightType.Rectangle;
                    // In case of change, think to update InitDefaultHDAdditionalLightData()
                    serialized.settings.lightType.enumValueIndex = (int)LightType.Point;
                    serialized.serializedLightData.lightTypeExtent.enumValueIndex = (int)LightTypeExtent.Tube;
                    serialized.settings.shadowsType.enumValueIndex = (int)LightShadows.None;
                    break;
                case (LightShape)(-1):
                    // don't do anything, this is just to handle multi selection
                    break;
                default:
                    Debug.Assert(false, "Not implemented light type");
                    break;
            }
        }

        static void DrawLowShadowSettingsContent(SerializedHDLight serialized, Editor owner)
        {
            // Currently there is nothing to display here
            // when adding something, update IsShadowSettings
        }

        static void DrawMediumShadowSettingsContent(SerializedHDLight serialized, Editor owner)
        {
            // Currently there is nothing to display here
            // when adding something, update IsShadowSettings
        }

        static void DrawHighShadowSettingsContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUILayout.PropertyField(serialized.serializedLightData.shadowSoftness, s_Styles.shadowSoftness);
            EditorGUILayout.PropertyField(serialized.serializedLightData.blockerSampleCount, s_Styles.blockerSampleCount);
            EditorGUILayout.PropertyField(serialized.serializedLightData.filterSampleCount, s_Styles.filterSampleCount);
            EditorGUILayout.PropertyField(serialized.serializedLightData.minFilterSize, s_Styles.minFilterSize);
        }

        static void DrawVeryHighShadowSettingsContent(SerializedHDLight serialized, Editor owner)
        {
            EditorGUILayout.PropertyField(serialized.serializedLightData.kernelSize, s_Styles.kernelSize);
            EditorGUILayout.PropertyField(serialized.serializedLightData.lightAngle, s_Styles.lightAngle);
            EditorGUILayout.PropertyField(serialized.serializedLightData.maxDepthBias, s_Styles.maxDepthBias);
        }
    }
}
