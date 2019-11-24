# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [6.9.2] - 2019-10-04

### Fixed
- Fixed issue with Ambient Occlusion flickering
- Fixed with compute shader error about too many threads in threadgroup on low GPU
- Fixed invalid contact shadow shaders being created on metal
- Fixed a bug where if Assembly.GetTypes throws an exception due to mis-versioned dlls, then no preprocessors are used in the shader stripper
- Fixed issue with Matcap view and AxF shader
- Fixed compilation issue with stacklit and direct specular Occlusion
- Fixed typo in AXF decal property preventing to compile
- Fixed ShaderGraph material synchronization issues
- Fix/workaround a probable graphics driver bug in the GTAO shader.
- Fixed Hair and PBR shader graphs double sided modes
- Fixed issue that caused decals not to modify the roughness in the normal buffer, causing SSR to not behave correctly (case 1178336)
- Fixed issue of spotlight breaking when minimizing the cone angle via the gizmo (case 1178279)
- Fix a potential NaN source with iridescence (case 1183216)
- Fixed issue that prevented decals from modifying specular occlusion (case 1178272).
- Fixed ShaderGraph time in main preview
- Fixed an error caused by turning off Volumetrics, fog and other effects via the editor toggle when AO is active. 
- Fixed NPE when using light module in Shuriken particle systems (1173348).
- Fixed an issue where SSAO (that needs temporal reprojection) was still being rendered when Motion Vectors were not available (case 1184998)
- Fixed a nullref when modifying the height parameters inside the layered lit shader UI.
- Fix error first time a preview is created for planar
- Fixed an issue where SSR would use an incorrect roughness value on ForwardOnly (StackLit, AxF, Fabric, etc.) materials when the pipeline is configured to also allow deferred Lit.
- Fix for issue that caused gizmos to render in render textures (case 1174395)
- Fixed refresh of baked cubemap by incrementing updateCount at the end of the bake (case 1158677).
- Fixed decals not affecting lightmap/lightprobe
- Fixed issue with rectangular area light when seen from the back
- Fixed fov to 0 in planar probe breaking the projection matrix (case 1182014)
- Fixed issue causing wrong shading when normal map mode is Object space, no normal map is set, but a detail map is present (case 1143352)
- Fixed an issue with shader stripper and asset dependencies search that was taking a long time with big projects
- Fixed an issue with Realtime GI not working on upgraded projects. 

### Changed
- Added Alembic velocity support to various Shaders.
- Allow in ShaderGraph to enable pre/post pass when the alpha clip is disabled
- Call the End/Begin camera rendering callbacks for camera with customRender enabled
- Replace hidden specular lighting control on framesettings by EnableSkyLighting and ReplaceFresnel0Diffuse.
- Increase max limit of area light and reflection probe to 128
- Change default texture for detailmap to grey
- Improve build time of shader (reduce shader stripper overhead)

## [6.9.1] - 2019-07-29

### Fixed
- Fixed PBR master node always opaque (wrong blend modes for forward pass)
- Fixed alphaClip option in materials not updating the renderqueue.
- Fixed PBR master node preview
- Now the static lighting sky will correctly take the default values for non-overridden properties
- Fix shadergraph material pass setup not called
- Fix property sync in shadergraph with the current material in the inspector
- Fixed an issue causing Scene View selection wire gizmo to not appear when using HDRP Shader Graphs.

### Changed
- direct strenght properties in ambient occlusion now affect direct specular as well
- Added a warning in the material UI when the diffusion profile assigned is not in the HDRP asset

## [6.9.0] - 2019-07-02

### Added
- Shader Graphs that use time-dependent vertex modification now generate correct motion vectors.

### Fixed
- Fixed issue with Low resolution transparency on PS4
- Fixed The Parallax Occlusion Mappping node in shader graph and it's UV input slot
- Fixed lighting with XR single-pass instancing by disabling deferred tiles
- Fixed camera flickering when using TAA and selecting the camera in the editor

### Changed
- Remove all obsolete wind code from shader
- Enlighten now uses inverse squared falloff instead of the built-in falloff.
- Enlighten is now deprecated. Use the CPU or GPU Lightmapper instead.

## [6.8.0] - 2019-06-12

### Added
- `Fixed`, `Viewer`, and `Automatic` modes to compute the FOV used when rendering a `PlanarReflectionProbe`
- A checkbox to toggle the chrome gizmo of `ReflectionProbe`and `PlanarReflectionProbe`
- Added a Light layer in shadows that allow for objects to cast shadows without being affected by light (and vice versa).
- You can now access ShaderGraph blend states from the Material UI (for example, **Surface Type**, **Sorting Priority**, and **Blending Mode**). This change may break Materials that use a ShaderGraph, to fix them, select **Edit > Render Pipeline > Reset all ShaderGraph Scene Materials BlendStates**. This syncs the blendstates of you ShaderGraph master nodes with the Material properties.
- You can now control ZTest, ZWrite, and CullMode for transparent Materials.
- Materials that use Unlit Shaders or Unlit Master Node Shaders now cast shadows.
- Added an option to enable the ztest on **After Post Process** materials when TAA is disabled.
- Added a new SSAO (based on Ground Truth Ambient Occlusion algorithm) to replace the previous one. 
- Added support for shadow tint on light
- BeginCameraRendering and EndCameraRendering callbacks are now called with probes

### Fixed
- The correct preview is displayed when selecting multiple `PlanarReflectionProbe`s
- Fixed volumetric rendering with camera-relative code and XR stereo instancing
- Fixed issue with flashing cyan due to async compilation of shader when selecting a mesh
- Fixed texture type mismatch when the contact shadow are disabled (causing errors on IOS devices)
- Fixed Generate Shader Includes while in package
- Fixed issue when texture where deleted in ShadowCascadeGUI
- Fixed issue in FrameSettingsHistory when disabling a camera several time without enabling it in between.
- Fixed volumetric reprojection with camera-relative code and XR stereo instancing
- Added custom BaseShaderPreprocessor in HDEditorUtils.GetBaseShaderPreprocessorList()
- Fixed compile issue when USE_XR_SDK is not defined
- Fixed procedural sky sun disk intensity for high directional light intensities
- Fixed Decal mip level when using texture mip map streaming to avoid dropping to lowest permitted mip (now loading all mips)
- Fixed deferred shading for XR single-pass instancing after lightloop refactor
- Fixed cluster and material classification debug (material classification now works with compute as pixel shader lighting)
- Fixed IOS Nan by adding a maximun epsilon definition REAL_EPS that uses HALF_EPS when fp16 are used
- Removed unnecessary GC allocation in motion blur code
- Fixed locked UI with advanded influence volume inspector for probes
- Fixed invalid capture direction when rendering planar reflection probes
- Fixed Decal HTILE optimization with platform not supporting texture atomatic (Disable it)
- Fixed a crash in the build when the contact shadows are disabled
- Fixed camera rendering callbacks order (endCameraRendering was being called before the actual rendering)
- Fixed issue with wrong opaque blending settings for After Postprocess
- Fixed issue with single shadow debug view and volumetrics

### Changed
- Removed ScriptRuntimeVersion check in wizard.
- Optimization: Reduce the group size of the deferred lighting pass from 16x16 to 8x8
- Replaced HDCamera.computePassCount by viewCount
- Removed xrInstancing flag in RTHandles (replaced by TextureXR.slices and TextureXR.dimensions)
- Refactor the HDRenderPipeline and lightloop code to preprare for high level rendergraph
- Removed the **Back Then Front Rendering** option in the fabric Master Node settings. Enabling this option previously did nothing.
- Shader type Real translates to FP16 precision on Nintendo Switch.
- Shader framework refactor: Introduce CBSDF, EvaluateBSDF, IsNonZeroBSDF to replace BSDF functions
- Shader framework refactor:  GetBSDFAngles, LightEvaluation and SurfaceShading functions
- Replace ComputeMicroShadowing by GetAmbientOcclusionForMicroShadowing
- Rename WorldToTangent to TangentToWorld as it was incorrectly named
- Remove SunDisk and Sun Halo size from directional light
- Remove the name in the diffusion profile UI
- Changed how shadow map resolution scaling with distance is computed. Now it uses screen space area rather than light range.


