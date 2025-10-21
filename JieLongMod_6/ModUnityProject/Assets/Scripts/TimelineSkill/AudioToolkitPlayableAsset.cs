using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class AudioToolkitPlayableAsset : PlayableAsset
{
    //public ExposedReference<GameObject> m_BattleUnit;
    public string m_AudioID;

    public override double duration { get { return 0.2f; } }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        AudioToolkitPlayableBehaviour playableBehaviour = new AudioToolkitPlayableBehaviour
        {
            m_AudioID = m_AudioID,
            //m_BattleUnit = m_BattleUnit.Resolve(graph.GetResolver()),
        };

        return ScriptPlayable<AudioToolkitPlayableBehaviour>.Create(graph, playableBehaviour);
    }
}
