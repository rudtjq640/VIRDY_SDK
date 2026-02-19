#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VIRDY.SDK;

[CustomEditor(typeof(VIRDYWorldDescriptor))]
public class VIRDYWorldDescriptorEditor : SRDataEditor
{
    private VIRDYWorldDescriptor _worldDescriptor;

    private string _outputPath;

    [MenuItem("GameObject/VIRDY/VIRDYWorldDescriptor", false)]
    private static void Create(MenuCommand menuCommand)
    {
        if (VIRDYWorldDescriptor.Instance != null) return;

        var gameObject = new GameObject("VIRDYWorld");
        gameObject.AddComponent<VIRDYWorldDescriptor>();

        GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);

        Undo.RegisterCreatedObjectUndo(gameObject, "VIRDYWorld");

        Selection.activeObject = gameObject;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (_worldDescriptor == null) _worldDescriptor = (VIRDYWorldDescriptor)target;

#if !VIRDY_CORE
        if (_worldDescriptor.SpawnPoint == null) _worldDescriptor.SpawnPoint = _worldDescriptor.transform;

        if (_worldDescriptor.RendererData == null) _worldDescriptor.RendererData = (UniversalRendererData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("9f357787aaebbc44880985fd33cc3391"), typeof(UniversalRendererData));
        m_RendererData = _worldDescriptor.RendererData;

        _worldDescriptor.UnityVersion = Application.unityVersion;

        if (GraphicsSettings.defaultRenderPipeline?.ToString().Contains("Universal") ?? false)
        {
            _worldDescriptor.URPVersion = GetURPVersion();
        }

        GUILayout.Space(5);

        DrawHorizontalGUILine();

        GUILayout.Space(5);

        DrawRendererGroup();

        GUILayout.Space(5);

        DrawHorizontalGUILine();

        GUILayout.Space(5);

