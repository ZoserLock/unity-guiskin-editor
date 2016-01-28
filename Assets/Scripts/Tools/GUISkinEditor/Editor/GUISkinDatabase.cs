using UnityEngine;
using UnityEditor;

using System.Collections;

[InitializeOnLoad]
static class GUISkinDatabase
{
    static GUISkin _defaultSkin;

    static GUISkinDatabase()
    {
        _defaultSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUIStyles/TestGUISkin.guiskin");
    }

    public static GUISkin DefaultSkin
    {
        get
        {
            return _defaultSkin;
        }
    }
}
