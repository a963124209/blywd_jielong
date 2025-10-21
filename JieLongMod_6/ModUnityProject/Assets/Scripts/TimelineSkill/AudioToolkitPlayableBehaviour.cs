using UnityEngine;
using UnityEngine.Playables;

public class AudioToolkitPlayableBehaviour : PlayableBehaviour
{
    public string m_AudioID;
    public GameObject m_BattleUnit;
    private bool _isFirstFrameProcess = true;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        m_BattleUnit = playerData as GameObject;
        if (_isFirstFrameProcess)
        {
            _isFirstFrameProcess = false;
            if (!string.IsNullOrEmpty(m_AudioID))
            {
                if (m_BattleUnit != null)
                {
                    AudioController.Play(m_AudioID, m_BattleUnit.transform);
                }
                else
                {
                    AudioController.Play(m_AudioID);
                }
            }
        }
    }
}