## [6.7.0-preview] - 2019-05-21

### Added
- Added ViewConstants StructuredBuffer to simplify XR rendering
- Added API to render specific settings during a frame
- Added stadia to the supported platforms (2019.3)
- Enabled cascade blends settings in the HD Shadow component
- Added Hardware Dynamic Resolution support. 
- Added MatCap debug view to replace the no scene lighting debug view. 
- Added clear GBuffer option in FrameSettings (default to false)
- Added preview for decal shader graph (Only albedo, normal and emission)
- Added exposure weight control for decal
- Screen Space Directional Shadow under a define option. Activated for ray tracing 
- Added a new abstraction for RendererList that will help transition to Render Graph and future RendererList API
- Added multipass support for VR
- Added XR SDK integration (multipass only)
- Added Shader Graph samples for Hair, Fabric and Decal master nodes.
- Add fade distance, shadow fade distance and light layers to light explorer
- Add method to draw light layer drawer in a rect to HDEditorUtils

### Fixed
- Fixed deserialization crash at runtime
- Fixed for ShaderGraph Unlit masternode not writing velocity
- Fixed a crash when assiging a new HDRP asset with the 'Verify Saving Assets' option enabled
- Fixed exposure to properly support TEXTURE2D_X
- Fixed TerrainLit basemap texture generation
- Fixed a bug that caused nans when material classification was enabled and a tile contained one standard material + a material with transmission.
- Fixed gradient sky hash that was not using the exposure hash
- Fixed displayed default FrameSettings in HDRenderPipelineAsset wrongly updated on scripts reload.
- Fixed gradient sky hash that was not using the exposure hash.
- Fixed visualize cascade mode with exposure.
- Fixed (enabled) exposure on override lighting debug modes.
- Fixed issue with LightExplorer when volume have no profile
- Fixed issue with SSR for negative, infinite and NaN history values
- Fixed LightLayer in HDReflectionProbe and PlanarReflectionProbe inspector that was not displayed as a mask.
- Fixed NaN in transmission when the thickness and a color component of the scattering distance was to 0
- Fixed Light's ShadowMask multi-edition.
- Fixed motion blur and SMAA with VR single-pass instancing
- Fixed NaNs generated by phase functionsin volumetric lighting
- Fixed NaN issue with refraction effect and IOR of 1 at extreme grazing angle
- Fixed nan tracker not using the exposure 
- Fixed sorting priority on lit and unlit materials
- Fixed null pointer exception when there are no AOVRequests defined on a camera
- Fixed dirty state of prefab using disabled ReflectionProbes
- Fixed an issue where gizmos and editor grid were not correctly depth tested
- Fixed created default scene prefab non editable due to wrong file extension.
- Fixed an issue where sky convolution was recomputed for nothing when a preview was visible (causing extreme slowness when fabric convolution is enabled)
- Fixed issue with decal that wheren't working currently in player
- Fixed missing stereo rendering macros in some fragment shaders
- Fixed exposure for ReflectionProbe and PlanarReflectionProbe gizmos
- Fixed single-pass instancing on PSVR
- Fixed Vulkan shader issue with Texture2DArray in ScreenSpaceShadow.compute by re-arranging code (workaround)
- Fixed camera-relative issue with lights and XR single-pass instancing
- Fixed single-pass instancing on Vulkan
- Fixed htile synchronization issue with shader graph decal
- Fixed Gizmos are not drawn in Camera preview
- Fixed pre-exposure for emissive decal
- Fixed wrong values computed in PreIntegrateFGD and in the generation of volumetric lighting data by forcing the use of fp32.
- Fixed NaNs arising during the hair lighting pass
- Fixed synchronization issue in decal HTile that occasionally caused rendering artifacts around decal borders
- Fixed QualitySettings getting marked as modified by HDRP (and thus checked out in Perforce)
- Fixed a bug with uninitialized values in light explorer
- Fixed issue with LOD transition
- Fixed shader warnings related to raytracing and TEXTURE2D_X
- Fixed an issue with history buffers causing effects like TAA or auto exposure to flicker when more than one camera was visible in the editor

### Changed
- Refactor PixelCoordToViewDirWS to be VR compatible and to compute it only once per frame
- Modified the variants stripper to take in account multiple HDRP assets used in the build.
- Improve the ray biasing code to avoid self-intersections during the SSR traversal
- Update Pyramid Spot Light to better match emitted light volume.
- Moved _XRViewConstants out of UnityPerPassStereo constant buffer to fix issues with PSSL
- Removed GetPositionInput_Stereo() and single-pass (double-wide) rendering mode
- Changed label width of the frame settings to accommodate better existing options. 
- SSR's Default FrameSettings for camera is now enable.
- Re-enabled the sharpening filter on Temporal Anti-aliasing
- Exposed HDEditorUtils.LightLayerMaskDrawer for integration in other packages and user scripting.
- Rename atmospheric scattering in FrameSettings to Fog
- The size modifier in the override for the culling sphere in Shadow Cascades now defaults to 0.6, which is the same as the formerly hardcoded value.
- Moved LOD Bias and Maximum LOD Level from Frame Setting section `Other` to `Rendering`
- ShaderGraph Decal that affect only emissive, only draw in emissive pass (was drawing in dbuffer pass too)
- Apply decal projector fade factor correctly on all attribut and for shader graph decal
- Move RenderTransparentDepthPostpass after all transparent
- Update exposure prepass to interleave XR single-pass instancing views in a checkerboard pattern

## [6.6.0-preview] - 2019-04-01

### Added
- Added preliminary changes for XR deferred shading
- Added support of 111110 color buffer
- Added proper support for Recorder in HDRP
- Added depth offset input in shader graph master nodes
- Added a Parallax Occlusion Mapping node
- Added SMAA support
- Added Homothety and Symetry quick edition modifier on volume used in ReflectionProbe, PlanarReflectionProbe and DensityVolume
- Added multi-edition support for DecalProjectorComponent
- Improve hair shader
- Added the _ScreenToTargetScaleHistory uniform variable to be used when sampling HDRP RTHandle history buffers.
- Added settings in `FrameSettings` to change `QualitySettings.lodBias` and `QualitySettings.maximumLODLevel` during a rendering
- Added an exposure node to retrieve the current, inverse and previous frame exposure value.
- Added an HD scene color node which allow to sample the scene color with mips and a toggle to remove the exposure.
- Added safeguard on HD scene creation if default scene not set in the wizard
- Added Low res transparency rendering pass. 

### Fixed
- Fixed HDRI sky intensity lux mode
- Fixed dynamic resolution for XR
- Fixed instance identifier semantic string used by Shader Graph
- Fixed null culling result occuring when changing scene that was causing crashes
- Fixed multi-edition light handles and inspector shapes
- Fixed light's LightLayer field when multi-editing
- Fixed normal blend edition handles on DensityVolume
- Fixed an issue with layered lit shader and height based blend where inactive layers would still have influence over the result
- Fixed multi-selection handles color for DensityVolume
- Fixed multi-edition inspector's blend distances for HDReflectionProbe, PlanarReflectionProbe and DensityVolume
- Fixed metric distance that changed along size in DensityVolume
- Fixed DensityVolume shape handles that have not same behaviour in advance and normal edition mode
- Fixed normal map blending in TerrainLit by only blending the derivatives
- Fixed Xbox One rendering just a grey screen instead of the scene
- Fixed probe handles for multiselection
- Fixed baked cubemap import settings for convolution
- Fixed regression causing crash when attempting to open HDRenderPipelineWizard without an HDRenderPipelineAsset setted
- Fixed FullScreenDebug modes: SSAO, SSR, Contact shadow, Prerefraction Color Pyramid, Final Color Pyramid
- Fixed volumetric rendering with stereo instancing
- Fixed shader warning
- Fixed missing resources in existing asset when updating package
- Fixed PBR master node preview in forward rendering or transparent surface
- Fixed deferred shading with stereo instancing
- Fixed "look at" edition mode of Rotation tool for DecalProjectorComponent
- Fixed issue when switching mode in ReflectionProbe and PlanarReflectionProbe
- Fixed issue where migratable component version where not always serialized when part of prefab's instance
- Fixed an issue where shadow would not be rendered properly when light layer are not enabled
- Fixed exposure weight on unlit materials
- Fixed Light intensity not played in the player when recorded with animation/timeline
- Fixed some issues when multi editing HDRenderPipelineAsset
- Fixed emission node breaking the main shader graph preview in certain conditions.
- Fixed checkout of baked probe asset when baking probes.
- Fixed invalid gizmo position for rotated ReflectionProbe
- Fixed multi-edition of material's SurfaceType and RenderingPath
- Fixed whole pipeline reconstruction on selecting for the first time or modifying other than the currently used HDRenderPipelineAsset
- Fixed single shadow debug mode
- Fixed global scale factor debug mode when scale > 1
- Fixed debug menu material overrides not getting applied to the Terrain Lit shader
- Fixed typo in computeLightVariants
- Fixed deferred pass with XR instancing by disabling ComputeLightEvaluation
- Fixed bloom resolution independence
- Fixed lens dirt intensity not behaving properly
- Fixed the Stop NaN feature
- Fixed some resources to handle more than 2 instanced views for XR
- Fixed issue with black screen (NaN) produced on old GPU hardware or intel GPU hardware with gaussian pyramid
- Fixed issue with disabled punctual light would still render when only directional light is present