        DrawBuildGroup();
#endif
    }

    private void DrawRendererGroup()
    {
        if (m_RendererData == null) return;

        GUILayout.Label("Universal Renderer Data", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

        GUILayout.Space(5);

        GUIStyle disabled = new GUIStyle(GUI.skin.box);
        disabled.normal.textColor = Color.white;
        disabled.hover.textColor = Color.white;
        GUIStyle enabled = new GUIStyle(GUI.skin.box);
        enabled.normal.textColor = Color.green;
        enabled.hover.textColor = Color.green;

        GUILayout.Label("- 사용 가능한 렌더 피처 -");
        var ssao = m_RendererData.rendererFeatures.Any(f => f.name == "SSAO");
        GUILayout.Label("| Screen Space Ambient Occlusion", ssao ? enabled : disabled);
#if VIRDY_HORIZONBASEDAMBIENTOCCLUSION
        var hbao = m_RendererData.rendererFeatures.Any(f => f.name == "HBAO");
        GUILayout.Label("| HBAO Renderer Feature", hbao ? enabled : disabled);
#endif
#if VIRDY_OCCASOFTWARE_ALTOS
        var altos = m_RendererData.rendererFeatures.Any(f => f.name == "AltosRenderFeature");
        GUILayout.Label("| Altos Render Feature", altos ? enabled : disabled);
#endif
#if VIRDY_VOLUMETRICLIGHTS
        var volumetricLights = m_RendererData.rendererFeatures.Any(f => f.name == "Volumetric Lights");
        GUILayout.Label("| Volumetric Lights Render Feature", volumetricLights ? enabled : disabled);
#endif
#if VIRDY_VOLUMETRICFOGANDMIST2
        var volumetricFog = m_RendererData.rendererFeatures.Any(f => f.name == "Volumetric Fog 2");
        GUILayout.Label("| Volumetric Fog Render Feature", volumetricFog ? enabled : disabled);
#endif

        GUILayout.Space(5);

        base.OnInspectorGUI();
    }

    private void DrawBuildGroup()
    {
        GUILayout.Label("World Builder", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Scene", AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path), typeof(SceneAsset), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.TextField("Output Path", _outputPath);

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Open", GUILayout.MaxWidth(75f))) Application.OpenURL(_outputPath);
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f))) BrowseFolder();
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f))) ResetPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        DrawBuildButton();
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey("vOutputPath")) _outputPath = EditorPrefs.GetString("vOutputPath");
    }

    private void OnDisable()
    {
        EditorPrefs.SetString("vOutputPath", _outputPath);
    }

    private void BrowseFolder()
    {
        var path = EditorUtility.OpenFolderPanel("Output Folder", _outputPath, string.Empty);
        if (string.IsNullOrWhiteSpace(path) == false)
        {
            var gamePath = Path.GetFullPath(".");
            gamePath = gamePath.Replace("\\", "/");
            if (path.StartsWith(gamePath) && path.Length > gamePath.Length) path = path.Remove(0, gamePath.Length + 1);
            _outputPath = path;
        }
    }

    private void ResetPath()
    {
        _outputPath = "WorldBundles";
    }

    private void DrawBuildButton()
    {
        if (Application.isPlaying == true) return;

        if (GUILayout.Button("Build", new GUIStyle(GUI.skin.button) { fontSize = 18 }, new GUILayoutOption[] { GUILayout.MinHeight(25) }))
        {
            ClearAssetBundles();

            var scene = EditorSceneManager.GetActiveScene();
            AssetImporter.GetAtPath(scene.path).SetAssetBundleNameAndVariant(scene.name, "");

            EditorApplication.delayCall += ExecuteBuild;
        }
    }

    private void ExecuteBuild()
    {
        if (string.IsNullOrWhiteSpace(_outputPath) == true) BrowseFolder();
        if (string.IsNullOrWhiteSpace(_outputPath) == true)
        {
            Debug.LogError("�ùٸ� ��θ� �������ּ���.", _worldDescriptor);
            return;
        }

        if (!Directory.Exists(_outputPath)) Directory.CreateDirectory(_outputPath);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        options |= BuildAssetBundleOptions.ChunkBasedCompression;

        BuildPipeline.BuildAssetBundles(_outputPath, options, BuildTarget.StandaloneWindows);

        var directoryLength = Path.GetDirectoryName(_outputPath).Length;
        var mf1 = Path.Combine(_outputPath, _outputPath.Substring(directoryLength < 1 ? 0 : directoryLength + 1));
        var mf2 = Path.Combine(_outputPath, _outputPath.Substring(directoryLength < 1 ? 0 : directoryLength + 1) + ".manifest");
        var mf3 = Path.Combine(_outputPath, EditorSceneManager.GetActiveScene().name + ".manifest");
        if (File.Exists(mf1) == true) File.Delete(mf1);
        if (File.Exists(mf2) == true) File.Delete(mf2);
        if (File.Exists(mf3) == true) File.Delete(mf3);

        ClearAssetBundles();
    }

    private void ClearAssetBundles()
    {
        foreach (var bundleName in AssetDatabase.GetAllAssetBundleNames())
        {
            foreach (var asset in AssetDatabase.GetAssetPathsFromAssetBundle(bundleName))
            {
                AssetImporter.GetAtPath(asset).SetAssetBundleNameAndVariant("", "");
            }
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    private static void DrawHorizontalGUILine(float height = 2f)
    {
        GUILayout.Space(4);

        Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
        rect.height = height;
        rect.xMin = 0;
        rect.xMax = EditorGUIUtility.currentViewWidth;

        Color lineColor = new Color(0.10196f, 0.10196f, 0.10196f, 0.5f);
        EditorGUI.DrawRect(rect, lineColor);
        GUILayout.Space(4);
    }

    private static string GetURPVersion()
    {
        string path = AssetDatabase.GUIDToAssetPath("30648b8d550465f4bb77f1e1afd0b37d");
        var package = JsonUtility.FromJson<PackageInfo>(File.ReadAllText(path));
        return package.version;
    }

    private class PackageInfo
    {
        public string version = "";
    }
}
#endif
