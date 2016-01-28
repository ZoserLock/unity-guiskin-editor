using UnityEngine;
using UnityEditor;

using System.Collections;

public class GUISkinEditorWindow : EditorWindow
{
    [MenuItem("Window/GUISkin Editor Window")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(GUISkinEditorWindow));
        window.titleContent = new GUIContent("BT Tree Editor");
    }

    void OnEnable()
    {
        
    }

    void OnGUI()
    {
        GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUIStyles/TestGUISkin.guiskin");
        GUI.skin = skin;

        GUILayout.BeginVertical();
        GUILayout.Label("Label");
        GUILayout.Button("Button");
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Label");
        GUILayout.Button("Button");
        GUILayout.EndHorizontal();
    }

}