### Changed
- DensityVolume scripting API will no longuer allow to change between advance and normal edition mode
- Disabled depth of field, lens distortion and panini projection in the scene view
- TerrainLit shaders and includes are reorganized and made simpler.
- TerrainLit shader GUI now allows custom properties to be displayed in the Terrain fold-out section.
- Optimize distortion pass with stencil
- Disable SceneSelectionPass in shader graph preview
- Control punctual light and area light shadow atlas separately
- Move SMAA anti-aliasing option to after Temporal Anti Aliasing one, to avoid problem with previously serialized project settings
- Optimize rendering with static only lighting and when no cullable lights/decals/density volumes are present. 
- Updated handles for DecalProjectorComponent for enhanced spacial position readability and have edition mode for better SceneView management
- DecalProjectorComponent are now scale independent in order to have reliable metric unit (see new Size field for changing the size of the volume)
- Restructure code from HDCamera.Update() by adding UpdateAntialiasing() and UpdateViewConstants()
- Renamed velocity to motion vectors
- Objects rendered during the After Post Process pass while TAA is enabled will not benefit from existing depth buffer anymore. This is done to fix an issue where those object would wobble otherwise
- Removed usage of builtin unity matrix for shadow, shadow now use same constant than other view
- The default volume layer mask for cameras & probes is now `Default` instead of `Everything`

## [6.5.0-preview] - 2019-03-07

### Added
- Added depth-of-field support with stereo instancing
- Adding real time area light shadow support
- Added a new FrameSettings: Specular Lighting to toggle the specular during the rendering

### Fixed
- Fixed diffusion profile upgrade breaking package when upgrading to a new version
- Fixed decals cropped by gizmo not updating correctly if prefab
- Fixed an issue when enabling SSR on multiple view
- Fixed edition of the intensity's unit field while selecting multiple lights
- Fixed wrong calculation in soft voxelization for density volume
- Fixed gizmo not working correctly with pre-exposure
- Fixed issue with setting a not available RT when disabling motion vectors
- Fixed planar reflection when looking at mirror normal
- Fixed mutiselection issue with HDLight Inspector
- Fixed HDAdditionalCameraData data migration
- Fixed failing builds when light explorer window is open
- Fixed cascade shadows border sometime causing artefacts between cascades
- Restored shadows in the Cascade Shadow debug visualization
- `camera.RenderToCubemap` use proper face culling

### Changed
- When rendering reflection probe disable all specular lighting and for metals use fresnelF0 as diffuse color for bake lighting.

## [6.4.0-preview] - 2019-02-21

### Added
- VR: Added TextureXR system to selectively expand TEXTURE2D macros to texture array for single-pass stereo instancing + Convert textures call to these macros
- Added an unit selection dropdown next to shutter speed (camera)
- Added error helpbox when trying to use a sub volume component that require the current HDRenderPipelineAsset to support a feature that it is not supporting.
- Add mesh for tube light when display emissive mesh is enabled

### Fixed
- Fixed Light explorer. The volume explorer used `profile` instead of `sharedProfile` which instantiate a custom volume profile instead of editing the asset itself.
- Fixed UI issue where all is displayed using metric unit in shadow cascade and Percent is set in the unit field (happening when opening the inspector).
- Fixed inspector event error when double clicking on an asset (diffusion profile/material).
- Fixed nullref on layered material UI when the material is not an asset.
- Fixed nullref exception when undo/redo a light property.
- Fixed visual bug when area light handle size is 0.

### Changed
- Update UI for 32bit/16bit shadow precision settings in HDRP asset
- Object motion vectors have been disabled in all but the game view. Camera motion vectors are still enabled everywhere, allowing TAA and Motion Blur to work on static objects.
- Enable texture array by default for most rendering code on DX11 and unlock stereo instancing (DX11 only for now)

## [6.3.0-preview] - 2019-02-18

### Added
- Added emissive property for shader graph decals
- Added a diffusion profile override volume so the list of diffusion profile assets to use can be chanaged without affecting the HDRP asset
- Added a "Stop NaNs" option on cameras and in the Scene View preferences.
- Added metric display option in HDShadowSettings and improve clamping
- Added shader parameter mapping in DebugMenu
- Added scripting API to configure DebugData for DebugMenu

### Fixed
- Fixed decals in forward
- Fixed issue with stencil not correctly setup for various master node and shader for the depth pass, motion vector pass and GBuffer/Forward pass
- Fixed SRP batcher and metal
- Fixed culling and shadows for Pyramid, Box, Rectangle and Tube lights
- Fixed an issue where scissor render state leaking from the editor code caused partially black rendering

### Changed
- When a lit material has a clear coat mask that is not null, we now use the clear coat roughness to compute the screen space reflection.
- Diffusion profiles are now limited to one per asset and can be referenced in materials, shader graphs and vfx graphs. Materials will be upgraded automatically except if they are using a shader graph, in this case it will display an error message.

## [6.2.0-preview] - 2019-02-15

### Added
- Added help box listing feature supported in a given HDRenderPipelineAsset alongs with the drawbacks implied.
- Added cascade visualizer, supporting disabled handles when not overriding.

### Fixed
- Fixed post processing with stereo double-wide
- Fixed issue with Metal: Use sign bit to find the cache type instead of lowest bit.
- Fixed invalid state when creating a planar reflection for the first time
- Fix FrameSettings's LitShaderMode not restrained by supported LitShaderMode regression.

### Changed
- The default value roughness value for the clearcoat has been changed from 0.03 to 0.01
- Update default value of based color for master node
- Update Fabric Charlie Sheen lighting model - Remove Fresnel component that wasn't part of initial model + Remap smoothness to [0.0 - 0.6] range for more artist friendly parameter

### Changed
- Code refactor: all macros with ARGS have been swapped with macros with PARAM. This is because the ARGS macros were incorrectly named.

## [6.1.0-preview] - 2019-02-13

### Added
- Added support for post-processing anti-aliasing in the Scene View (FXAA and TAA). These can be set in Preferences.
- Added emissive property for decal material (non-shader graph)

### Fixed
- Fixed a few UI bugs with the color grading curves.
- Fixed "Post Processing" in the scene view not toggling post-processing effects
- Fixed bake only object with flag `ReflectionProbeStaticFlag` when baking a `ReflectionProbe`

### Changed
- Removed unsupported Clear Depth checkbox in Camera inspector
- Updated the toggle for advanced mode in inspectors.

## [6.0.0-preview] - 2019-02-23

