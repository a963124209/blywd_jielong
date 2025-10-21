using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(AudioToolkitPlayableAsset), true)]
[TrackBindingType(typeof(GameObject))]
public class AudioToolkitTrack : TrackAsset
{
    public AudioToolkitTrack() { }
}
