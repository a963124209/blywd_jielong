using System;
using UnityEngine;
using UnityEngine.Playables;

public class SkillEventPlayableBehaviour : PlayableBehaviour
{
    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        //Debug.Log("OnBehaviourPlay Called");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        //Debug.Log("OnBehaviourPause Called");
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        // Debug.Log("PrepareFrame Called");
    }
}
