using UnityEngine.Rendering;
using System;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    internal class SkyRenderingContext
    {
        IBLFilterBSDF[]             m_IBLFilterArray;
        RTHandleSystem.RTHandle     m_SkyboxCubemapRT;
        RTHandleSystem.RTHandle     m_SkyboxBSDFCubemapIntermediate;
        CubemapArray                m_SkyboxBSDFCubemapArray;
        RTHandleSystem.RTHandle     m_SkyboxMarginalRowCdfRT;
        RTHandleSystem.RTHandle     m_SkyboxConditionalCdfRT;
        Vector4                     m_CubemapScreenSize;
        Matrix4x4[]                 m_facePixelCoordToViewDirMatrices   = new Matrix4x4[6];
        bool                        m_SupportsConvolution = false;
        bool                        m_SupportsMIS = false;
        BuiltinSkyParameters        m_BuiltinParameters = new BuiltinSkyParameters();
        bool                        m_NeedUpdate = true;

        ComputeShader               m_ComputeAmbientProbeCS;
        ComputeBuffer               m_AmbientProbeResult;
        SphericalHarmonicsL2        m_AmbientProbe;
        readonly int                m_AmbientProbeOutputBufferParam = Shader.PropertyToID("_AmbientProbeOutputBuffer");
        readonly int                m_AmbientProbeInputCubemap = Shader.PropertyToID("_AmbientProbeInputCubemap");
        int                         m_ComputeAmbientProbeKernel;


        public RenderTexture cubemapRT => m_SkyboxCubemapRT;
        public Texture reflectionTexture => m_SkyboxBSDFCubemapArray;
        public SphericalHarmonicsL2 ambientProbe => m_AmbientProbe;

        public SkyRenderingContext(IBLFilterBSDF[] iblFilterBDSDFArray, int resolution, bool supportsConvolution)
        {
            m_IBLFilterArray = iblFilterBDSDFArray;
            m_SupportsConvolution = supportsConvolution;

            // Compute buffer storing the resulting SH from diffuse convolution. L2 SH => 9 float per component.
            m_AmbientProbeResult = new ComputeBuffer(27, 4);
            var hdrp = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;
            m_ComputeAmbientProbeCS = hdrp.renderPipelineResources.shaders.ambientProbeConvolutionCS;
            m_ComputeAmbientProbeKernel = m_ComputeAmbientProbeCS.FindKernel("AmbientProbeConvolution");

            RebuildTextures(resolution);
        }

        public void RebuildTextures(int resolution)
        {
            bool updateNeeded = m_SkyboxCubemapRT == null || (m_SkyboxCubemapRT.rt.width != resolution);

            // Cleanup first if needed
            if (updateNeeded)
            {
                RTHandles.Release(m_SkyboxCubemapRT);
                m_SkyboxCubemapRT = null;

                RTHandles.Release(m_SkyboxBSDFCubemapIntermediate);
                m_SkyboxBSDFCubemapIntermediate = null;

                CoreUtils.Destroy(m_SkyboxBSDFCubemapArray);
                m_SkyboxBSDFCubemapArray = null;

                RTHandles.Release(m_SkyboxConditionalCdfRT);
                m_SkyboxConditionalCdfRT = null;

                RTHandles.Release(m_SkyboxMarginalRowCdfRT);
                m_SkyboxMarginalRowCdfRT = null;
            }

            // Reallocate everything
            if (m_SkyboxCubemapRT == null)
            {
                m_SkyboxCubemapRT = RTHandles.Alloc(resolution, resolution, colorFormat: GraphicsFormat.R16G16B16A16_SFloat, dimension: TextureDimension.Cube, useMipMap: true, autoGenerateMips: false, filterMode: FilterMode.Trilinear, name: "SkyboxCubemap");

                if (m_SupportsConvolution)
                {
                    m_SkyboxBSDFCubemapIntermediate = RTHandles.Alloc(resolution, resolution, colorFormat: GraphicsFormat.R16G16B16A16_SFloat, dimension: TextureDimension.Cube, useMipMap: true, autoGenerateMips: false, filterMode: FilterMode.Trilinear, name: "SkyboxBSDFIntermediate");
                    m_SkyboxBSDFCubemapArray = new CubemapArray(resolution, m_IBLFilterArray.Length, TextureFormat.RGBAHalf, true)
                    {
                        hideFlags = HideFlags.HideAndDontSave,
                        wrapMode = TextureWrapMode.Repeat,
                        wrapModeV = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Trilinear,
                        anisoLevel = 0,
                        name = "SkyboxCubemapConvolution"
                    };
                }

                if (m_SupportsMIS)
                {
                    // Temporary, it should be dependent on the sky resolution
                    int width = (int)LightSamplingParameters.TextureWidth;
                    int height = (int)LightSamplingParameters.TextureHeight;

                    // + 1 because we store the value of the integral of the cubemap at the end of the texture.
                    m_SkyboxMarginalRowCdfRT = RTHandles.Alloc(height + 1, 1, colorFormat: GraphicsFormat.R32_SFloat, useMipMap: false, enableRandomWrite: true, filterMode: FilterMode.Point, name: "SkyboxMarginalRowCdf");

                    // TODO: switch the format to R16 (once it's available) to save some bandwidth.
                    m_SkyboxConditionalCdfRT = RTHandles.Alloc(width, height, colorFormat: GraphicsFormat.R32_SFloat, useMipMap: false, enableRandomWrite: true, filterMode: FilterMode.Point, name: "SkyboxConditionalRowCdf");
                }
            }

            m_CubemapScreenSize = new Vector4((float)resolution, (float)resolution, 1.0f / (float)resolution, 1.0f / (float)resolution);

            if (updateNeeded)
            {
                m_NeedUpdate = true; // Special case. Even if update mode is set to OnDemand, we need to regenerate the environment after destroying the texture.
                RebuildSkyMatrices(resolution);
            }
        }

        public void RebuildSkyMatrices(int resolution)
        {
            var cubeProj = Matrix4x4.Perspective(90.0f, 1.0f, 0.01f, 1.0f);

            for (int i = 0; i < 6; ++i)
            {
                var lookAt      = Matrix4x4.LookAt(Vector3.zero, CoreUtils.lookAtList[i], CoreUtils.upVectorList[i]);
                var worldToView = lookAt * Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f)); // Need to scale -1.0 on Z to match what is being done in the camera.wolrdToCameraMatrix API. ...

                m_facePixelCoordToViewDirMatrices[i] = HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(0.5f * Mathf.PI, Vector2.zero, m_CubemapScreenSize, worldToView, true);
            }
        }

        public void Cleanup()
        {
            RTHandles.Release(m_SkyboxCubemapRT);
            RTHandles.Release(m_SkyboxBSDFCubemapIntermediate);
            if (m_SkyboxBSDFCubemapArray != null)
            {
                CoreUtils.Destroy(m_SkyboxBSDFCubemapArray);
            }

            m_AmbientProbeResult.Release();

            RTHandles.Release(m_SkyboxMarginalRowCdfRT);
            RTHandles.Release(m_SkyboxConditionalCdfRT);
        }

        void RenderSkyToCubemap(SkyUpdateContext skyContext)
        {
            for (int i = 0; i < 6; ++i)
            {
                m_BuiltinParameters.pixelCoordToViewDirMatrix = m_facePixelCoordToViewDirMatrices[i];
                m_BuiltinParameters.colorBuffer = m_SkyboxCubemapRT;
                m_BuiltinParameters.depthBuffer = null;
                m_BuiltinParameters.hdCamera = null;

                CoreUtils.SetRenderTarget(m_BuiltinParameters.commandBuffer, m_SkyboxCubemapRT, ClearFlag.None, 0, (CubemapFace)i);
                skyContext.renderer.RenderSky(m_BuiltinParameters, true, skyContext.skySettings.includeSunInBaking.value);
            }

            // Generate mipmap for our cubemap
            Debug.Assert(m_SkyboxCubemapRT.rt.autoGenerateMips == false);
            m_BuiltinParameters.commandBuffer.GenerateMips(m_SkyboxCubemapRT);
        }

        void RenderCubemapGGXConvolution(SkyUpdateContext skyContext)
        {
            using (new ProfilingSample(m_BuiltinParameters.commandBuffer, "Update Env: GGX Convolution"))
            {
                for(int bsdfIdx = 0; bsdfIdx < m_IBLFilterArray.Length; ++bsdfIdx)
                {
                    // First of all filter this cubemap using the target filter
                    m_IBLFilterArray[bsdfIdx].FilterCubemap(m_BuiltinParameters.commandBuffer, m_SkyboxCubemapRT, m_SkyboxBSDFCubemapIntermediate);
                    // Then copy it to the cubemap array slice
                    for(int i = 0; i < 6; ++i)
                    {
                        m_BuiltinParameters.commandBuffer.CopyTexture(m_SkyboxBSDFCubemapIntermediate, i, m_SkyboxBSDFCubemapArray, 6 * bsdfIdx + i);
                    }
                }
            }
        }

        // We do our own hash here because Unity does not provide correct hash for builtin types
        // Moreover, we don't want to test every single parameters of the light so we filter them here in this specific function.
        int GetSunLightHashCode(Light light)
        {
            HDAdditionalLightData ald = light.GetComponent<HDAdditionalLightData>();
            unchecked
            {
                // Sun could influence the sky (like for procedural sky). We need to handle this possibility. If sun property change, then we need to update the sky
                int hash = 13;
                hash = hash * 23 + light.transform.position.GetHashCode();
                hash = hash * 23 + light.transform.rotation.GetHashCode();
                hash = hash * 23 + light.color.GetHashCode();
                hash = hash * 23 + light.colorTemperature.GetHashCode();
                hash = hash * 23 + light.intensity.GetHashCode();
                // Note: We don't take into account cookie as it doesn't influence GI
                if (ald != null)
                {
                    hash = hash * 23 + ald.lightDimmer.GetHashCode();
                }

                return hash;
            }
        }

        private void OnComputeAmbientProbeDone(AsyncGPUReadbackRequest request)
        {
            if (!request.hasError)
            {
                var result = request.GetData<float>();
                for (int channel = 0; channel < 3; ++channel)
                {
                    for (int coeff = 0; coeff < 9; ++coeff)
                    {
                        m_AmbientProbe[channel, coeff] = result[channel * 9 + coeff];
                    }
                }
            }
        }

        // GC.Alloc
        // VolumeParameter`.op_Equality()
        public bool UpdateEnvironment(SkyUpdateContext skyContext, HDCamera camera, Light sunLight, bool updateRequired, bool updateAmbientProbe, CommandBuffer cmd)
        {
            bool result = false;
            if (skyContext.IsValid())
            {
                skyContext.currentUpdateTime += Time.deltaTime;

                m_BuiltinParameters.commandBuffer = cmd;
                m_BuiltinParameters.sunLight = sunLight;
                m_BuiltinParameters.screenSize = m_CubemapScreenSize;
                m_BuiltinParameters.hdCamera = null;
                m_BuiltinParameters.debugSettings = null; // We don't want any debug when updating the environment.

                int sunHash = 0;
                if (sunLight != null)
                    sunHash = GetSunLightHashCode(sunLight);
                int skyHash = sunHash * 23 + skyContext.skySettings.GetHashCode();

                bool forceUpdate = (updateRequired || skyContext.updatedFramesRequired > 0 || m_NeedUpdate);
                if (forceUpdate ||
                    (skyContext.skySettings.updateMode == EnvironmentUpdateMode.OnChanged && skyHash != skyContext.skyParametersHash) ||
                    (skyContext.skySettings.updateMode == EnvironmentUpdateMode.Realtime && skyContext.currentUpdateTime > skyContext.skySettings.updatePeriod.value))
                {
                    using (new ProfilingSample(cmd, "Sky Environment Pass"))
                    {
                        using (new ProfilingSample(cmd, "Update Env: Generate Lighting Cubemap"))
                        {
                            RenderSkyToCubemap(skyContext);

                            if (updateAmbientProbe)
                            {
                                using (new ProfilingSample(cmd, "Update Ambient Probe"))
                                {
                                    cmd.SetComputeBufferParam(m_ComputeAmbientProbeCS, m_ComputeAmbientProbeKernel, m_AmbientProbeOutputBufferParam, m_AmbientProbeResult);
                                    cmd.SetComputeTextureParam(m_ComputeAmbientProbeCS, m_ComputeAmbientProbeKernel, m_AmbientProbeInputCubemap, m_SkyboxCubemapRT);
                                    cmd.DispatchCompute(m_ComputeAmbientProbeCS, m_ComputeAmbientProbeKernel, 1, 1, 1);
                                    cmd.RequestAsyncReadback(m_AmbientProbeResult, OnComputeAmbientProbeDone);
                                }
                            }
                        }

                        if (m_SupportsConvolution)
                        {
                            using (new ProfilingSample(cmd, "Update Env: Convolve Lighting Cubemap"))
                            {
                                RenderCubemapGGXConvolution(skyContext);
                            }
                        }

                        result = true;
                        skyContext.skyParametersHash = skyHash;
                        skyContext.currentUpdateTime = 0.0f;
                        skyContext.updatedFramesRequired--;
                        m_NeedUpdate = false;

#if UNITY_EDITOR
                        // In the editor when we change the sky we want to make the GI dirty so when baking again the new sky is taken into account.
                        // Changing the hash of the rendertarget allow to say that GI is dirty
                        m_SkyboxCubemapRT.rt.imageContentsHash = new Hash128((uint)skyContext.skySettings.GetHashCode(), 0, 0, 0);
#endif
                    }
                }
            }
            else
            {
                if (skyContext.skyParametersHash != 0)
                {
                    using (new ProfilingSample(cmd, "Clear Sky Environment Pass"))
                    {
                        CoreUtils.ClearCubemap(cmd, m_SkyboxCubemapRT, Color.black, true);
                        if (m_SupportsConvolution)
                        {
                            CoreUtils.ClearCubemap(cmd, m_SkyboxBSDFCubemapIntermediate, Color.black, true);
                            for (int bsdfIdx = 0; bsdfIdx < m_IBLFilterArray.Length; ++bsdfIdx)
                            {
                                for (int face = 0; face < 6; ++face)
                                    cmd.CopyTexture(m_SkyboxBSDFCubemapIntermediate, face, m_SkyboxBSDFCubemapArray, 6 * bsdfIdx + face);
                            }
                        }
                    }

                    skyContext.skyParametersHash = 0;
                    result = true;
                }
            }

            return result;
        }

        public void RenderSky(SkyUpdateContext skyContext, HDCamera hdCamera, Light sunLight, RTHandleSystem.RTHandle colorBuffer, RTHandleSystem.RTHandle depthBuffer, DebugDisplaySettings debugSettings, CommandBuffer cmd)
        {
            if (skyContext.IsValid() && hdCamera.clearColorMode == HDAdditionalCameraData.ClearColorMode.Sky)
            {
                using (new ProfilingSample(cmd, "Sky Pass"))
                {
                    m_BuiltinParameters.commandBuffer = cmd;
                    m_BuiltinParameters.sunLight = sunLight;
                    m_BuiltinParameters.pixelCoordToViewDirMatrix = hdCamera.mainViewConstants.pixelCoordToViewDirWS;
                    m_BuiltinParameters.screenSize = hdCamera.screenSize;
                    m_BuiltinParameters.colorBuffer = colorBuffer;
                    m_BuiltinParameters.depthBuffer = depthBuffer;
                    m_BuiltinParameters.hdCamera = hdCamera;
                    m_BuiltinParameters.debugSettings = debugSettings;

                    skyContext.renderer.SetRenderTargets(m_BuiltinParameters);

                    // If the luxmeter is enabled, we don't render the sky
                    if (debugSettings.data.lightingDebugSettings.debugLightingMode != DebugLightingMode.LuxMeter)
                    {
                        // When rendering the visual sky for reflection probes, we need to remove the sun disk if skySettings.includeSunInBaking is false.
                        skyContext.renderer.RenderSky(m_BuiltinParameters, false, hdCamera.camera.cameraType != CameraType.Reflection || skyContext.skySettings.includeSunInBaking.value);
                    }
                }
            }
        }
    }
}
