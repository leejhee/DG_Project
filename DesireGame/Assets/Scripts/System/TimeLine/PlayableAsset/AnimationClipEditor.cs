#if UNITY_EDITOR
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Client
{
    [CustomTimelineEditor(typeof(AnimationPlayableAsset))]
    public class AnimationClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            Debug.Log("[ClipEditor] OnCreate Called");
            var asset = clip.asset as AnimationPlayableAsset;
            if (asset != null && asset.AnimationClip != null)
            {
                Debug.Log($"Clip Duration Set: {asset.AnimationClip.length}");
                clip.duration = asset.AnimationClip.length;
            }
            base.OnCreate(clip, track, clonedFrom);
        }
    }

}
#endif