### Added
- Added new API to perform a camera rendering
- Added support for hair master node (Double kajiya kay - Lambert)
- Added Reset behaviour in DebugMenu (ingame mapping is right joystick + B)
- Added Default HD scene at new scene creation while in HDRP
- Added Wizard helping to configure HDRP project
- Added new UI for decal material to allow remapping and scaling of some properties
- Added cascade shadow visualisation toggle in HD shadow settings
- Added icons for assets
- Added replace blending mode for distortion
- Added basic distance fade for density volumes
- Added decal master node for shader graph
- Added HD unlit master node (Cross Pipeline version is name Unlit)
- Added new Rendering Queue in materials
- Added post-processing V3 framework embed in HDRP, remove postprocess V2 framework
- Post-processing now uses the generic volume framework
-   New depth-of-field, bloom, panini projection effects, motion blur
-   Exposure is now done as a pre-exposition pass, the whole system has been revamped
-   Exposure now use EV100 everywhere in the UI (Sky, Emissive Light)
- Added emissive intensity (Luminance and EV100 control) control for Emissive
- Added pre-exposure weigth for Emissive
- Added an emissive color node and a slider to control the pre-exposure percentage of emission color
- Added physical camera support where applicable
- Added more color grading tools
- Added changelog level for Shader Variant stripping
- Added Debug mode for validation of material albedo and metalness/specularColor values
- Added a new dynamic mode for ambient probe and renamed BakingSky to StaticLightingSky
- Added command buffer parameter to all Bind() method of material
- Added Material validator in Render Pipeline Debug
- Added code to future support of DXR (not enabled)
- Added support of multiviewport
- Added HDRenderPipeline.RequestSkyEnvironmentUpdate function to force an update from script when sky is set to OnDemand
- Added a Lighting and BackLighting slots in Lit, StackLit, Fabric and Hair master nodes
- Added support for overriding terrain detail rendering shaders, via the render pipeline editor resources asset
- Added xrInstancing flag support to RTHandle
- Added support for cullmask for decal projectors
- Added software dynamic resolution support
- Added support for "After Post-Process" render pass for unlit shader
- Added support for textured rectangular area lights
- Added stereo instancing macros to MSAA shaders
- Added support for Quarter Res Raytraced Reflections (not enabled)
- Added fade factor for decal projectors.
- Added stereo instancing macros to most shaders used in VR
- Added multi edition support for HDRenderPipelineAsset

### Fixed
- Fixed logic to disable FPTL with stereo rendering
- Fixed stacklit transmission and sun highlight
- Fixed decals with stereo rendering
- Fixed sky with stereo rendering
- Fixed flip logic for postprocessing + VR
- Fixed copyStencilBuffer pass for Switch
- Fixed point light shadow map culling that wasn't taking into account far plane
- Fixed usage of SSR with transparent on all master node
- Fixed SSR and microshadowing on fabric material
- Fixed blit pass for stereo rendering
- Fixed lightlist bounds for stereo rendering
- Fixed windows and in-game DebugMenu sync.
- Fixed FrameSettings' LitShaderMode sync when opening DebugMenu.
- Fixed Metal specific issues with decals, hitting a sampler limit and compiling AxF shader
- Fixed an issue with flipped depth buffer during postprocessing
- Fixed normal map use for shadow bias with forward lit - now use geometric normal
- Fixed transparent depth prepass and postpass access so they can be use without alpha clipping for lit shader
- Fixed support of alpha clip shadow for lit master node
- Fixed unlit master node not compiling
- Fixed issue with debug display of reflection probe
- Fixed issue with phong tessellations not working with lit shader
- Fixed issue with vertex displacement being affected by heightmap setting even if not heightmap where assign
- Fixed issue with density mode on Lit terrain producing NaN
- Fixed issue when going back and forth from Lit to LitTesselation for displacement mode
- Fixed issue with ambient occlusion incorrectly applied to emissiveColor with light layers in deferred
- Fixed issue with fabric convolution not using the correct convolved texture when fabric convolution is enabled
- Fixed issue with Thick mode for Transmission that was disabling transmission with directional light
- Fixed shutdown edge cases with HDRP tests
- Fixed slowdow when enabling Fabric convolution in HDRP asset
- Fixed specularAA not compiling in StackLit Master node
- Fixed material debug view with stereo rendering
- Fixed material's RenderQueue edition in default view.
- Fixed banding issues within volumetric density buffer
- Fixed missing multicompile for MSAA for AxF
- Fixed camera-relative support for stereo rendering
- Fixed remove sync with render thread when updating decal texture atlas.
- Fixed max number of keyword reach [256] issue. Several shader feature are now local
- Fixed Scene Color and Depth nodes
- Fixed SSR in forward
- Fixed custom editor of Unlit, HD Unlit and PBR shader graph master node
- Fixed issue with NewFrame not correctly calculated in Editor when switching scene
- Fixed issue with TerrainLit not compiling with depth only pass and normal buffer
- Fixed geometric normal use for shadow bias with PBR master node in forward
- Fixed instancing macro usage for decals
- Fixed error message when having more than one directional light casting shadow
- Fixed error when trying to display preview of Camera or PlanarReflectionProbe
- Fixed LOAD_TEXTURE2D_ARRAY_MSAA macro
- Fixed min-max and amplitude clamping value in inspector of vertex displacement materials
- Fixed issue with alpha shadow clip (was incorrectly clipping object shadow)
- Fixed an issue where sky cubemap would not be cleared correctly when setting the current sky to None
- Fixed a typo in Static Lighting Sky component UI
- Fixed issue with incorrect reset of RenderQueue when switching shader in inspector GUI
- Fixed issue with variant stripper stripping incorrectly some variants
- Fixed a case of ambient lighting flickering because of previews
- Fixed Decals when rendering multiple camera in a single frame
- Fixed cascade shadow count in shader
- Fixed issue with Stacklit shader with Haze effect
- Fixed an issue with the max sample count for the TAA
- Fixed post-process guard band for XR
- Fixed exposure of emissive of Unlit
- Fixed depth only and motion vector pass for Unlit not working correctly with MSAA
- Fixed an issue with stencil buffer copy causing unnecessary compute dispatches for lighting
- Fixed multi edition issue in FrameSettings
- Fixed issue with SRP batcher and DebugDisplay variant of lit shader
- Fixed issue with debug material mode not doing alpha test
- Fixed "Attempting to draw with missing UAV bindings" errors on Vulkan
- Fixed pre-exposure incorrectly apply to preview
- Fixed issue with duplicate 3D texture in 3D texture altas of volumetric?
- Fixed Camera rendering order (base on the depth parameter)
- Fixed shader graph decals not being cropped by gizmo
- Fixed "Attempting to draw with missing UAV bindings" errors on Vulkan.


### Changed
- ColorPyramid compute shader passes is swapped to pixel shader passes on platforms where the later is faster (Nintendo Switch).
- Removing the simple lightloop used by the simple lit shader
- Whole refactor of reflection system: Planar and reflection probe
- Separated Passthrough from other RenderingPath
- Update several properties naming and caption based on feedback from documentation team
- Remove tile shader variant for transparent backface pass of lit shader
- Rename all HDRenderPipeline to HDRP folder for shaders
- Rename decal property label (based on doc team feedback)
- Lit shader mode now default to Deferred to reduce build time
- Update UI of Emission parameters in shaders
- Improve shader variant stripping including shader graph variant
- Refactored render loop to render realtime probes visible per camera
- Enable SRP batcher by default
- Shader code refactor: Rename LIGHTLOOP_SINGLE_PASS => LIGHTLOOP_DISABLE_TILE_AND_CLUSTER and clean all usage of LIGHTLOOP_TILE_PASS
- Shader code refactor: Move pragma definition of vertex and pixel shader inside pass + Move SURFACE_GRADIENT definition in XXXData.hlsl
- Micro-shadowing in Lit forward now use ambientOcclusion instead of SpecularOcclusion
- Upgraded FrameSettings workflow, DebugMenu and Inspector part relative to it
- Update build light list shader code to support 32 threads in wavefronts on Switch
- LayeredLit layers' foldout are now grouped in one main foldout per layer
- Shadow alpha clip can now be enabled on lit shader and haor shader enven for opaque
- Temporal Antialiasing optimization for Xbox One X
- Parameter depthSlice on SetRenderTarget functions now defaults to -1 to bind the entire resource
- Rename SampleCameraDepth() functions to LoadCameraDepth() and SampleCameraDepth(), same for SampleCameraColor() functions
- Improved Motion Blur quality. 
- Update stereo frame settings values for single-pass instancing and double-wide
- Rearrange FetchDepth functions to prepare for stereo-instancing
- Remove unused _ComputeEyeIndex
- Updated HDRenderPipelineAsset inspector
- Re-enable SRP batcher for metal

