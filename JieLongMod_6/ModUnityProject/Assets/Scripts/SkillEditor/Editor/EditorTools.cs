using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GLib;
using System;

public class EditorTools : ScriptableObject
{
    private static string GameFilePath(string fileName)
    {
        return Path.Combine(EditorSettings.Instance.RootPath, fileName + ".exe");
    }

    //[MenuItem("Tools/Play Game", false, 301)]
    public static void PlayGame()
    {
        Process myprocess = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo(GameFilePath("DesertLegend"));
        startInfo.WorkingDirectory = EditorSettings.Instance.RootPath;
        myprocess.StartInfo = startInfo;
        myprocess.StartInfo.UseShellExecute = false;
        myprocess.Start();
    }
    private static string OutputSoundBanksPath()
    {
        return Path.Combine(EditorSettings.Instance.RootPath, "DesertLegend_Data", "StreamingAssets", "Audio", "GeneratedSoundBanks", "Windows");
    }

    public static void CopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);

            if (fsi is FileInfo)
                File.Copy(fsi.FullName, destName, true);
            else
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }

    [MenuItem("Tools/Export Data", false, 101)]
    public static void ExportData()
    {
        UnityEngine.Debug.Log("Build AssetBundles");
        BuildPipeline.BuildAssetBundles(EditorSettings.Instance.RootPath + "/Mod", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        UnityEngine.Debug.Log("Done.");
    }

    public static T[] LoadAllAsset<T>(string path) where T : UnityEngine.Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            throw new Exception("LoadAllAsset<T>不支持GameObject，请使用LoadPrefab*");
        }

        if (!path.ExistsAsFolder())
        {
            //_Logger.Error("LoadAllAsset错误：路径不存在：[{0}]", path);
            return null;
        }

        //_Logger.Trace("{0}[{1}] Loading ", typeof(T).Name, path);
        List<T> tArray = new List<T>();
        DirectoryInfo direction = new DirectoryInfo(path);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            string fullPath = files[i].FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
            T t = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
            if (t != null && typeof(T) == typeof(Sprite))
            {
                UnityEditor.TextureImporter textureImporter = UnityEditor.AssetImporter.GetAtPath(fullPath) as UnityEditor.TextureImporter;
                if (textureImporter != null)
                {
                    if (textureImporter.spriteImportMode == UnityEditor.SpriteImportMode.Multiple)
                    {
                        UnityEngine.Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(fullPath);
                        for (int j = 0; j < objects.Length; j++)
                        {
                            if (objects[j].GetType() != typeof(Texture2D) && objects[j] != null)
                            {
                                tArray.Add((T)objects[j]);
                            }
                        }
                    }
                }
            }
            if (!tArray.Contains(t) && t != null) tArray.Add(t);
        }
        //_Logger.Trace("{0}[{1}] Loaded {2}个.", typeof(T).Name, path, tArray.Count);
        return tArray.ToArray();
    }
}