using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor.Timeline;
using HanSquirrel.ResourceManager;
using ServiceStack;
using System.Linq;
using System.Collections;
using System.Text;
using GLib;

public class TimelineSkillEditorTools : ScriptableObject
{
    [InitializeOnLoadMethod]
    private static void InitializeOnLoadMethod()
    {
        //EditorApplication.playModeStateChanged -= ExitingEditMode;
        //EditorApplication.playModeStateChanged += ExitingEditMode;
        //Selection.selectionChanged -= Load;
        //Selection.selectionChanged += Load;

        EditorApplication.projectWindowItemOnGUI = (guid, rect) =>
        {
            string selectGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject));
            if (Selection.activeObject && guid == selectGuid && Selection.activeObject.GetType() == typeof(TimelineAsset))
            {
                float width = 40;
                rect.x += rect.width - width;
                rect.width = width;

                if (GUI.Button(rect, "写入"))
                {
                    var timeline = Selection.activeObject as TimelineAsset;
                    var content = TimelineTools.TimelineToScript(timeline);
                    SkillScriptEditManager.Write(timeline.name, content);
                    Debug.Log("[Timeline技能编辑器辅助]写入timeline脚本:" + timeline.name);
                }

                rect.x += rect.width - 80;
                rect.width = width;
                if (GUI.Button(rect, "读取"))
                {
                    string skillId = Selection.activeObject.name;
                    var skillScript = SkillScriptEditManager.Load(skillId);
                    ScriptToTimeline(skillScript, (TimelineAsset) Selection.activeObject);
                    Debug.Log("[Timeline技能编辑器辅助]读取timeline脚本:" + skillId);
                }
            }
        };
    }

    //private static void Load()
    //{
    //    if (Selection.activeObject != null && Selection.activeObject != _currentEditTimeline)
    //    {
    //        //读取
    //        if (Selection.activeObject.GetType() == typeof(TimelineAsset))
    //        {
    //            _currentEditTimeline = (TimelineAsset)Selection.activeObject;
    //            string skillId = Selection.activeObject.name;
    //            var skillScript = SkillScriptManager.Load(skillId);
    //            ScriptToTimeline(skillScript, (TimelineAsset)Selection.activeObject);
    //            Debug.Log("[Timeline技能编辑器辅助]读取timeline脚本:" + skillId);
    //        }
    //        else if (_currentEditTimeline != null) //否则保存当前编辑的
    //        {
    //            var content = TimelineToScript(_currentEditTimeline);
    //            SkillScriptManager.Write(_currentEditTimeline.name, content);
    //            Debug.Log("[Timeline技能编辑器辅助]写入timeline脚本:" + _currentEditTimeline.name);
    //            _currentEditTimeline = null;
    //        }
    //    }
    //}

    //private static void ExitingEditMode(PlayModeStateChange playModeStateChange)
    //{
    //    if(playModeStateChange == PlayModeStateChange.ExitingEditMode)
    //    {
    //        GenerateAllSkillScript();
    //    }
    //}


    [MenuItem("Tools/Timeline技能编辑/全部写入")]
    private static void GenerateAllSkillScript()
    {
        int count = 0;
        DirectoryInfo info = new DirectoryInfo(ConStr.SkillAbsPath);
        string strDestPath = SkillScriptEditManager.skillsDataPath;
        Directory.CreateDirectory(strDestPath);
        TimelineAsset timelineAsset;
        string tlFilePath;
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            if (fsi.Extension.ToLower() == ".playable")
            {
                tlFilePath = Path.Combine(ConStr.SkillAbsPath, fsi.Name);
                timelineAsset = (TimelineAsset) AssetDatabase.LoadAssetAtPath(tlFilePath, typeof(TimelineAsset));
                if (timelineAsset != null)
                {
                    var script = TimelineTools.TimelineToScript(timelineAsset);
                    SkillScriptEditManager.Write(timelineAsset.name, script);
                    count++;
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("[Timeline技能编辑辅助]写入技能数=" + count);
    }

    [MenuItem("Tools/Timeline技能编辑/全部读入（慢，谨慎！）")]
    private static void LoadAllSkillScripts()
    {
        int count = 0;
        DirectoryInfo info = new DirectoryInfo(ConStr.SkillAbsPath);
        string strDestPath = SkillScriptEditManager.skillsDataPath;
        Directory.CreateDirectory(strDestPath);
        TimelineAsset timelineAsset;
        string tlFilePath;
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            if (fsi.Extension.ToLower() == ".playable")
            {
                tlFilePath = Path.Combine(ConStr.SkillAbsPath, fsi.Name);
                timelineAsset = (TimelineAsset) AssetDatabase.LoadAssetAtPath(tlFilePath, typeof(TimelineAsset));
                if (timelineAsset != null)
                {
                    var script = SkillScriptEditManager.Load(timelineAsset.name);
                    if (script != null)
                    {
                        ScriptToTimeline(script, timelineAsset);
                        count++;
                    }
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("[Timeline技能编辑辅助]同步技能数=" + count);
    }

    //从脚本初始化TIMELINE
    public static void ScriptToTimeline(List<string> scripts, TimelineAsset timelineAsset)
    {
        try
        {
            List<TrackAsset> deleteTracks = new List<TrackAsset>();
            foreach (var t in timelineAsset.GetOutputTracks())
            {
                if (t.GetType() == typeof(SkillEventTrack))
                {
                    deleteTracks.Add(t);
                }
            }

            foreach (var t in deleteTracks)
            {
                timelineAsset.DeleteTrack(t);
            }

            bool createNewTrack = true;
            foreach (var t in timelineAsset.GetOutputTracks())
            {
                if (t.name == "SourceEvent")
                {
                    createNewTrack = false;
                }
            }

            if (createNewTrack)
            {
                var eventTrack = timelineAsset.CreateTrack<SkillEventTrack>(null, "SourceEvent");
                Dictionary<string, List<string>> skillEventTrackCmd = new Dictionary<string, List<string>>();

                foreach (var cmd in scripts)
                {
                    int firstPos = cmd.IndexOf('*');
                    int firstPos_ = cmd.IndexOf('#');

                    var time = cmd.Split('*')[0];
                    if (firstPos_ < firstPos || firstPos < 0) time = cmd.Split('#')[0];

                    var truePos = Math.Min(firstPos, firstPos_);
                    if (truePos < 0) truePos = firstPos_;

                    var newCmd = cmd.Substring(truePos + 1);
                    if (!skillEventTrackCmd.ContainsKey(time))
                    {
                        skillEventTrackCmd[time] = new List<string>();
                    }
                    skillEventTrackCmd[time].Add(cmd);
                }

                foreach (var cmd in skillEventTrackCmd)
                {
                    var clip = eventTrack.CreateClip<SkillEventPlayableAsset>();
                    clip.start = Convert.ToDouble(cmd.Key);
                    clip.duration = 1f / 60f;
                    SkillEventPlayableAsset asset = (SkillEventPlayableAsset) clip.asset;
                    asset.m_Cmd = cmd.Value.ToArray();
                }

                AssetDatabase.SaveAssets();
            }
        }
        catch (Exception e)
        {
            Debug.Log(timelineAsset.name + "导入错误！");
        }
    }

    /// <summary>
    /// 技能脚本编辑器管理器
    /// </summary
    public class SkillScriptEditManager
    {
        static public string skillsDataPath = EditorSettings.Instance.RootPath.Sub("data/skills").CreateDir();

        //编辑接口：读取
        static public List<string> Load(string id)
        {
            string path = GetFilePath(id);
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).ToList();
            }
            return null;
        }

        //编辑接口：写入
        static public void Write(string id, List<string> script)
        {
            string path = GetFilePath(id);
            if (script != null)
            {
                File.WriteAllText(path, string.Join("\n", script.ToArray()), Encoding.UTF8);
            }
        }

        static private string GetFilePath(string id)
        {
            return skillsDataPath + id + ".txt";
        }
    }
}
