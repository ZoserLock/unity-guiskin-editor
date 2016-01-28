
using UnityEngine;
using UnityEditor;
using System.IO;

static class TemplateFileUtility
{
    [MenuItem("Assets/Tools/Create Inspector from file")]
    private static void CreateCustomInspector()
    {
        MonoScript ms = Selection.objects[0] as MonoScript;

        string path = AssetDatabase.GetAssetPath(ms);
        string filename = Path.GetFileNameWithoutExtension(path);

        string folder = Path.Combine(Path.GetDirectoryName(path), "Editor");
        Debug.Log("filename: " + filename);
        Debug.Log("folder: " + folder);
        string outPath = Path.Combine(folder, filename)+"Editor.cs";

     
        if(File.Exists(outPath))
        {
            Debug.LogError("Editor File Already Exists! " + outPath);
            return;
        }

        if (!Directory.Exists(folder))
        {
            Debug.Log("Creating Editor Directory");
            Directory.CreateDirectory(folder);
        }

        Debug.Log("Writing Editor File into:"+ outPath);

        string template = File.ReadAllText(Application.dataPath+"/ResourcesEditor/Templates/"+"CustomInspectorTemplate.template");

        string finalText = template.Replace("%0%", filename);
        File.WriteAllText(outPath, finalText);
    }

    [MenuItem("Create Inspector from file", true)]
    static bool ValidateCreateAsset()
    {
        if (Selection.objects != null && Selection.objects.Length == 1 && Selection.objects[0] is MonoScript)
        {
            MonoScript script = Selection.objects[0] as MonoScript;

            if (((script).GetClass().IsSubclassOf(typeof(ScriptableObject)) || (script).GetClass().IsSubclassOf(typeof(MonoBehaviour))) && 
                !((script).GetClass().IsSubclassOf(typeof(UnityEditor.Editor))) &&
                !((script).GetClass().IsSubclassOf(typeof(UnityEditor.EditorWindow))))
            {
                return true;
            }
        }
        return false;

    }

}