## [5.2.0-preview] - 2018-11-27

### Added
- Added option to run Contact Shadows and Volumetrics Voxelization stage in Async Compute
- Added camera freeze debug mode - Allow to visually see culling result for a camera
- Added support of Gizmo rendering before and after postprocess in Editor
- Added support of LuxAtDistance for punctual lights

### Fixed
- Fixed Debug.DrawLine and Debug.Ray call to work in game view
- Fixed DebugMenu's enum resetted on change
- Fixed divide by 0 in refraction causing NaN
- Fixed disable rough refraction support
- Fixed refraction, SSS and atmospheric scattering for VR
- Fixed forward clustered lighting for VR (double-wide).
- Fixed Light's UX to not allow negative intensity
- Fixed HDRenderPipelineAsset inspector broken when displaying its FrameSettings from project windows.
- Fixed forward clustered lighting for VR (double-wide).
- Fixed HDRenderPipelineAsset inspector broken when displaying its FrameSettings from project windows.
- Fixed Decals and SSR diable flags for all shader graph master node (Lit, Fabric, StackLit, PBR)
- Fixed Distortion blend mode for shader graph master node (Lit, StackLit)
- Fixed bent Normal for Fabric master node in shader graph
- Fixed PBR master node lightlayers
- Fixed shader stripping for built-in lit shaders.

### Changed
- Rename "Regular" in Diffusion profile UI "Thick Object"
- Changed VBuffer depth parametrization for volumetric from distanceRange to depthExtent - Require update of volumetric settings - Fog start at near plan
- SpotLight with box shape use Lux unit only

## [5.1.0-preview] - 2018-11-19

### Added

- Added a separate Editor resources file for resources Unity does not take when it builds a Player.
- You can now disable SSR on Materials in Shader Graph.
- Added support for MSAA when the Supported Lit Shader Mode is set to Both. Previously HDRP only supported MSAA for Forward mode.
- You can now override the emissive color of a Material when in debug mode.
- Exposed max light for Light Loop Settings in HDRP asset UI.
- HDRP no longer performs a NormalDBuffer pass update if there are no decals in the Scene.
- Added distant (fall-back) volumetric fog and improved the fog evaluation precision.
- Added an option to reflect sky in SSR.
- Added a y-axis offset for the PlanarReflectionProbe and offset tool.
- Exposed the option to run SSR and SSAO on async compute.
- Added support for the _GlossMapScale parameter in the Legacy to HDRP Material converter.
- Added wave intrinsic instructions for use in Shaders (for AMD GCN).


### Fixed
- Fixed sphere shaped influence handles clamping in Reflection Probes.
- Fixed Reflection Probe data migration for projects created before using HDRP.
- Fixed UI of Layered Material where Unity previously rendered the scrollbar above the Copy button.
- Fixed Material tessellations parameters Start fade distance and End fade distance. Originally, Unity clamped these values when you modified them.
- Fixed various distortion and refraction issues - handle a better fall-back.
- Fixed SSR for multiple views.
- Fixed SSR issues related to self-intersections.
- Fixed shape density volume handle speed.
- Fixed density volume shape handle moving too fast.
- Fixed the Camera velocity pass that we removed by mistake.
- Fixed some null pointer exceptions when disabling motion vectors support.
- Fixed viewports for both the Subsurface Scattering combine pass and the transparent depth prepass.
- Fixed the blend mode pop-up in the UI. It previously did not appear when you enabled pre-refraction.
- Fixed some null pointer exceptions that previously occurred when you disabled motion vectors support.
- Fixed Layered Lit UI issue with scrollbar.
- Fixed cubemap assignation on custom ReflectionProbe.
- Fixed Reflection Probes’ capture settings' shadow distance.
- Fixed an issue with the SRP batcher and Shader variables declaration.
- Fixed thickness and subsurface slots for fabric Shader master node that wasn't appearing with the right combination of flags.
- Fixed d3d debug layer warning.
- Fixed PCSS sampling quality.
- Fixed the Subsurface and transmission Material feature enabling for fabric Shader.
- Fixed the Shader Graph UV node’s dimensions when using it in a vertex Shader.
- Fixed the planar reflection mirror gizmo's rotation.
- Fixed HDRenderPipelineAsset's FrameSettings not showing the selected enum in the Inspector drop-down.
- Fixed an error with async compute.
- MSAA now supports transparency.
- The HDRP Material upgrader tool now converts metallic values correctly.
- Volumetrics now render in Reflection Probes.
- Fixed a crash that occurred whenever you set a viewport size to 0.
- Fixed the Camera physic parameter that the UI previously did not display.
- Fixed issue in pyramid shaped spotlight handles manipulation

### Changed

- Renamed Line shaped Lights to Tube Lights.
- HDRP now uses mean height fog parametrization.
- Shadow quality settings are set to All when you use HDRP (This setting is not visible in the UI when using SRP). This avoids Legacy Graphics Quality Settings disabling the shadows and give SRP full control over the Shadows instead.
- HDRP now internally uses premultiplied alpha for all fog.
- Updated default FrameSettings used for realtime Reflection Probes when you create a new HDRenderPipelineAsset.
- Remove multi-camera support. LWRP and HDRP will not support multi-camera layered rendering.
- Updated Shader Graph subshaders to use the new instancing define.
- Changed fog distance calculation from distance to plane to distance to sphere.
- Optimized forward rendering using AMD GCN by scalarizing the light loop.
- Changed the UI of the Light Editor.
- Change ordering of includes in HDRP Materials in order to reduce iteration time for faster compilation.
- Added a StackLit master node replacing the InspectorUI version. IMPORTANT: All previously authored StackLit Materials will be lost. You need to recreate them with the master node.

## [5.0.0-preview] - 2018-09-28

### Added
- Added occlusion mesh to depth prepass for VR (VR still disabled for now)
- Added a debug mode to display only one shadow at once
- Added controls for the highlight created by directional lights
- Added a light radius setting to punctual lights to soften light attenuation and simulate fill lighting
- Added a 'minRoughness' parameter to all non-area lights (was previously only available for certain light types)
- Added separate volumetric light/shadow dimmers
- Added per-pixel jitter to volumetrics to reduce aliasing artifacts
- Added a SurfaceShading.hlsl file, which implements material-agnostic shading functionality in an efficient manner
- Added support for shadow bias for thin object transmission
- Added FrameSettings to control realtime planar reflection
- Added control for SRPBatcher on HDRP Asset
- Added an option to clear the shadow atlases in the debug menu
- Added a color visualization of the shadow atlas rescale in debug mode
- Added support for disabling SSR on materials
- Added intrinsic for XBone
- Added new light volume debugging tool
- Added a new SSR debug view mode
- Added translaction's scale invariance on DensityVolume
- Added multiple supported LitShadermode and per renderer choice in case of both Forward and Deferred supported
- Added custom specular occlusion mode to Lit Shader Graph Master node

