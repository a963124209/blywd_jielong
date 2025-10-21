using UnityEngine;
using UnityEngine.Playables;

public class SkillEventPlayableAsset : PlayableAsset
{
    public string[] m_Cmd;

    public override double duration { get { return 0.2f; } }

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var scriptPlayable = ScriptPlayable<SkillEventPlayableBehaviour>.Create(graph);
        return scriptPlayable;
    }
}
