using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelineTools
{
    //从TIMELINE上抽取脚本
    public static List<string> TimelineToScript(TimelineAsset timelineAsset)
    {
        List<string> rst = new List<string>();
        //特效轨道绑定角色
        Dictionary<string, TrackAsset> tracks = new Dictionary<string, TrackAsset>();
        foreach (var t in timelineAsset.GetOutputTracks())
        {
            if (!tracks.ContainsKey(t.name))
            {
                tracks[t.name] = t;
            }
        }

        if (tracks.ContainsKey("SourceEvent"))
        {
            foreach (var c in tracks["SourceEvent"].GetClips())
            {
                SkillEventPlayableAsset asset = (SkillEventPlayableAsset)c.asset;
                for (int i = 0; i < asset.m_Cmd.Length; i++)
                {
                    int firstPos = asset.m_Cmd[i].IndexOf('*');
                    int firstPos_ = asset.m_Cmd[i].IndexOf('#');

                    if (firstPos_ < firstPos || firstPos < 0)
                    {
                        asset.m_Cmd[i] = Math.Round(Convert.ToSingle(c.start), 2) + "#" + asset.m_Cmd[i].Substring(firstPos_ + 1);
                    }
                    else if (firstPos > 0)
                    {
                        asset.m_Cmd[i] = Math.Round(Convert.ToSingle(c.start), 2) + "*" + asset.m_Cmd[i].Substring(firstPos + 1);
                    }
                    else
                    {
                        asset.m_Cmd[i] = Math.Round(Convert.ToSingle(c.start), 2) + "*" + asset.m_Cmd[i];
                    }

                }
                rst.AddRange(asset.m_Cmd);
            }
        }

        return rst;
    }

}