### Fixed
- Fixed a normal bias issue with Stacklit (Was causing light leaking)
- Fixed camera preview outputing an error when both scene and game view where display and play and exit was call
- Fixed override debug mode not apply correctly on static GI
- Fixed issue where XRGraphicsConfig values set in the asset inspector GUI weren't propagating correctly (VR still disabled for now)
- Fixed issue with tangent that was using SurfaceGradient instead of regular normal decoding
- Fixed wrong error message display when switching to unsupported target like IOS
- Fixed an issue with ambient occlusion texture sometimes not being created properly causing broken rendering
- Shadow near plane is no longer limited at 0.1
- Fixed decal draw order on transparent material
- Fixed an issue where sometime the lookup texture used for GGX convolution was broken, causing broken rendering
- Fixed an issue where you wouldn't see any fog for certain pipeline/scene configurations
- Fixed an issue with volumetric lighting where the anisotropy value of 0 would not result in perfectly isotropic lighting
- Fixed shadow bias when the atlas is rescaled
- Fixed shadow cascade sampling outside of the atlas when cascade count is inferior to 4
- Fixed shadow filter width in deferred rendering not matching shader config
- Fixed stereo sampling of depth texture in MSAA DepthValues.shader
- Fixed box light UI which allowed negative and zero sizes, thus causing NaNs
- Fixed stereo rendering in HDRISky.shader (VR)
- Fixed normal blend and blend sphere influence for reflection probe
- Fixed distortion filtering (was point filtering, now trilinear)
- Fixed contact shadow for large distance
- Fixed depth pyramid debug view mode
- Fixed sphere shaped influence handles clamping in reflection probes
- Fixed reflection probes data migration for project created before using hdrp
- Fixed ambient occlusion for Lit Master Node when slot is connected

### Changed
- Use samplerunity_ShadowMask instead of samplerunity_samplerLightmap for shadow mask
- Allow to resize reflection probe gizmo's size
- Improve quality of screen space shadow
- Remove support of projection model for ScreenSpaceLighting (SSR always use HiZ and refraction always Proxy)
- Remove all the debug mode from SSR that are obsolete now
- Expose frameSettings and Capture settings for reflection and planar probe
- Update UI for reflection probe, planar probe, camera and HDRP Asset
- Implement proper linear blending for volumetric lighting via deep compositing as described in the paper "Deep Compositing Using Lie Algebras"
- Changed  planar mapping to match terrain convention (XZ instead of ZX)
- XRGraphicsConfig is no longer Read/Write. Instead, it's read-only. This improves consistency of XR behavior between the legacy render pipeline and SRP
- Change reflection probe data migration code (to update old reflection probe to new one)
- Updated gizmo for ReflectionProbes
- Updated UI and Gizmo of DensityVolume

## [4.0.0-preview] - 2018-09-28

### Added
- Added a new TerrainLit shader that supports rendering of Unity terrains.
- Added controls for linear fade at the boundary of density volumes
- Added new API to control decals without monobehaviour object
- Improve Decal Gizmo
- Implement Screen Space Reflections (SSR) (alpha version, highly experimental)
- Add an option to invert the fade parameter on a Density Volume
- Added a Fabric shader (experimental) handling cotton and silk
- Added support for MSAA in forward only for opaque only
- Implement smoothness fade for SSR
- Added support for AxF shader (X-rite format - require special AxF importer from Unity not part of HDRP)
- Added control for sundisc on directional light (hack)
- Added a new HD Lit Master node that implements Lit shader support for Shader Graph
- Added Micro shadowing support (hack)
- Added an event on HDAdditionalCameraData for custom rendering
- HDRP Shader Graph shaders now support 4-channel UVs.

### Fixed
- Fixed an issue where sometimes the deferred shadow texture would not be valid, causing wrong rendering.
- Stencil test during decals normal buffer update is now properly applied
- Decals corectly update normal buffer in forward
- Fixed a normalization problem in reflection probe face fading causing artefacts in some cases
- Fix multi-selection behavior of Density Volumes overwriting the albedo value
- Fixed support of depth texture for RenderTexture. HDRP now correctly output depth to user depth buffer if RenderTexture request it.
- Fixed multi-selection behavior of Density Volumes overwriting the albedo value
- Fixed support of depth for RenderTexture. HDRP now correctly output depth to user depth buffer if RenderTexture request it.
- Fixed support of Gizmo in game view in the editor
- Fixed gizmo for spot light type
- Fixed issue with TileViewDebug mode being inversed in gameview
- Fixed an issue with SAMPLE_TEXTURECUBE_SHADOW macro
- Fixed issue with color picker not display correctly when game and scene view are visible at the same time
- Fixed an issue with reflection probe face fading
- Fixed camera motion vectors shader and associated matrices to update correctly for single-pass double-wide stereo rendering
- Fixed light attenuation functions when range attenuation is disabled
- Fixed shadow component algorithm fixup not dirtying the scene, so changes can be saved to disk.
- Fixed some GC leaks for HDRP
- Fixed contact shadow not affected by shadow dimmer
- Fixed GGX that works correctly for the roughness value of 0 (mean specular highlgiht will disappeard for perfect mirror, we rely on maxSmoothness instead to always have a highlight even on mirror surface)
- Add stereo support to ShaderPassForward.hlsl. Forward rendering now seems passable in limited test scenes with camera-relative rendering disabled.
- Add stereo support to ProceduralSky.shader and OpaqueAtmosphericScattering.shader.
- Added CullingGroupManager to fix more GC.Alloc's in HDRP
- Fixed rendering when multiple cameras render into the same render texture

### Changed
- Changed the way depth & color pyramids are built to be faster and better quality, thus improving the look of distortion and refraction.
- Stabilize the dithered LOD transition mask with respect to the camera rotation.
- Avoid multiple depth buffer copies when decals are present
- Refactor code related to the RT handle system (No more normal buffer manager)
- Remove deferred directional shadow and move evaluation before lightloop
- Add a function GetNormalForShadowBias() that material need to implement to return the normal used for normal shadow biasing
- Remove Jimenez Subsurface scattering code (This code was disabled by default, now remove to ease maintenance)
- Change Decal API, decal contribution is now done in Material. Require update of material using decal
- Move a lot of files from CoreRP to HDRP/CoreRP. All moved files weren't used by Ligthweight pipeline. Long term they could move back to CoreRP after CoreRP become out of preview
- Updated camera inspector UI
- Updated decal gizmo
- Optimization: The objects that are rendered in the Motion Vector Pass are not rendered in the prepass anymore
- Removed setting shader inclue path via old API, use package shader include paths
- The default value of 'maxSmoothness' for punctual lights has been changed to 0.99
- Modified deferred compute and vert/frag shaders for first steps towards stereo support
- Moved material specific Shader Graph files into corresponding material folders.
- Hide environment lighting settings when enabling HDRP (Settings are control from sceneSettings)
- Update all shader includes to use absolute path (allow users to create material in their Asset folder)
- Done a reorganization of the files (Move ShaderPass to RenderPipeline folder, Move all shadow related files to Lighting/Shadow and others)
- Improved performance and quality of Screen Space Shadows

## [3.3.0-preview]

### Added
- Added an error message to say to use Metal or Vulkan when trying to use OpenGL API
- Added a new Fabric shader model that supports Silk and Cotton/Wool
- Added a new HDRP Lighting Debug mode to visualize Light Volumes for Point, Spot, Line, Rectangular and Reflection Probes
- Add support for reflection probe light layers
- Improve quality of anisotropic on IBL

### Fixed
- Fix an issue where the screen where darken when rendering camera preview
- Fix display correct target platform when showing message to inform user that a platform is not supported
- Remove workaround for metal and vulkan in normal buffer encoding/decoding
- Fixed an issue with color picker not working in forward
- Fixed an issue where reseting HDLight do not reset all of its parameters
- Fixed shader compile warning in DebugLightVolumes.shader

