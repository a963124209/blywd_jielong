using GLib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EditorSettings : ScriptableObject
{
    private static EditorSettings m_Instance = null;
    public static EditorSettings Instance
    {
        get
        {
            if (m_Instance == null)
            {
                var name = typeof(EditorSettings).Name;
                m_Instance = GetOrCreateAsset<EditorSettings>(name);
            }
            return m_Instance;
        }
    }



    public string ModRootPath
    {
        get
        {
            return RootPath.InVisible() ? null : RootPath.Sub("DeployedMods.HS");
        }
    }

    public string GendataToolPath
    {
        get
        {
            return RootPath.InVisible() ? null : RootPath.Sub("Tools/ModGenData/ModGenData.exe");
        }
    }

    [Tooltip("游戏或者工程的根目录")]
    public string RootPath;

    [MenuItem("Tools/[Editor Settings]", false, 0)]
    private static void Settings()
    {
        var settings = GetOrCreateAsset<EditorSettings>("EditorSettings");
        m_Instance = settings;
        Selection.activeObject = settings;
    }

    private static string AssetFilePath(string fileName)
    {
        return System.IO.Path.Combine("Assets/Resources", fileName + ".asset");
    }

    public static T GetOrCreateAsset<T>(string fileName) where T : ScriptableObject
    {
        var path = AssetFilePath(fileName);
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (!asset)
        {
            asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }

        return asset;
    }
}
