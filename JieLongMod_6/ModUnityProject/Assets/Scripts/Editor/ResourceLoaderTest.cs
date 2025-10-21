using UnityEditor;
using UnityEngine;
using HanSquirrel.ResourceManager;
using System;
using GLib;
using HSFrameWork.Component;
using HSFrameWork.ConfigTable.Editor;

public class ResourceLoaderTest
{
    //[MenuItem("码农专用/测试资源加载")]
    public static void TestTree()
    {
        ModFacade4App.IncludedMods = null;
        ResourceLoaderConfig.LoadFromABAlways = true;
        try
        {
            var text = ResourceLoader.LoadText("Assets/BuildSource/GameDatas/update_detail.txt");
            Debug.Log(text.ToString());
        }
        finally
        {
            ResourceLoaderCtrl.FullyRestartResourceLoader_DevOnly("测试");
        }
    }

    //[MenuItem("码农专用/显示AB包内容")]
    public static void DisplayABInfo()
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(@"F:\SVNs\Blywd.Resource\DeployedMods.HS\blywd#artwork#0.1\ABS\blywd#artwork#0.1.bundle");
        string.Join(Environment.NewLine, assetBundle.GetAllAssetNames()).SaveToFile(HSCTC.InDebug("xxx.txt"));
        assetBundle.Unload(false);
    }

}