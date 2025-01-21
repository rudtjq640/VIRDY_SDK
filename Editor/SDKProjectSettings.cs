#if UNITY_EDITOR && !VIRDY_CORE
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class SDKProjectSettings
{
    [InitializeOnLoadMethod]
    public static void SDKStartUpMethod()
    {
        SetVIRDYRenderPipeline();

        SetVIRDYLayer();
    }

    private static void SetVIRDYRenderPipeline()
    {
        var renderPipelineAsset = (RenderPipelineAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("566a695071e63f742906347b7aab5d82"), typeof(RenderPipelineAsset));        

        GraphicsSettings.defaultRenderPipeline = renderPipelineAsset;

        File.WriteAllText("ProjectSettings/QualitySettings.asset", QUALITYSETTINGS);
        QualitySettings.renderPipeline = renderPipelineAsset;
    }

    private static void SetVIRDYLayer()
    {
        var allLayers = GetAllLayers();

        foreach (var layer in Enum.GetValues(typeof(VIRDYLayers)))
        {
            if (allLayers.ContainsKey(layer.ToString()) == false)
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty layers = tagManager.FindProperty("layers");

                for (int i = 0; i < 32; i++)
                {
                    SerializedProperty element = layers.GetArrayElementAtIndex(i);
                    if (i == ((int)(VIRDYLayers)layer))
                    {
                        element.stringValue = layer.ToString();

                        tagManager.ApplyModifiedProperties();
                        break;
                    }
                }
            }
        }
    }

    private static Dictionary<string, int> GetAllLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        int layerSize = layers.arraySize;

        Dictionary<string, int> LayerDictionary = new Dictionary<string, int>();

        for (int i = 0; i < layerSize; i++)
        {
            SerializedProperty element = layers.GetArrayElementAtIndex(i);
            string layerName = element.stringValue;

            if (!string.IsNullOrEmpty(layerName))
            {
                LayerDictionary.Add(layerName, i);
            }
        }

        return LayerDictionary;
    }

    public enum VIRDYLayers
    {
        Avatar = 11,
        Settings = 30,
        DepthOfField = 31,
    }

    private const string QUALITYSETTINGS =
    @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!47 &1
QualitySettings:
  m_ObjectHideFlags: 0
  serializedVersion: 5
  m_CurrentQuality: 0
  m_QualitySettings:
  - serializedVersion: 3
    name: VIRDY Highest
    pixelLightCount: 2
    shadows: 2
    shadowResolution: 1
    shadowProjection: 1
    shadowCascades: 2
    shadowDistance: 40
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 1
    skinWeights: 255
    globalTextureMipmapLimit: 0
    textureMipmapLimitSettings: []
    anisotropicTextures: 2
    antiAliasing: 8
    softParticles: 0
    softVegetation: 1
    realtimeReflectionProbes: 1
    billboardsFaceCameraPosition: 1
    useLegacyDetailDistribution: 1
    vSyncCount: 0
    lodBias: 2
    maximumLODLevel: 0
    enableLODCrossFade: 1
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 2048
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 11400000, guid: 566a695071e63f742906347b7aab5d82,
      type: 2}
    terrainQualityOverrides: 0
    terrainPixelError: 1
    terrainDetailDensityScale: 1
    terrainBasemapDistance: 1000
    terrainDetailDistance: 80
    terrainTreeDistance: 5000
    terrainBillboardStart: 50
    terrainFadeLength: 5
    terrainMaxTrees: 50
    excludedTargetPlatforms: []
  m_TextureMipmapLimitGroupNames: []
  m_PerPlatformDefaultQuality:
    Android: 0
    CloudRendering: 0
    GameCoreScarlett: 0
    GameCoreXboxOne: 0
    Lumin: 0
    Nintendo Switch: 0
    PS4: 0
    PS5: 0
    Server: 0
    Stadia: 0
    Standalone: 0
    WebGL: 0
    Windows Store Apps: 0
    XboxOne: 0
    iPhone: 0
    tvOS: 0";
}
#endif
