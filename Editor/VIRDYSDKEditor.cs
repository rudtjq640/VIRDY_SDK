#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VIRDY.SDK;

[CustomEditor(typeof(VIRDYFadeUI))]
public class VIRDYFadeUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var fadeUI = (VIRDYFadeUI)target;

        EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Fade In")) fadeUI.FadeIn();
                if (GUILayout.Button("Fade Out")) fadeUI.FadeOut();
            }
            GUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();

        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(VIRDYSubtitleUI))]
public class VIRDYSubtitleUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Show")) VIRDYSubtitleUI.ShowSubtitle();
                if (GUILayout.Button("Hide")) VIRDYSubtitleUI.HideSubtitle();
            }
            GUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();

        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(VIRDYVideoPlayer))]
public class VIRDYVideoPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var videoPlayer = (VIRDYVideoPlayer)target;

        EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Play")) videoPlayer.Play();
                if (GUILayout.Button("Pause")) videoPlayer.Pause();
                if (GUILayout.Button("Stop")) videoPlayer.Stop();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var clipCount = serializedObject.FindProperty("_videoClips").arraySize;

            for (int n = 0; n < clipCount; n++)
            {
                if (GUILayout.Button($"Play Clip: {n}")) videoPlayer.PlayClip(n);
            }
        }
        EditorGUI.EndDisabledGroup();

        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(VIRDYLightTransition))]
public class VIRDYLightTransitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var lightTransition = (VIRDYLightTransition)target;

        EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
        {
            var presetCount = serializedObject.FindProperty("_presets").arraySize;

            for (int n = 0; n < presetCount; n++)
            {
                if (GUILayout.Button($"Set Preset: {n}")) lightTransition.SetPreset(n);
            }
        }
        EditorGUI.EndDisabledGroup();

        base.OnInspectorGUI();
    }
}
#endif
