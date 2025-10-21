using UnityEngine;
using System.Collections;


/// <summary>
/// 播放声音
/// </summary>
public class eftSound : MonoBehaviour
{
    public string SoundEventId; //对应wwise中的eventId

    /// <summary>
    /// 考虑到可能从对象池载入，所以使用OnEnable而非Start/Awake
    /// </summary>
    private void OnEnable()
    {
        //AudioManager.Instance.PlayAtEmitter(SoundEventId, this.gameObject);
    }
}