### Changed
- Changed default reflection probe to be 256x256x6 and array size to be 64
- Removed dependence on the NdotL for thickness evaluation for translucency (based on artist's input)
- Increased the precision when comparing Planar or HD reflection probe volumes
- Remove various GC alloc in C#. Slightly better performance

## [3.2.0-preview]

### Added
- Added a luminance meter in the debug menu
- Added support of Light, reflection probe, emissive material, volume settings related to lighting to Lighting explorer
- Added support for 16bit shadows

### Fixed
- Fix issue with package upgrading (HDRP resources asset is now versionned to worarkound package manager limitation)
- Fix HDReflectionProbe offset displayed in gizmo different than what is affected.
- Fix decals getting into a state where they could not be removed or disabled.
- Fix lux meter mode - The lux meter isn't affected by the sky anymore
- Fix area light size reset when multi-selected
- Fix filter pass number in HDUtils.BlitQuad
- Fix Lux meter mode that was applying SSS
- Fix planar reflections that were not working with tile/cluster (olbique matrix)
- Fix debug menu at runtime not working after nested prefab PR come to trunk
- Fix scrolling issue in density volume

### Changed
- Shader code refactor: Split MaterialUtilities file in two parts BuiltinUtilities (independent of FragInputs) and MaterialUtilities (Dependent of FragInputs)
- Change screen space shadow rendertarget format from ARGB32 to RG16

## [3.1.0-preview]

### Added
- Decal now support per channel selection mask. There is now two mode. One with BaseColor, Normal and Smoothness and another one more expensive with BaseColor, Normal, Smoothness, Metal and AO. Control is on HDRP Asset. This may require to launch an update script for old scene: 'Edit/Render Pipeline/Single step upgrade script/Upgrade all DecalMaterial MaskBlendMode'.
- Decal now supports depth bias for decal mesh, to prevent z-fighting
- Decal material now supports draw order for decal projectors
- Added LightLayers support (Base on mask from renderers name RenderingLayers and mask from light name LightLayers - if they match, the light apply) - cost an extra GBuffer in deferred (more bandwidth)
- When LightLayers is enabled, the AmbientOclusion is store in the GBuffer in deferred path allowing to avoid double occlusion with SSAO. In forward the double occlusion is now always avoided.
- Added the possibility to add an override transform on the camera for volume interpolation
- Added desired lux intensity and auto multiplier for HDRI sky
- Added an option to disable light by type in the debug menu
- Added gradient sky
- Split EmissiveColor and bakeDiffuseLighting in forward avoiding the emissiveColor to be affect by SSAO
- Added a volume to control indirect light intensity
- Added EV 100 intensity unit for area lights
- Added support for RendererPriority on Renderer. This allow to control order of transparent rendering manually. HDRP have now two stage of sorting for transparent in addition to bact to front. Material have a priority then Renderer have a priority.
- Add Coupling of (HD)Camera and HDAdditionalCameraData for reset and remove in inspector contextual menu of Camera
- Add Coupling of (HD)ReflectionProbe and HDAdditionalReflectionData for reset and remove in inspector contextual menu of ReflectoinProbe
- Add macro to forbid unity_ObjectToWorld/unity_WorldToObject to be use as it doesn't handle camera relative rendering
- Add opacity control on contact shadow

### Fixed
- Fixed an issue with PreIntegratedFGD texture being sometimes destroyed and not regenerated causing rendering to break
- PostProcess input buffers are not copied anymore on PC if the viewport size matches the final render target size
- Fixed an issue when manipulating a lot of decals, it was displaying a lot of errors in the inspector
- Fixed capture material with reflection probe
- Refactored Constant Buffers to avoid hitting the maximum number of bound CBs in some cases.
- Fixed the light range affecting the transform scale when changed.
- Snap to grid now works for Decal projector resizing.
- Added a warning for 128x128 cookie texture without mipmaps
- Replace the sampler used for density volumes for correct wrap mode handling

### Changed
- Move Render Pipeline Debug "Windows from Windows->General-> Render Pipeline debug windows" to "Windows from Windows->Analysis-> Render Pipeline debug windows"
- Update detail map formula for smoothness and albedo, goal it to bright and dark perceptually and scale factor is use to control gradient speed
- Refactor the Upgrade material system. Now a material can be update from older version at any time. Call Edit/Render Pipeline/Upgrade all Materials to newer version
- Change name EnableDBuffer to EnableDecals at several place (shader, hdrp asset...), this require a call to Edit/Render Pipeline/Upgrade all Materials to newer version to have up to date material.
- Refactor shader code: BakeLightingData structure have been replace by BuiltinData. Lot of shader code have been remove/change.
- Refactor shader code: All GBuffer are now handled by the deferred material. Mean ShadowMask and LightLayers are control by lit material in lit.hlsl and not outside anymore. Lot of shader code have been remove/change.
- Refactor shader code: Rename GetBakedDiffuseLighting to ModifyBakedDiffuseLighting. This function now handle lighting model for transmission too. Lux meter debug mode is factor outisde.
- Refactor shader code: GetBakedDiffuseLighting is not call anymore in GBuffer or forward pass, including the ConvertSurfaceDataToBSDFData and GetPreLightData, this is done in ModifyBakedDiffuseLighting now
- Refactor shader code: Added a backBakeDiffuseLighting to BuiltinData to handle lighting for transmission
- Refactor shader code: Material must now call InitBuiltinData (Init all to zero + init bakeDiffuseLighting and backBakeDiffuseLighting ) and PostInitBuiltinData

## [3.0.0-preview]

### Fixed
- Fixed an issue with distortion that was using previous frame instead of current frame
- Fixed an issue where disabled light where not upgrade correctly to the new physical light unit system introduce in 2.0.5-preview

### Changed
- Update assembly definitions to output assemblies that match Unity naming convention (Unity.*).

## [2.0.5-preview]

### Added
- Add option supportDitheringCrossFade on HDRP Asset to allow to remove shader variant during player build if needed
- Add contact shadows for punctual lights (in additional shadow settings), only one light is allowed to cast contact shadows at the same time and so at each frame a dominant light is choosed among all light with contact shadows enabled.
- Add PCSS shadow filter support (from SRP Core)
- Exposed shadow budget parameters in HDRP asset
- Add an option to generate an emissive mesh for area lights (currently rectangle light only). The mesh fits the size, intensity and color of the light.
- Add an option to the HDRP asset to increase the resolution of volumetric lighting.
- Add additional ligth unit support for punctual light (Lumens, Candela) and area lights (Lumens, Luminance)
- Add dedicated Gizmo for the box Influence volume of HDReflectionProbe / PlanarReflectionProbe

### Changed
- Re-enable shadow mask mode in debug view
- SSS and Transmission code have been refactored to be able to share it between various material. Guidelines are in SubsurfaceScattering.hlsl
- Change code in area light with LTC for Lit shader. Magnitude is now take from FGD texture instead of a separate texture
- Improve camera relative rendering: We now apply camera translation on the model matrix, so before the TransformObjectToWorld(). Note: unity_WorldToObject and unity_ObjectToWorld must never be used directly.
- Rename positionWS to positionRWS (Camera relative world position) at a lot of places (mainly in interpolator and FragInputs). In case of custom shader user will be required to update their code.
- Rename positionWS, capturePositionWS, proxyPositionWS, influencePositionWS to positionRWS, capturePositionRWS, proxyPositionRWS, influencePositionRWS (Camera relative world position) in LightDefinition struct.
- Improve the quality of trilinear filtering of density volume textures.
- Improve UI for HDReflectionProbe / PlanarReflectionProbe

### Fixed
- Fixed a shader preprocessor issue when compiling DebugViewMaterialGBuffer.shader against Metal target
- Added a temporary workaround to Lit.hlsl to avoid broken lighting code with Metal/AMD
- Fixed issue when using more than one volume texture mask with density volumes.
- Fixed an error which prevented volumetric lighting from working if no density volumes with 3D textures were present.
- Fix contact shadows applied on transmission
- Fix issue with forward opaque lit shader variant being removed by the shader preprocessor
- Fixed compilation errors on Nintendo Switch (limited XRSetting support).
- Fixed apply range attenuation option on punctual light
- Fixed issue with color temperature not take correctly into account with static lighting
- Don't display fog when diffuse lighting, specular lighting, or lux meter debug mode are enabled.

## [2.0.4-preview]

### Fixed
- Fix issue when disabling rough refraction and building a player. Was causing a crash.

## [2.0.3-preview]

### Added
- Increased debug color picker limit up to 260k lux

## [2.0.2-preview]

### Added
- Add Light -> Planar Reflection Probe command
- Added a false color mode in rendering debug
- Add support for mesh decals
- Add flag to disable projector decals on transparent geometry to save performance and decal texture atlas space
- Add ability to use decal diffuse map as mask only
- Add visualize all shadow masks in lighting debug
- Add export of normal and roughness buffer for forwardOnly and when in supportOnlyForward mode for forward
- Provide a define in lit.hlsl (FORWARD_MATERIAL_READ_FROM_WRITTEN_NORMAL_BUFFER) when output buffer normal is used to read the normal and roughness instead of caclulating it (can save performance, but lower quality due to compression)
- Add color swatch to decal material

### Changed
- Change Render -> Planar Reflection creation to 3D Object -> Mirror
- Change "Enable Reflector" name on SpotLight to "Angle Affect Intensity"
- Change prototype of BSDFData ConvertSurfaceDataToBSDFData(SurfaceData surfaceData) to BSDFData ConvertSurfaceDataToBSDFData(uint2 positionSS, SurfaceData surfaceData)

### Fixed
- Fix issue with StackLit in deferred mode with deferredDirectionalShadow due to GBuffer not being cleared. Gbuffer is still not clear and issue was fix with the new Output of normal buffer.
- Fixed an issue where interpolation volumes were not updated correctly for reflection captures.
- Fixed an exception in Light Loop settings UI

## [2.0.1-preview]

### Added
- Add stripper of shader variant when building a player. Save shader compile time.
- Disable per-object culling that was executed in C++ in HD whereas it was not used (Optimization)
- Enable texture streaming debugging (was not working before 2018.2)
- Added Screen Space Reflection with Proxy Projection Model
- Support correctly scene selection for alpha tested object
- Add per light shadow mask mode control (i.e shadow mask distance and shadow mask). It use the option NonLightmappedOnly
- Add geometric filtering to Lit shader (allow to reduce specular aliasing)
- Add shortcut to create DensityVolume and PlanarReflection in hierarchy
- Add a DefaultHDMirrorMaterial material for PlanarReflection
- Added a script to be able to upgrade material to newer version of HDRP
- Removed useless duplication of ForwardError passes.
- Add option to not compile any DEBUG_DISPLAY shader in the player (Faster build) call Support Runtime Debug display

### Changed
- Changed SupportForwardOnly to SupportOnlyForward in render pipeline settings
- Changed versioning variable name in HDAdditionalXXXData from m_version to version
- Create unique name when creating a game object in the rendering menu (i.e Density Volume(2))
- Re-organize various files and folder location to clean the repository
- Change Debug windows name and location. Now located at:  Windows -> General -> Render Pipeline Debug

### Removed
- Removed GlobalLightLoopSettings.maxPlanarReflectionProbes and instead use value of GlobalLightLoopSettings.planarReflectionProbeCacheSize
- Remove EmissiveIntensity parameter and change EmissiveColor to be HDR (Matching Builtin Unity behavior) - Data need to be updated - Launch Edit -> Single Step Upgrade Script -> Upgrade all Materials emissionColor

### Fixed
- Fix issue with LOD transition and instancing
- Fix discrepency between object motion vector and camera motion vector
- Fix issue with spot and dir light gizmo axis not highlighted correctly
- Fix potential crash while register debug windows inputs at startup
- Fix warning when creating Planar reflection
- Fix specular lighting debug mode (was rendering black)
- Allow projector decal with null material to allow to configure decal when HDRP is not set
- Decal atlas texture offset/scale is updated after allocations (used to be before so it was using date from previous frame)

## [2018.1 experimental]

### Added
- Configure the VolumetricLightingSystem code path to be on by default
- Trigger a build exception when trying to build an unsupported platform
- Introduce the VolumetricLightingController component, which can (and should) be placed on the camera, and allows one to control the near and the far plane of the V-Buffer (volumetric "froxel" buffer) along with the depth distribution (from logarithmic to linear)
- Add 3D texture support for DensityVolumes
- Add a better mapping of roughness to mipmap for planar reflection
- The VolumetricLightingSystem now uses RTHandles, which allows to save memory by sharing buffers between different cameras (history buffers are not shared), and reduce reallocation frequency by reallocating buffers only if the rendering resolution increases (and suballocating within existing buffers if the rendering resolution decreases)
- Add a Volumetric Dimmer slider to lights to control the intensity of the scattered volumetric lighting
- Add UV tiling and offset support for decals.
- Add mipmapping support for volume 3D mask textures

### Changed
- Default number of planar reflection change from 4 to 2
- Rename _MainDepthTexture to _CameraDepthTexture
- The VolumetricLightingController has been moved to the Interpolation Volume framework and now functions similarly to the VolumetricFog settings
- Update of UI of cookie, CubeCookie, Reflection probe and planar reflection probe to combo box
- Allow enabling/disabling shadows for area lights when they are set to baked.
- Hide applyRangeAttenuation and FadeDistance for directional shadow as they are not used

### Removed
- Remove Resource folder of PreIntegratedFGD and add the resource to RenderPipeline Asset

### Fixed
- Fix ConvertPhysicalLightIntensityToLightIntensity() function used when creating light from script to match HDLightEditor behavior
- Fix numerical issues with the default value of mean free path of volumetric fog
- Fix the bug preventing decals from coexisting with density volumes
- Fix issue with alpha tested geometry using planar/triplanar mapping not render correctly or flickering (due to being wrongly alpha tested in depth prepass)
- Fix meta pass with triplanar (was not handling correctly the normal)
- Fix preview when a planar reflection is present
- Fix Camera preview, it is now a Preview cameraType (was a SceneView)
- Fix handling unknown GPUShadowTypes in the shadow manager.
- Fix area light shapes sent as point lights to the baking backends when they are set to baked.
- Fix unnecessary division by PI for baked area lights.
- Fix line lights sent to the lightmappers. The backends don't support this light type.
- Fix issue with shadow mask framesettings not correctly taken into account when shadow mask is enabled for lighting.
- Fix directional light and shadow mask transition, they are now matching making smooth transition
- Fix banding issues caused by high intensity volumetric lighting
- Fix the debug window being emptied on SRP asset reload
- Fix issue with debug mode not correctly clearing the GBuffer in editor after a resize
- Fix issue with ResetMaterialKeyword not resetting correctly ToggleOff/Roggle Keyword
- Fix issue with motion vector not render correctly if there is no depth prepass in deferred

## [2018.1.0f2]

### Added
- Screen Space Refraction projection model (Proxy raycasting, HiZ raymarching)
- Screen Space Refraction settings as volume component
- Added buffered frame history per camera
- Port Global Density Volumes to the Interpolation Volume System.
- Optimize ImportanceSampleLambert() to not require the tangent frame.
- Generalize SampleVBuffer() to handle different sampling and reconstruction methods.
- Improve the quality of volumetric lighting reprojection.
- Optimize Morton Order code in the Subsurface Scattering pass.
- Planar Reflection Probe support roughness (gaussian convolution of captured probe)
- Use an atlas instead of a texture array for cluster transparent decals
- Add a debug view to visualize the decal atlas
- Only store decal textures to atlas if decal is visible, debounce out of memory decal atlas warning.
- Add manipulator gizmo on decal to improve authoring workflow
- Add a minimal StackLit material (work in progress, this version can be used as template to add new material)

### Changed
- EnableShadowMask in FrameSettings (But shadowMaskSupport still disable by default)
- Forced Planar Probe update modes to (Realtime, Every Update, Mirror Camera)
- Screen Space Refraction proxy model uses the proxy of the first environment light (Reflection probe/Planar probe) or the sky
- Moved RTHandle static methods to RTHandles
- Renamed RTHandle to RTHandleSystem.RTHandle
- Move code for PreIntegratedFDG (Lit.shader) into its dedicated folder to be share with other material
- Move code for LTCArea (Lit.shader) into its dedicated folder to be share with other material

### Removed
- Removed Planar Probe mirror plane position and normal fields in inspector, always display mirror plane and normal gizmos

### Fixed
- Fix fog flags in scene view is now taken into account
- Fix sky in preview windows that were disappearing after a load of a new level
- Fix numerical issues in IntersectRayAABB().
- Fix alpha blending of volumetric lighting with transparent objects.
- Fix the near plane of the V-Buffer causing out-of-bounds look-ups in the clustered data structure.
- Depth and color pyramid are properly computed and sampled when the camera renders inside a viewport of a RTHandle.
- Fix decal atlas debug view to work correctly when shadow atlas view is also enabled

## [2018.1.0b13]

...
