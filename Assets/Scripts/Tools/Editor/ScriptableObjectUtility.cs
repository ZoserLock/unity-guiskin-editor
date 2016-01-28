
using UnityEngine;
using UnityEditor;
using System.IO;

static class ScriptableObjectUtility
{
    [MenuItem("Assets/Tools/Create Asset from Selected Script")]
    private static void CreateAsset()
    {
        MonoScript ms = Selection.objects[0] as MonoScript;

        string path = EditorUtility.SaveFilePanel("Save location", "Assets", "" + ms.name, "asset");

        if (string.IsNullOrEmpty(path))
            return;

        var projectRelative = FileUtil.GetProjectRelativePath(path);
        if (string.IsNullOrEmpty(projectRelative))
        {
            EditorUtility.DisplayDialog("Error", "Please select somewhere within your assets folder.", "OK");
            return;
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(projectRelative);

        ScriptableObject scriptableObject = ScriptableObject.CreateInstance(ms.GetClass());
        AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = scriptableObject;

    }

    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Tools/Create Asset from Selected Script", true)]
    static bool ValidateCreateAsset()
    {
        if (Selection.objects != null && Selection.objects.Length == 1 && Selection.objects[0] is MonoScript &&
           (Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject)) &&
           !((Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor))))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}