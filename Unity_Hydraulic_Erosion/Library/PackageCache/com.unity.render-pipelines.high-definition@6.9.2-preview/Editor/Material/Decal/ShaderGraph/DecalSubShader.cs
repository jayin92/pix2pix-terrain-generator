using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    class DecalSubShader : IDecalSubShader
    {
        // CAUTION: c# code relies on the order in which the passes are declared, any change will need to be reflected in Decalsystem.cs - s_MaterialDecalNames and s_MaterialDecalSGNames array
        // and DecalSet.InitializeMaterialValues()

        Pass m_PassProjector3RT = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferProjector3RT],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferProjector3RT],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_DBUFFER_PROJECTOR",

            CullOverride = "Cull Front",
            ZTestOverride = "ZTest Greater",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha",

            ExtraDefines = new List<string>()
            {
                "#define DECALS_3RT",
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass) =>
            {

                DecalMasterNode masterNode = node as DecalMasterNode;
                int colorMaskIndex = 4; // smoothness only
                pass.ColorMaskOverride = m_ColorMasks[colorMaskIndex];
                pass.StencilOverride = new List<string>()
                {
                    "// Stencil setup",
                    "Stencil",
                    "{",
                        string.Format("   WriteMask {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        string.Format("   Ref  {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        "Comp Always",
                        "Pass Replace",
                    "}"
                };
            }
        };

        Pass m_PassProjector4RT = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferProjector4RT],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferProjector4RT],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_DBUFFER_PROJECTOR",

            CullOverride = "Cull Front",
            ZTestOverride = "ZTest Greater",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor",

            ExtraDefines = new List<string>()
            {
                "#define DECALS_4RT",
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass) =>
            {

                DecalMasterNode masterNode = node as DecalMasterNode;
                int colorMaskIndex = (masterNode.affectsMetal.isOn ? 1 : 0);
                colorMaskIndex |= (masterNode.affectsAO.isOn ? 2 : 0);
                colorMaskIndex |= (masterNode.affectsSmoothness.isOn ? 4 : 0);
                pass.ColorMaskOverride = m_ColorMasks[colorMaskIndex];
                pass.StencilOverride = new List<string>()
                {
                    "// Stencil setup",
                    "Stencil",
                    "{",
                        string.Format("   WriteMask {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        string.Format("   Ref  {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        "Comp Always",
                        "Pass Replace",
                    "}"
                };
            }
        };

        Pass m_PassProjectorEmissive = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_ProjectorEmissive],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_ProjectorEmissive],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_FORWARD_EMISSIVE_PROJECTOR",

            CullOverride = "Cull Front",
            ZTestOverride = "ZTest Greater",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha One",

            ExtraDefines = new List<string>()
            {
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.EmissionSlotId
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
        };


        Pass m_PassMesh3RT = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferMesh3RT],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferMesh3RT],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_DBUFFER_MESH",

            ZTestOverride = "ZTest LEqual",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha",

            ExtraDefines = new List<string>()
            {
                "#define DECALS_3RT",
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
                "AttributesMesh.normalOS",
                "AttributesMesh.tangentOS",
                "AttributesMesh.uv0",

                "FragInputs.tangentToWorld",
                "FragInputs.positionRWS",
                "FragInputs.texCoord0",
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass) =>
            {

                DecalMasterNode masterNode = node as DecalMasterNode;
                int colorMaskIndex = 4; // smoothness only
                pass.ColorMaskOverride = m_ColorMasks[colorMaskIndex];
                pass.StencilOverride = new List<string>()
                {
                    "// Stencil setup",
                    "Stencil",
                    "{",
                        string.Format("   WriteMask {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        string.Format("   Ref  {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        "Comp Always",
                        "Pass Replace",
                    "}"
                };
            }
        };


        Pass m_PassMesh4RT = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferMesh4RT],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_DBufferMesh4RT],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_DBUFFER_MESH",

            ZTestOverride = "ZTest LEqual",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor",

            ExtraDefines = new List<string>()
            {
                "#define DECALS_4RT",
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
                "AttributesMesh.normalOS",
                "AttributesMesh.tangentOS",     
                "AttributesMesh.uv0",

                "FragInputs.tangentToWorld",
                "FragInputs.positionRWS",
                "FragInputs.texCoord0",
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass) =>
            {

                DecalMasterNode masterNode = node as DecalMasterNode;
                int colorMaskIndex = (masterNode.affectsMetal.isOn ? 1 : 0);
                colorMaskIndex |= (masterNode.affectsAO.isOn ? 2 : 0);
                colorMaskIndex |= (masterNode.affectsSmoothness.isOn ? 4 : 0);
                pass.ColorMaskOverride = m_ColorMasks[colorMaskIndex];
                pass.StencilOverride = new List<string>()
                {
                    "// Stencil setup",
                    "Stencil",
                    "{",
                        string.Format("   WriteMask {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        string.Format("   Ref  {0}", (int) HDRenderPipeline.StencilBitMask.Decals),
                        "Comp Always",
                        "Pass Replace",
                    "}"
                };
            }
        };


        Pass m_PassMeshEmissive = new Pass()
        {
            Name = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_MeshEmissive],
            LightMode = DecalSystem.s_MaterialSGDecalPassNames[(int)DecalSystem.MaterialSGDecalPass.ShaderGraph_MeshEmissive],
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_FORWARD_EMISSIVE_MESH",

            ZTestOverride = "ZTest LEqual",
            ZWriteOverride = "ZWrite Off",
            BlendOverride = "Blend 0 SrcAlpha One",

            ExtraDefines = new List<string>()
            {
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
                "AttributesMesh.normalOS",
                "AttributesMesh.tangentOS",
                "AttributesMesh.uv0",

                "FragInputs.tangentToWorld",
                "FragInputs.positionRWS",
                "FragInputs.texCoord0",
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
                DecalMasterNode.EmissionSlotId
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = false,
        };

        // Pass to have a preview
        Pass m_PassPreview = new Pass()
        {
            Name = "ForwardOnly",
            LightMode = "ForwardOnly",
            TemplateName = "DecalPass.template",
            MaterialName = "Decal",
            ShaderPassName = "SHADERPASS_FORWARD_PREVIEW",

            ZTestOverride = "ZTest LEqual",

            ExtraDefines = new List<string>()
            {
            },

            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDecal.hlsl\""
            },

            RequiredFields = new List<string>()
            {
                "AttributesMesh.normalOS",
                "AttributesMesh.tangentOS",
                "AttributesMesh.uv0",

                "FragInputs.tangentToWorld",
                "FragInputs.positionRWS",
                "FragInputs.texCoord0",
            },

            PixelShaderSlots = new List<int>()
            {
                DecalMasterNode.AlbedoSlotId,
                DecalMasterNode.BaseColorOpacitySlotId,
                DecalMasterNode.NormalSlotId,
                DecalMasterNode.NormaOpacitySlotId,
                DecalMasterNode.MetallicSlotId,
                DecalMasterNode.AmbientOcclusionSlotId,
                DecalMasterNode.SmoothnessSlotId,
                DecalMasterNode.MAOSOpacitySlotId,
                DecalMasterNode.EmissionSlotId
            },

            VertexShaderSlots = new List<int>()
            {
            },

            UseInPreview = true,
        };

        public int GetPreviewPassIndex() { return 0; }

        private static string[] m_ColorMasks = new string[8]
        {
            "ColorMask 0 2 ColorMask 0 3",     // nothing
            "ColorMask R 2 ColorMask R 3",     // metal
            "ColorMask G 2 ColorMask G 3",     // AO
            "ColorMask RG 2 ColorMask RG 3",    // metal + AO
            "ColorMask BA 2 ColorMask 0 3",     // smoothness
            "ColorMask RBA 2 ColorMask R 3",     // metal + smoothness
            "ColorMask GBA 2 ColorMask G 3",     // AO + smoothness
            "ColorMask RGBA 2 ColorMask RG 3",   // metal + AO + smoothness
        };


        private static HashSet<string> GetActiveFieldsFromMasterNode(AbstractMaterialNode iMasterNode, Pass pass)
        {
            HashSet<string> activeFields = new HashSet<string>();

            DecalMasterNode masterNode = iMasterNode as DecalMasterNode;
            if (masterNode == null)
            {
                return activeFields;
            }
            if(masterNode.affectsAlbedo.isOn)
            {
                activeFields.Add("Material.AffectsAlbedo");
            }
            if (masterNode.affectsNormal.isOn)
            {
                activeFields.Add("Material.AffectsNormal");
            }
            if (masterNode.affectsEmission.isOn)
            {
                activeFields.Add("Material.AffectsEmission");
            }
            if (masterNode.affectsSmoothness.isOn || masterNode.affectsMetal.isOn || masterNode.affectsAO.isOn)
            {
                activeFields.Add("Material.AffectsMaskMap");
            }

            return activeFields;
        }

        private static bool GenerateShaderPass(DecalMasterNode masterNode, Pass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            if (mode == GenerationMode.ForReals || pass.UseInPreview)
            {
                pass.OnGeneratePass(masterNode);

                // apply master node options to active fields
                HashSet<string> activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

                // use standard shader pass generation
                bool vertexActive = masterNode.IsSlotConnected(DecalMasterNode.PositionSlotId);
                return HDSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths, vertexActive);
            }
            else
            {
                return false;
            }
        }

        public string GetSubshader(IMasterNode iMasterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
                // DecalSubShader.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("3b523fb79ded88842bb5195be78e0354"));
                // HDSubShaderUtilities.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("713ced4e6eef4a44799a4dd59041484b"));
            }

            var masterNode = iMasterNode as DecalMasterNode;

            var subShader = new ShaderGenerator();
            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                // Add tags at the SubShader level
                int queue = HDRenderQueue.ChangeType(HDRenderQueue.RenderQueueType.Opaque, masterNode.drawOrder, false);
                HDSubShaderUtilities.AddTags(subShader, HDRenderPipeline.k_ShaderTagName, HDRenderTypeTags.Opaque, queue);

                // Caution: Order of GenerateShaderPass matter. Only generate required pass
                if (masterNode.affectsAlbedo.isOn || masterNode.affectsNormal.isOn || masterNode.affectsMetal.isOn || masterNode.affectsAO.isOn || masterNode.affectsSmoothness.isOn)
                {
                    GenerateShaderPass(masterNode, m_PassProjector3RT, mode, subShader, sourceAssetDependencyPaths);
                    GenerateShaderPass(masterNode, m_PassProjector4RT, mode, subShader, sourceAssetDependencyPaths);
                }
                if (masterNode.affectsEmission.isOn)
                {
                    GenerateShaderPass(masterNode, m_PassProjectorEmissive, mode, subShader, sourceAssetDependencyPaths);
                }
                if (masterNode.affectsAlbedo.isOn || masterNode.affectsNormal.isOn || masterNode.affectsMetal.isOn || masterNode.affectsAO.isOn || masterNode.affectsSmoothness.isOn)
                {
                    GenerateShaderPass(masterNode, m_PassMesh3RT, mode, subShader, sourceAssetDependencyPaths);
                    GenerateShaderPass(masterNode, m_PassMesh4RT, mode, subShader, sourceAssetDependencyPaths);
                }
                if (masterNode.affectsEmission.isOn)
                {
                    GenerateShaderPass(masterNode, m_PassMeshEmissive, mode, subShader, sourceAssetDependencyPaths);
                }

                if (mode.IsPreview())
                {
                    GenerateShaderPass(masterNode, m_PassPreview, mode, subShader, sourceAssetDependencyPaths);
                }
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);
            subShader.AddShaderChunk(@"CustomEditor ""UnityEditor.Experimental.Rendering.HDPipeline.DecalGUI""");
            string s = subShader.GetShaderString(0);
            return s;
        }


        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is HDRenderPipelineAsset;
        }
    }
}
